using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public enum ImpactType { Normal = 0, Obstacle, Enemy, InteractionObject, }

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;      // 피격 이펙트
    private MemoryPool[] memoryPool;        // 피격 이펙트 메모리 풀

    private void Awake()
    {
        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
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
