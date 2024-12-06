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
    private WeaponBase[] weapons;        // 사용가능한 무기들

    [SerializeField] private List<WeaponBase> playerWeapons;    // 소지중인 무기 3종류

    [SerializeField]
    private WeaponBase currentWeapon;   // 현재 사용중인 무기
    [SerializeField]
    private WeaponBase previousWeapon;  // 직전에 사용했던 무기

    [SerializeField]
    private Transform playerHand;       // 플레이어 손

    public List<WeaponBase> PlayerWeapons => playerWeapons;

    private void Awake()
    {
        instance = this;

        // 무기 정보 출력을 위해 현재 소지중인 모든 무기 이벤트 등록
        playerHUD.SetupAllWeapons(weapons);

        // 현재 소지중인 모든 무기를 보이지 않게 설정
        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        // Main 무기를 현재 사용 무기로 설정
        SwitchingWeapon(WeaponType.Sub);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        // 1~4 숫자키를 누르면 무기 교체
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
        // 교체 가능한 무기가 없으면 종료
        if (playerWeapons[(int)weaponType] == null)
        {
            return;
        }

        // 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if(currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // 무기 교체
        currentWeapon = playerWeapons[(int)weaponType];

        // 현재 사용중인 무기로 교체하려고 할 때 종료
        if (currentWeapon == previousWeapon)
        {
            return;
        }

        // 무기를 사용하는 PlayerController, PlayerHUD에 현재 무기 정보 전달
        playerController.SwitcningWeapon(currentWeapon);
        playerHUD.SwtchingWeapon(currentWeapon);

        // 이전에 사용하던 무기 비활성화
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }

        // 현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// 첫 번째 매개변수에 설정된 하나의 무기 탄창 수 증가
    /// </summary>
    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        // 해당 무기가 있는지 검사
        if (playerWeapons[(int)weaponType] != null)
        {
            // 해당 무기의 탄창 수를 magazine만큼 증가
            playerWeapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }

    /// <summary>
    /// 소지중인 모든 무기의 탄창 수 증가
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
