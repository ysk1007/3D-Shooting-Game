using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public enum ImpactType { Normal = 0, Obstacle, Enemy, InteractionObject, }

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;      // �ǰ� ����Ʈ
    private MemoryPool[] memoryPool;        // �ǰ� ����Ʈ �޸� Ǯ

    private void Awake()
    {
        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            //memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(WeaponName type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem(position);
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().SetUp(memoryPool[(int)type]);
    }

    public void SpawnImpact(Collider other, Transform knifeTransform)
    {

    }
}
