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

        itemMemoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        //Destroy(gameObject);
    }

    public override void ItemSetUp(int callerViewID, string weaponBaseJson)
    {
        PhotonView callerView = PhotonView.Find(callerViewID);
        this.itemMemoryPool = callerView.GetComponent<MemoryPool>();

        photonView = GetComponent<PhotonView>();
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);
    }

    public override void PickUp(int index, int callerViewID)
    {

    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
