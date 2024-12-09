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
        // 소지중인 모든 무기의 탄창 수를 increaseMagazine 만큼 증가
        //entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(increaseMagazine);

        // Main 무기의 탄창 수를 increaseMagazine 만큼 증가
        entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(WeaponType.Main,increaseMagazine);

        Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);

        itemMemoryPool.DeactivatePoolItem(this.gameObject);
        //Destroy(gameObject);
    }

    public override void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBase)
    {
        this.itemMemoryPool = itemMemoryPool;
    }

    public override void PickUp(int index)
    {
        
    }
}
