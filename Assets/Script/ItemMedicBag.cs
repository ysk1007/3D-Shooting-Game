using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemMedicBag : ItemBase
{
    [SerializeField]
    private GameObject hpEffectPrefab;
    [SerializeField]
    private int increaseHP = 50;
    [SerializeField]
    private float moveDistance = 0.2f;
    [SerializeField]
    private float pinpongSpeed = 0.5f;
    [SerializeField]
    private float rotateSpeed = 50;

    private MemoryPool itemMemoryPool;
    private PhotonView photonView;
    private IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            // y축을 기준으로 회전
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // 처음 배치된 위치를 기준으로 y 위치를 위, 아래로 이동
            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pinpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponent<Status>().IncreaseHp(increaseHP);
        photonView = GetComponent<PhotonView>();
        Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);

        itemMemoryPool.DeactivatePoolItem(this.gameObject);
        //Destroy(gameObject);
    }

    public override void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBas)
    {
        this.itemMemoryPool = itemMemoryPool;
    }

    public override void PickUp(int index)
    {

    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void DeactivateObjectRPC()
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(itemMemoryPool.ParentTransform);
    }
}
