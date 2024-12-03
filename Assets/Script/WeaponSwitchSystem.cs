using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerManager playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons;        // �������� ���� 4����

    [SerializeField]
    private WeaponBase currentWeapon;   // ���� ������� ����
    [SerializeField]
    private WeaponBase previousWeapon;  // ������ ����ߴ� ����

    private void Awake()
    {
        // ���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetupAllWeapons(weapons);

        // ���� �������� ��� ���⸦ ������ �ʰ� ����
        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        // Main ���⸦ ���� ��� ����� ����
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        // 1~4 ����Ű�� ������ ���� ��ü
        int inputIndex = 0;
        if(int.TryParse(Input.inputString,out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // ��ü ������ ���Ⱑ ������ ����
        if (weapons[(int)weaponType] == null)
        {
            return;
        }

        // ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if(currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // ���� ��ü
        currentWeapon = weapons[(int)weaponType];

        // ���� ������� ����� ��ü�Ϸ��� �� �� ����
        if (currentWeapon == previousWeapon)
        {
            return;
        }

        // ���⸦ ����ϴ� PlayerController, PlayerHUD�� ���� ���� ���� ����
        playerController.SwitcningWeapon(currentWeapon);
        playerHUD.SwtchingWeapon(currentWeapon);

        // ������ ����ϴ� ���� ��Ȱ��ȭ
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }

        // ���� ����ϴ� ���� Ȱ��ȭ
        currentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// ù ��° �Ű������� ������ �ϳ��� ���� źâ �� ����
    /// </summary>
    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        // �ش� ���Ⱑ �ִ��� �˻�
        if (weapons[(int)weaponType] != null)
        {
            // �ش� ������ źâ ���� magazine��ŭ ����
            weapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }

    /// <summary>
    /// �������� ��� ������ źâ �� ����
    /// </summary>
    public void IncreaseMagazine(int magazine)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(magazine);
            }
        }
    }
}
