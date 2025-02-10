using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMemoryPool : MonoBehaviourPunCallbacks
{
    public static GunMemoryPool instance;

    [SerializeField] private GameObject[] gunPrefab;   // �� ������
    [SerializeField] private WeaponBase[] weapons;     // �� ������
    [SerializeField] private Vector3[] gunPos;        // �� ���� ��ġ
    [SerializeField] private Vector3 gunRotation;     // �� ����

    private MemoryPool[] gunPool; // �� �޸� Ǯ

    [SerializeField] private Transform guns; // ������� ������ �θ� ������Ʈ

    public WeaponBase[] Weapons => weapons;
    public MemoryPool[] GunPool => gunPool;

    private void Awake()
    {
        instance = this;

        // �迭�� �����ϴ��� üũ �� �ʱ�ȭ
        if (gunPrefab == null || gunPrefab.Length == 0)
        {
            Debug.LogError("[GunMemoryPool] gunPrefab �迭�� ����ֽ��ϴ�!");
            return;
        }

        gunPool = new MemoryPool[gunPrefab.Length];

        for (int i = 0; i < gunPrefab.Length; ++i)
        {
            if (gunPrefab[i] != null)
                gunPool[i] = new MemoryPool(gunPrefab[i]);
            else
                Debug.LogWarning($"[GunMemoryPool] gunPrefab[{i}]��(��) null �Դϴ�.");
        }
    }

    // �÷��̾� �տ� ���� ����
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
            // Photon ��Ʈ��ũ���� ��� Ŭ���̾�Ʈ���� ����ȭ
            gunPhotonView.RPC("Setup", RpcTarget.AllBuffered, gunPhotonView.ViewID);
        }
        else
        {
            Debug.LogWarning($"[GunMemoryPool] {gun.name}�� PhotonView�� �����ϴ�.");
        }

        return gun;
    }

    // �ٴڿ� ����� �� ����
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
            Debug.LogWarning($"[GunMemoryPool] {gun.name}�� PhotonView�� �����ϴ�.");
        }

        return weapons[weaponIndex].WeaponSetting;
    }

    // �ѱ� ��Ȱ��ȭ
    public void DeactivateGun(GameObject gun)
    {
        if (gun == null) return;

        WeaponBase weaponBase = gun.GetComponent<WeaponBase>();
        if (weaponBase == null)
        {
            Debug.LogWarning($"[GunMemoryPool] {gun.name}�� WeaponBase ������Ʈ�� �����ϴ�.");
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
