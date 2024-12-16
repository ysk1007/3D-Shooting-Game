using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class ItemGun : ItemBase
{
    [Header("������ ����")]
    [SerializeField] private WeaponSetting weaponSetting;

    [Header("������ ȸ�� �ӵ�")]
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float pinpongSpeed = 0.5f;
    [SerializeField] private float rotateSpeed = 50;

    [Header("������ ����")]
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

    }

    public override void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBase)
    {
        weaponSwitchSystem = WeaponSwitchSystem.instance;
        this.itemMemoryPool = itemMemoryPool;

        //int weaponCount = System.Enum.GetNames(typeof(WeaponName)).Length;
        //int randomIndex = Random.Range(0, weaponCount); // 0���� weaponCount - 1������ ���� ����
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
