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

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            // y���� �������� ȸ��
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // ó�� ��ġ�� ��ġ�� �������� y ��ġ�� ��, �Ʒ��� �̵�
            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pinpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponent<Status>().IncreaseHp(increaseHP);

        Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);

        itemMemoryPool.DeactivatePoolItem(this.gameObject);
        //Destroy(gameObject);
    }

    public override void SetUp(MemoryPool itemMemoryPool)
    {
        this.itemMemoryPool = itemMemoryPool;
    }

    public override void PickUp(int index)
    {

    }
}
