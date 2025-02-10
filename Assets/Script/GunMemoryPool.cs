using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMemoryPool : MonoBehaviourPunCallbacks
{
    public static GunMemoryPool instance;

    [SerializeField] private GameObject[] gunPrefab;   // 총 프리팹
    [SerializeField] private WeaponBase[] weapons;     // 총 데이터
    [SerializeField] private Vector3[] gunPos;        // 총 생성 위치
    [SerializeField] private Vector3 gunRotation;     // 총 각도

    private MemoryPool[] gunPool; // 총 메모리 풀

    [SerializeField] private Transform guns; // 무기들을 관리할 부모 오브젝트

    public WeaponBase[] Weapons => weapons;
    public MemoryPool[] GunPool => gunPool;

    private void Awake()
    {
        instance = this;

        // 배열이 존재하는지 체크 후 초기화
        if (gunPrefab == null || gunPrefab.Length == 0)
        {
            Debug.LogError("[GunMemoryPool] gunPrefab 배열이 비어있습니다!");
            return;
        }

        gunPool = new MemoryPool[gunPrefab.Length];

        for (int i = 0; i < gunPrefab.Length; ++i)
        {
            if (gunPrefab[i] != null)
                gunPool[i] = new MemoryPool(gunPrefab[i]);
            else
                Debug.LogWarning($"[GunMemoryPool] gunPrefab[{i}]이(가) null 입니다.");
        }
    }

    // 플레이어 손에 무기 생성
    public GameObject SpawnGun(WeaponSetting weaponSetting, Transform playerHand)
    {
        int weaponIndex = (int)weaponSetting.WeaponName;

        GameObject gun = gunPool[weaponIndex].ActivatePoolItem(playerHand.position);
        gun.transform.SetParent(playerHand);
        gun.transform.localPosition = gunPos[weaponIndex];
        gun.transform.localEulerAngles = gunRotation;

        PhotonView gunPhotonView = gun.GetComponent<PhotonView>();

        if (gunPhotonView != null)
        {
            // Photon 네트워크에서 모든 클라이언트에게 동기화
            gunPhotonView.RPC("Setup", RpcTarget.AllBuffered, gunPhotonView.ViewID);
        }
        else
        {
            Debug.LogWarning($"[GunMemoryPool] {gun.name}에 PhotonView가 없습니다.");
        }

        return gun;
    }

    // 바닥에 드랍된 총 생성
    public WeaponSetting SpawnGun(WeaponName type, Transform spawnPoint)
    {
        int weaponIndex = (int)type;

        GameObject gun = gunPool[weaponIndex].ActivatePoolItem(spawnPoint.position);
        gun.transform.SetParent(spawnPoint);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        PhotonView gunPhotonView = gun.GetComponent<PhotonView>();

        if (gunPhotonView != null)
        {
            gunPhotonView.RPC("Setup", RpcTarget.AllBuffered, gunPhotonView.ViewID);
        }
        else
        {
            Debug.LogWarning($"[GunMemoryPool] {gun.name}에 PhotonView가 없습니다.");
        }

        return weapons[weaponIndex].WeaponSetting;
    }

    // 총기 비활성화
    public void DeactivateGun(GameObject gun)
    {
        if (gun == null) return;

        WeaponBase weaponBase = gun.GetComponent<WeaponBase>();
        if (weaponBase == null)
        {
            Debug.LogWarning($"[GunMemoryPool] {gun.name}에 WeaponBase 컴포넌트가 없습니다.");
            return;
        }

        int weaponIndex = (int)weaponBase.WeaponName;

        if (weaponIndex < 0 || weaponIndex >= gunPool.Length || gunPool[weaponIndex] == null)
        {
            Debug.LogError($"[GunMemoryPool] Invalid weapon index: {weaponIndex}");
            return;
        }

        gunPool[weaponIndex].DeactivatePoolItem(gun);
    }
}
