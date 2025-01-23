using InfimaGames.LowPolyShooterPack;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private WeaponInfoPopup[] weaponInfoPopup;  // ���� ���� ���� ui

    [SerializeField]
    private WeaponBase currentWeapon;   // ���� ������� ����
    [SerializeField]
    private WeaponBase previousWeapon;  // ������ ����ߴ� ����

    [SerializeField]
    private Transform playerHand;       // �÷��̾� ��

    WeaponBase productWeaponBase;
    WeaponSetting productWeaponSetting;

    public List<WeaponBase> PlayerWeapons => playerWeapons;

    private PhotonView photonView;

    WeaponSetting tempWeaponSetting;
    bool isTemp = false;

    private void Awake()
    {
        instance = this;

        photonView = GetComponent<PhotonView>();

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
        SwitchingWeapon(2);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown || !photonView.IsMine) return;
        // 1~4 ����Ű�� ������ ���� ��ü
        int inputIndex = 0;
        if(int.TryParse(Input.inputString,out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            photonView.RPC("SwitchingWeapon", RpcTarget.AllBuffered, inputIndex - 1);
        }
    }

    public bool PickUpWeapon(WeaponSetting weaponSetting, int index)
    {
        if (playerWeapons[index] != null) {
            tempWeaponSetting = weaponSetting;
            isTemp = true;
            ThrowOutWeapon(index); 
        }
        else
        {
            GameObject gun = GunMemoryPool.instance.SpawnGun(weaponSetting, playerHand);
            playerHUD.WeaponAddListener(gun.GetComponent<WeaponBase>());
            playerWeapons[index] = gun.GetComponent<WeaponBase>();
            gun.GetComponent<WeaponBase>().WeaponSetting = weaponSetting;
            gun.GetComponent<WeaponBase>().PlayerManager = playerController;
            SwitchingWeapon(index);
        }

        return true;
    }

    public void ThrowOutWeapon(int index)
    {
        if (!photonView.IsMine) return;
        photonView.RPC("RPC_ThrowOutWeapon", RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    public void RPC_ThrowOutWeapon(int index)
    {
        ItemMemoryPool.instance.SpawnDropGun(this.transform.position, playerWeapons[index]);
        playerWeapons[index].MemoryPool.DeactivateGun(playerWeapons[index].gameObject);
        playerWeapons[index] = null;
        SwitchingWeapon(2);

        if(isTemp)
        {
            PickUpWeapon(tempWeaponSetting, index);
            isTemp = false;
        }
    }

    [PunRPC]
    private void SwitchingWeapon(int weaponType)
    {
        // ��ü ������ ���Ⱑ ������ ����
        if (playerWeapons[weaponType] == null)
        {
            return;
        }

        // ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if(currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // ���� ��ü
        currentWeapon = playerWeapons[weaponType];

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

    [PunRPC]
    public void DropProduct(string weaponBaseJson)
    {
        WeaponBaseData data = JsonUtility.FromJson<WeaponBaseData>(weaponBaseJson);
        ItemMemoryPool.instance.SpawnDropGun(this.transform.position, weapons[(int)data.weaponSetting.WeaponName], data.weaponSetting);
    }


    [PunRPC]
    public void UpgradeWeapon(int index)
    {
        playerWeapons[index].WeaponLevel += 1;
    }
}
