using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponSwitchSystem : MonoBehaviour
{
    public static WeaponSwitchSystem instance;

    [SerializeField]
    private PlayerManager playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons;        // ��밡���� �����

    [SerializeField] private List<WeaponBase> playerWeapons;    // �������� ���� 3����

    [SerializeField]
    private WeaponBase currentWeapon;   // ���� ������� ����
    [SerializeField]
    private WeaponBase previousWeapon;  // ������ ����ߴ� ����

    [SerializeField]
    private Transform playerHand;       // �÷��̾� ��

    public List<WeaponBase> PlayerWeapons => playerWeapons;

    private void Awake()
    {
        instance = this;

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
        SwitchingWeapon(WeaponType.Sub);
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

    public bool PickUpWeapon(WeaponName weapon,int index)
    {
        GameObject gun =  GunMemoryPool.instance.SpawnGun(weapon);
        PlayerHUD.instance.WeaponAddListener(gun.GetComponent<WeaponBase>());
        playerWeapons[index] = gun.GetComponent<WeaponBase>();

        return true;
    }

    public void ThrowOutWeapon(int index)
    {
        playerWeapons[index].MemoryPool.DeactivatePoolItem(playerWeapons[index].gameObject);
        playerWeapons[index] = null;
        SwitchingWeapon(WeaponType.Sub);
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // ��ü ������ ���Ⱑ ������ ����
        if (playerWeapons[(int)weaponType] == null)
        {
            return;
        }

        // ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if(currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // ���� ��ü
        currentWeapon = playerWeapons[(int)weaponType];

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
        if (playerWeapons[(int)weaponType] != null)
        {
            // �ش� ������ źâ ���� magazine��ŭ ����
            playerWeapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }

    /// <summary>
    /// �������� ��� ������ źâ �� ����
    /// </summary>
    public void IncreaseMagazine(int magazine)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (playerWeapons[i] != null)
            {
                playerWeapons[i].IncreaseMagazine(magazine);
            }
        }
    }
}
