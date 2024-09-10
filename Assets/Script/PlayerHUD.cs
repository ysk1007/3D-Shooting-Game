using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Compents")]
    [SerializeField]
    private WeaponAssaultRifle weapon;          // ���� ������ ��µǴ� ����

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;     // �����̸�
    [SerializeField]
    private Image imageWeaponIcon;              // ���� ������
    [SerializeField]
    private Sprite[] spriteWeaponIcons;         // ���� �����ܿ� ���Ǵ� sprite �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;           // ����/�ִ� ź �� ��� Text

    private void Awake()
    {
        SetupWeapon();

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }
}
