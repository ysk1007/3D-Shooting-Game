using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> targets;                               // 적의 목표 (플레이어)
    [SerializeField]
    private GameObject enemySpawnPointPrefab;               // 적이 등장하기 전 적의 등장 위치를 알려주는 프리팹
    [SerializeField]
    private GameObject[] enemyPrefab;                       // 생성되는 적 프리팹
    [SerializeField]
    private float enemySpawnTime = 1;                       // 적 생성 주기
    [SerializeField]
    private float enemySpawnLatency = 1;                    // 타일 생성 후 적이 등장하기까지 대기 시간

    [SerializeField]
    private int currentEnemy = 0;                           // 생성된 적의 숫자
    [SerializeField]
    private int maximumEnemy = 5;                           // 최대 생성할 수 
    [SerializeField]
    private int ableSpawnNumber = 0;                        // 소환할 수 있는 몬스터 종류

    private MemoryPool spawnPointMemoryPool;                // 적 등장 알림 오브젝트 활성/비활성 관리
    private MemoryPool[] enemyMemoryPool;                     // 적 생성, 활성/비활성 관리

    private int numberOfEnemiesSpawnedAtOnce = 1;           // 동시에 생성되는 적의 숫자
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);  // 맵 크기

    [SerializeField] private Transform enemys; // 관리할 부모 오브젝트
    private PhotonView photonView;

    public List<GameObject> Targets => targets;

    public int CurrentEnemy
    {
        get => currentEnemy;
        set => currentEnemy = value;
    }


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);

        // Enemy 가 여러 종류이면 종류별로 memoryPool 생성
        enemyMemoryPool = new MemoryPool[enemyPrefab.Length];
        for (int i = 0; i < enemyPrefab.Length; ++i)
        {
            enemyMemoryPool[i] = new MemoryPool(enemyPrefab[i]);
        }

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        while (true)
        {
            if (currentEnemy < maximumEnemy && !GameManager.instance.gamePause)
            {
                // 동시에 numberOfEnemiesSpawnedAtOnce 숫자만큼 적이 생성되도록 반복문 사용
                for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
                {
                    GameObject enemy = spawnPointMemoryPool.ActivatePoolItem();

                    enemy.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                                                            Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));
                    enemy.transform.SetParent(enemys);
                    StartCoroutine("SpawnEnemy", enemy);
                    currentEnemy++;
                }
            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {

        yield return new WaitForSeconds(enemySpawnLatency);

        // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
        GameObject enemy = enemyMemoryPool[(int)(EnemyType)Random.Range(0, ableSpawnNumber)].ActivatePoolItem();
        enemy.transform.SetParent(enemys);
        enemy.transform.position = point.transform.position;

        GameObject target;
        int targetNum = 0;
        float distance = 999999;
        for (int i = 0; i < targets.Count; i++)
        {
            float td = Vector3.Distance(this.transform.position, targets[i].transform.position);
            if (td < distance)
            {
                distance = td;
                targetNum = i;
            }
        }

        target = targets[targetNum];

        //enemy.GetComponent<EnemyFSM>().Setup(target, this);
        enemy.GetComponent<PhotonView>().RPC("Setup", RpcTarget.AllBuffered, target.GetComponent<PhotonView>().ViewID, photonView.ViewID);

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DifficultyUpdate(List<float> difficulty)
    {
        maximumEnemy = (int)difficulty[0];
        numberOfEnemiesSpawnedAtOnce = (int)difficulty[1];
        enemySpawnTime = difficulty[2];
        ableSpawnNumber = (int)difficulty[3];
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool[(int)enemy.GetComponent<EnemyFSM>().EnemyType].DeactivatePoolItem(enemy);
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void DeactivateObjectRPC()
    {
        gameObject.SetActive(false);
        //gameObject.transform.SetParent(enemyMemoryPool.ParentTransform);
    }
}
