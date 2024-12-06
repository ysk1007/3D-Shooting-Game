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

    private IEnumerator Start()
    {
        weaponSwitchSystem = WeaponSwitchSystem.instance;

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

    public override void SetUp(MemoryPool itemMemoryPool)
    {
        this.itemMemoryPool = itemMemoryPool;

        //int weaponCount = System.Enum.GetNames(typeof(WeaponName)).Length;
        //int randomIndex = Random.Range(0, weaponCount); // 0���� weaponCount - 1������ ���� ����

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
