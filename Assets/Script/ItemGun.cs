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

    private void Start()
    {
        
    }

    private IEnumerator Rotate()
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

    }

    public override void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBase)
    {
        weaponSwitchSystem = WeaponSwitchSystem.instance;
        this.itemMemoryPool = itemMemoryPool;

        //int weaponCount = System.Enum.GetNames(typeof(WeaponName)).Length;
        //int randomIndex = Random.Range(0, weaponCount); // 0부터 weaponCount - 1까지의 랜덤 정수
        if (weaponBase != null) { 
            weaponSetting = weaponBase.WeaponSetting;
            GunMemoryPool.instance.SpawnGun(weaponBase.WeaponSetting.WeaponName, this.transform);
        }
        else
            weaponSetting = GunMemoryPool.instance.SpawnGun((WeaponName)Random.Range(0, 2), this.transform);

        StartCoroutine("Rotate");
    }

    public override void PickUp(int index)
    {
        if(weaponSwitchSystem.PickUpWeapon(weaponSetting, index))
        {
            itemMemoryPool.DeactivatePoolItem(this.gameObject);
            GameObject gun = transform.GetComponentInChildren<WeaponBase>().gameObject;
            gun.GetComponent<WeaponBase>().MemoryPool.DeactivatePoolItem(gun);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().WeaponInfoPopup.SetUp(weaponSetting, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().WeaponInfoPopup.init();
        }
    }
}
