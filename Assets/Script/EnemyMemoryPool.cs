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
    private GameObject enemyPrefab;                         // �����Ǵ� �� ������
    [SerializeField]
    private float enemySpawnTime = 1;                       // �� ���� �ֱ�
    [SerializeField]
    private float enemySpawnLatency = 1;                    // Ÿ�� ���� ���� ���� �����ϱ���� ��� �ð�

    private MemoryPool spawnPointMemoryPool;                // �� ���� �˸� ������Ʈ Ȱ��/��Ȱ�� ����
    private MemoryPool enemyMemoryPool;                     // �� ����, Ȱ��/��Ȱ�� ����

    private int numberOfEnemiesSpawnedAtOnce = 1;           // ���ÿ� �����Ǵ� ���� ����
    private Vector2Int mapSize = new Vector2Int(100, 100);  // �� ũ��

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

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
        GameObject enemy = enemyMemoryPool.ActivatePoolItem();
        enemy.transform.position = point.transform.position;

        enemy.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
}
