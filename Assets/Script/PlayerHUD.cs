using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Compents")]
    [SerializeField]
    private WeaponBase weapon;                  // 현재 정보가 출력되는 무기
    [SerializeField]
    private Status status;                      // 플레이어의 상태 (이동속도, 체력)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;     // 무기이름
    [SerializeField]
    private Image imageWeaponIcon;              // 무기 아이콘
    [SerializeField]
    private Vector2 sizeWeaponIcons;          // 무기 아이콘의 UI 크기

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;           // 현재/최대 탄 수 출력 Text

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;        // 탄창 Ui 프리팹

    [SerializeField]
    private Transform magazineParent;        // 탄창 Ui가 배치되는 Panel
    [SerializeField]
    private int maxMagazineCount;               // 처음 생성하는 최대 탄창 수

    [SerializeField]
    private List<GameObject> magazineList;        // 탄창 Ui 리스트

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;                 // 플레이어의 체력을 출력하는 Text
    [SerializeField]
    private Slider sliderHP;                        // 플레이어의 체력을 출력하는 Slider
    [SerializeField]
    private Image imageBloodScreen;                 // 플레이어가 공격받았을 때 화면에 표시되는 Image
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    [Header("Coin")]
    [SerializeField]
    private TextMeshProUGUI textCoin;               // 플레이어의 코인을 출력하는 Text

    private void Awake()
    {
        //instance = this;
        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        status.onHPEvent.AddListener(UpdateHPHUD);
        GetComponent<PlayerManager>().onCoinEvent.AddListener(UpdateCoinHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        // 사용 가능한 모든 무기의 이벤트 등록
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void WeaponAddListener(WeaponBase weapon)
    {
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
    }

    public void SwtchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponSetting.weaponName;
        //imageWeaponIcon.sprite = weapon.WeaponSetting.weaponSprite;
        imageWeaponIcon.sprite = Resources.Load<Sprite>("Sprites/" + weapon.WeaponSetting.WeaponName.ToString());
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons;
        UpdateAmmoHUD(weapon.WeaponSetting.currentAmmo, weapon.WeaponSetting.maxAmmo);
        UpdateMagazineHUD(weapon.WeaponSetting.currentMagazine);
    }
    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=24>{currentAmmo}<color=\"grey\">/</size>{maxAmmo}</color>";
    }
    private void SetupMagazine()
    {
        // weapon 에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
        // magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
        magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }
    }
    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화하고, currentMagazine 개수만큼 활성화
        for (int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }

        for (int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateHPHUD(float previous, float current)
    {
        textHP.text = $"{current}/{status.MaxHP}";
        sliderHP.value = current / status.MaxHP;

        // 체력이 증가했을 때는 화면에 빨간색 이미지를 출력하지 않도록 return
        if (previous <= current) return;

        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private void UpdateCoinHUD(int value)
    {
        textCoin.text = value.ToString();
    }


    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
