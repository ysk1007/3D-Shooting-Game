using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> targets;                               // ���� ��ǥ (�÷��̾�)
    [SerializeField]
    private GameObject enemySpawnPointPrefab;               // ���� �����ϱ� �� ���� ���� ��ġ�� �˷��ִ� ������
    [SerializeField]
    private GameObject[] enemyPrefab;                       // �����Ǵ� �� ������
    [SerializeField]
    private float enemySpawnTime = 1;                       // �� ���� �ֱ�
    [SerializeField]
    private float enemySpawnLatency = 1;                    // Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�

    [SerializeField]
    private int currentEnemy = 0;                           // ������ ���� ����
    [SerializeField]
    private int maximumEnemy = 5;                           // �ִ� ������ �� 
    [SerializeField]
    private int ableSpawnNumber = 0;                        // ��ȯ�� �� �ִ� ���� ����

    private MemoryPool spawnPointMemoryPool;                // �� ���� �˸� ������Ʈ Ȱ��/��Ȱ�� ����
    private MemoryPool[] enemyMemoryPool;                     // �� ����, Ȱ��/��Ȱ�� ����

    private int numberOfEnemiesSpawnedAtOnce = 1;           // ���ÿ� �����Ǵ� ���� ����
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);  // �� ũ��

    [SerializeField] private Transform enemys; // ������ �θ� ������Ʈ
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

        // Enemy �� ���� �����̸� �������� memoryPool ����
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
                // ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
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

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
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

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
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

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void DeactivateObjectRPC()
    {
        gameObject.SetActive(false);
        //gameObject.transform.SetParent(enemyMemoryPool.ParentTransform);
    }
}
