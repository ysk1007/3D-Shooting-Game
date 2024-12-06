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

    [SerializeField] private Transform guns;
    [SerializeField] private Transform playerHand; // 관리할 부모 오브젝트

    public WeaponBase[] Weapons => weapons;
    public MemoryPool[] GunPool => gunPool;

    private void Awake()
    {
        instance = this;

        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        gunPool = new MemoryPool[gunPrefab.Length];
        for (int i = 0; i < gunPrefab.Length; ++i)
        {
            gunPool[i] = new MemoryPool(gunPrefab[i], guns);
        }
    }

    // 플레이어 손 무기 생성
    public GameObject SpawnGun(WeaponName type)
    {
        GameObject gun = gunPool[(int)type].ActivatePoolItem();
        gun.gameObject.SetActive(false);
        gun.transform.SetParent(playerHand);
        gun.transform.localPosition = gunPos[(int)type];
        gun.transform.localEulerAngles = gunRotation;
        gun.GetComponent<WeaponBase>().Setup(gunPool[(int)type]);
        return gun;
    }

    // 드랍된 총 생성
    public WeaponSetting SpawnGun(WeaponName type, Transform transform)
    {
        GameObject gun = gunPool[(int)type].ActivatePoolItem();
        gun.gameObject.SetActive(true);
        gun.transform.SetParent(transform);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;
        gun.GetComponent<WeaponBase>().Setup(gunPool[(int)type]);
        return weapons[(int)type].WeaponSetting;
    }
}
