using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMemoryPool : MonoBehaviour
{
    public static GunMemoryPool instance;

    [SerializeField] private GameObject[] gunPrefab;      // 총 프리팹
    [SerializeField] private WeaponBase[] weapons;        // 총 데이터
    private MemoryPool[] gunPool;        // 총 메모리 풀
    [SerializeField] private Vector3[] gunPos;            // 총 생성 위치
    [SerializeField] private Vector3 gunRotation;       // 총 각도

    [SerializeField] private Transform playerHand; // 관리할 부모 오브젝트

    public WeaponBase[] Weapons => weapons;

    private void Awake()
    {
        instance = this;

        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        gunPool = new MemoryPool[gunPrefab.Length];
        for (int i = 0; i < gunPrefab.Length; ++i)
        {
            gunPool[i] = new MemoryPool(gunPrefab[i]);
        }
    }

    public GameObject SpawnGun(WeaponName type)
    {
        GameObject gun = gunPool[(int)type].ActivatePoolItem();
        gun.transform.SetParent(playerHand);
        gun.transform.localPosition = gunPos[(int)type];
        gun.transform.localEulerAngles = gunRotation;
        return gun;
    }
}
