using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class ItemGun : ItemBase
{
    [Header("아이템 세팅")]
    [SerializeField] private WeaponSetting weaponSetting;

    [Header("아이템 회전 속도")]
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float pinpongSpeed = 0.5f;
    [SerializeField] private float rotateSpeed = 50;

    [Header("아이템 세팅")]
    //[SerializeField] private WeaponInfoPopup weaponInfoPopup;

    WeaponSwitchSystem weaponSwitchSystem;

    private MemoryPool itemMemoryPool;

    private IEnumerator Start()
    {
        weaponSwitchSystem = WeaponSwitchSystem.instance;

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

    }

    public override void SetUp(MemoryPool itemMemoryPool)
    {
        this.itemMemoryPool = itemMemoryPool;

        //int weaponCount = System.Enum.GetNames(typeof(WeaponName)).Length;
        //int randomIndex = Random.Range(0, weaponCount); // 0부터 weaponCount - 1까지의 랜덤 정수

        weaponSetting = GunMemoryPool.instance.SpawnGun(/*(WeaponName)Random.Range(0, 2)*/(WeaponName)1, this.transform);
    }

    public override void PickUp(int index)
    {
        if(weaponSwitchSystem.PickUpWeapon(weaponSetting.WeaponName, index))
        {
            itemMemoryPool.DeactivatePoolItem(this.gameObject);
            GameObject gun = transform.GetComponentInChildren<WeaponBase>().gameObject;
            gun.GetComponent<WeaponBase>().MemoryPool.DeactivatePoolItem(gun);
            WeaponInfoPopup.instance.init();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            WeaponInfoPopup.instance.SetUp(weaponSetting, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            WeaponInfoPopup.instance.init();
        }
    }
}
