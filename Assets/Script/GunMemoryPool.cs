using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMemoryPool : MonoBehaviour
{
    public static GunMemoryPool instance;

    [SerializeField] private GameObject[] gunPrefab;      // �� ������
    [SerializeField] private WeaponBase[] weapons;        // �� ������
    private MemoryPool[] gunPool;        // �� �޸� Ǯ
    [SerializeField] private Vector3[] gunPos;            // �� ���� ��ġ
    [SerializeField] private Vector3 gunRotation;       // �� ����

    [SerializeField] private Transform playerHand; // ������ �θ� ������Ʈ

    public WeaponBase[] Weapons => weapons;

    private void Awake()
    {
        instance = this;

        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
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
