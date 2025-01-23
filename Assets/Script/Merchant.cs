using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private GunMemoryPool gunMemoryPool;
    [SerializeField] private List<WeaponSetting> weaponList;
    [SerializeField] private GameObject keyDesc;

    private void OnEnable()
    {
        weaponListUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            keyDesc.SetActive(true);
            other.GetComponent<PlayerManager>().ShopUi(weaponList);
            other.GetComponent<PlayerManager>().visitShop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            keyDesc.SetActive(false);
            other.GetComponent<PlayerManager>().visitShop = false;
            other.GetComponent<PlayerManager>().VisitShop();
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
