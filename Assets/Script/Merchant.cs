using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private GunMemoryPool gunMemoryPool;
    [SerializeField] private List<WeaponSetting> weaponList;

    private void OnEnable()
    {
        weaponListUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerManager>().ShopUi(true, weaponList);
        }
    }

    void weaponListUpdate()
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            WeaponSetting newWeaponSetting = gunMemoryPool.Weapons[Random.Range(0, 2)].WeaponSetting;
            newWeaponSetting.RandomLevel();
            weaponList[i] = newWeaponSetting;
        }
    }
}
