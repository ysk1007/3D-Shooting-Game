using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoPopup : MonoBehaviour
{
    [Header("무기 스크립트")]
    [SerializeField] private WeaponSetting weapon;

    [Header("무기 아이콘")]
    [SerializeField] private Image weaponIcon;

    [Header("무기 텍스트")]
    [SerializeField] private TextMeshProUGUI weaponNameText;            // 무기 이름 텍스트
    [SerializeField] private TextMeshProUGUI weaponDamageText;          // 무기 대미지 텍스트
    [SerializeField] private TextMeshProUGUI weaponCriticalText;        // 무기 연사 속도 텍스트
    [SerializeField] private TextMeshProUGUI weaponAttackRateText;      // 무기 연사 속도 텍스트
    [SerializeField] private TextMeshProUGUI weaponAmmoText;            // 탄창 용량 텍스트

    [SerializeField] private ItemBase item;

    [SerializeField] private Transform uiSet;

    [SerializeField] private bool isPickupPopup;
    [SerializeField] private bool pickupPopupActive;

    void Start()
    {

    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!pickupPopupActive) return;
        if (!Input.anyKeyDown) return;

        if (Input.GetKey(KeyCode.Q)) PickUpGun(0);
        if (Input.GetKey(KeyCode.E)) PickUpGun(1);
    }

    public void SetUp(WeaponSetting weapon, ItemBase item = null)
    {
        uiSet.localScale = Vector3.one;
        weaponNameText.text = weapon.weaponName;
        weaponDamageText.text = weapon.damage.ToString();
        weaponCriticalText.text = "x"+weapon.critical.ToString();
        weaponAttackRateText.text = weapon.attackRate.ToString("F1");
        weaponAmmoText.text = weapon.maxAmmo.ToString();
        weaponIcon.sprite = weapon.weaponSprite;

        this.item = item;

        transform.localScale = Vector3.one;


        if(isPickupPopup) pickupPopupActive = true;
    }

    public void SetUpEmpty()
    {
        uiSet.localScale = Vector3.zero;
    }

    public void init()
    {
        this.item = null;
        transform.localScale = Vector3.zero;
        pickupPopupActive = false;
    }

    public void PickUpGun(int index)
    {
        item.PickUp(index);
    }
}
