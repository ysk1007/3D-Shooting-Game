using Photon.Pun;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField]
    private GameObject magazineEffectPrefab;
    [SerializeField]
    private int increaseMagazine = 2;

    [SerializeField]
    private float rotateSpeed = 50;

    private MemoryPool itemMemoryPool;
    private PhotonView photonView;

    private IEnumerator Start()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        // �������� ��� ������ źâ ���� increaseMagazine ��ŭ ����
        //entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(increaseMagazine);

        // Main ������ źâ ���� increaseMagazine ��ŭ ����
        entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(WeaponType.Main,increaseMagazine);

        Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);

        itemMemoryPool.DeactivatePoolItem(this.gameObject);
        //Destroy(gameObject);
    }

    public override void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBase)
    {
        this.itemMemoryPool = itemMemoryPool;
        photonView = GetComponent<PhotonView>();
    }

    public override void PickUp(int index)
    {
        
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void DeactivateObjectRPC()
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(itemMemoryPool.ParentTransform);
    }
}
