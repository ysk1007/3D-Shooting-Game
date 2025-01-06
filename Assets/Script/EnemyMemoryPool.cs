using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform target;                               // ���� ��ǥ (�÷��̾�)
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

    private MemoryPool spawnPointMemoryPool;                // �� ���� �˸� ������Ʈ Ȱ��/��Ȱ�� ����
    private MemoryPool[] enemyMemoryPool;                     // �� ����, Ȱ��/��Ȱ�� ����

    private int numberOfEnemiesSpawnedAtOnce = 1;           // ���ÿ� �����Ǵ� ���� ����
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);  // �� ũ��

    [SerializeField] private Transform enemys; // ������ �θ� ������Ʈ
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab, enemys);

        // Enemy �� ���� �����̸� �������� memoryPool ����
        enemyMemoryPool = new MemoryPool[enemyPrefab.Length];
        for (int i = 0; i < enemyPrefab.Length; ++i)
        {
            enemyMemoryPool[i] = new MemoryPool(enemyPrefab[i], enemys);
        }

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 5;

        while (true)
        {
            // ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject enemy = spawnPointMemoryPool.ActivatePoolItem();

                enemy.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                                                        Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));
                enemy.transform.SetParent(enemys);
                StartCoroutine("SpawnEnemy", enemy);
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {

        yield return new WaitForSeconds(enemySpawnLatency);

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
        GameObject enemy = enemyMemoryPool[(int)(EnemyType)Random.Range(0, enemyPrefab.Length)].ActivatePoolItem();
        enemy.transform.SetParent(enemys);
        enemy.transform.position = point.transform.position;

        enemy.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
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
