using System.Collections;
using System.Collections.Generic;
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

    private MemoryPool spawnPointMemoryPool;                // �� ���� �˸� ������Ʈ Ȱ��/��Ȱ�� ����
    private MemoryPool[] enemyMemoryPool;                     // �� ����, Ȱ��/��Ȱ�� ����

    private int numberOfEnemiesSpawnedAtOnce = 1;           // ���ÿ� �����Ǵ� ���� ����
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);  // �� ũ��

    [SerializeField] private Transform enemys; // ������ �θ� ������Ʈ

    private void Awake()
    {
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
        int maximumNumber = 50;

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
        yield return new WaitForSeconds(enemySpawnTime);

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
        GameObject enemy = enemyMemoryPool[(int)(WeaponName)Random.Range(0, enemyPrefab.Length)].ActivatePoolItem();
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
}
