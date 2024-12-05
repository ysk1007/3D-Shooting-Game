using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private WeaponInfoPopup[] weaponInfoPopups;

    [SerializeField] private bool inventoryIsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)){
            transform.localScale = (inventoryIsOpen) ? Vector3.zero : Vector3.one;
            inventoryIsOpen = !inventoryIsOpen;
            WeaponUiUpdate();
        }
    }

    public void WeaponUiUpdate()
    {
        for (int i = 0; i < weaponInfoPopups.Length; i++)
        {
            if (WeaponSwitchSystem.instance.PlayerWeapons[i] == null){
                weaponInfoPopups[i].SetUpEmpty();
                continue;
            }
            weaponInfoPopups[i].SetUp(WeaponSwitchSystem.instance.PlayerWeapons[i].WeaponSetting,null);
        }
    }
}
