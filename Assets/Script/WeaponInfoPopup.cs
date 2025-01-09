using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoPopup : MonoBehaviour
{
    [Header("���� ��ü �ý���")]
    [SerializeField] private WeaponSwitchSystem weaponSwitchSystem;

    [Header("���� ��ũ��Ʈ")]
    [SerializeField] private WeaponSetting weapon;

    [Header("���� ������")]
    [SerializeField] private Image weaponIcon;

    [Header("���� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI weaponNameText;            // ���� �̸� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI weaponDamageText;          // ���� ����� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI weaponCriticalText;        // ���� ���� �ӵ� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI weaponAttackRateText;      // ���� ���� �ӵ� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI weaponAmmoText;            // źâ �뷮 �ؽ�Ʈ

    [SerializeField] private ItemBase item;

    [SerializeField] private Transform uiSet;

    [SerializeField] private bool isPickupPopup;
    [SerializeField] private bool pickupPopupActive;
    [SerializeField] private PhotonView photonView;

    private void Awake()
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
        this.weapon = weapon;
        weaponNameText.text = weapon.weaponName;
        weaponDamageText.text = weapon.damage.ToString();
        weaponCriticalText.text = "x"+weapon.critical.ToString();
        weaponAttackRateText.text = weapon.attackRate.ToString("F1");
        weaponAmmoText.text = weapon.maxAmmo.ToString();
        //weaponIcon.sprite = weapon.weaponSprite;
        weaponIcon.sprite = Resources.Load<Sprite>("Sprites/"+ weapon.WeaponName.ToString());

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
        photonView = weaponSwitchSystem.gameObject.GetComponent<PhotonView>();
        item.GetComponent<PhotonView>().RPC("PickUp",RpcTarget.AllBuffered,index, photonView.ViewID);
        //item.PickUp(index, callerViewID);
        init();
    }
}
