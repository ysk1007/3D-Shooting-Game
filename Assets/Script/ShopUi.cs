using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUi : MonoBehaviour
{
    [SerializeField] private PlayerManager player;

    [SerializeField] private WeaponInfoPopup[] weaponInfoPopups;
    [SerializeField] private WeaponInfoPopup[] productInfoPopups;
    [SerializeField] private WeaponSwitchSystem weaponSwitchSystem;

    [SerializeField] private bool inventoryIsOpen = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WeaponUiUpdate()
    {
        for (int i = 0; i < weaponInfoPopups.Length; i++)
        {
            if (weaponSwitchSystem.PlayerWeapons[i] == null)
            {
                weaponInfoPopups[i].SetUpEmpty();
                continue;
            }
            weaponInfoPopups[i].SetUp(weaponSwitchSystem.PlayerWeapons[i].WeaponSetting, null);
        }
    }

    public void WeaponUpgrade(int index)
    { 
        weaponSwitchSystem.gameObject.GetComponent<PhotonView>().RPC("UpgradeWeapon", RpcTarget.AllBuffered, index);
    }

    public void Shop(bool open)
    {
        transform.localScale = open ? Vector3.one : Vector3.zero;
        if (open) {
            WeaponUiUpdate();
        }
    }

   public void ProductBuy(int index)
    {
        WeaponSetting weaponSetting = productInfoPopups[index].weaponSetting;
        if (player.Coin >= weaponSetting.GetPrice)
        {
            player.CoinUpdate(-weaponSetting.GetPrice);
            weaponSwitchSystem.ProductBuy(weaponSetting);
            productInfoPopups[index].SetUpEmpty();
        }
    }

    public void ProductUpdate(List<WeaponSetting> weaponList)
    {
        for (int i = 0; i < productInfoPopups.Length; i++)
        {
            productInfoPopups[i].SetUp(weaponList[i], null);
        }
    }
}
