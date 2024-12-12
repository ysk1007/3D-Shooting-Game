using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;
    [Header("Compents")]
    [SerializeField]
    private WeaponBase weapon;                  // ���� ������ ��µǴ� ����
    [SerializeField]
    private Status status;                      // �÷��̾��� ���� (�̵��ӵ�, ü��)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;     // �����̸�
    [SerializeField]
    private Image imageWeaponIcon;              // ���� ������
    [SerializeField]
    private Vector2 sizeWeaponIcons;          // ���� �������� UI ũ��

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;           // ����/�ִ� ź �� ��� Text

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;        // źâ Ui ������

    [SerializeField]
    private Transform magazineParent;        // źâ Ui�� ��ġ�Ǵ� Panel
    [SerializeField]
    private int maxMagazineCount;               // ó�� �����ϴ� �ִ� źâ ��

    [SerializeField]
    private List<GameObject> magazineList;        // źâ Ui ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;                 // �÷��̾��� ü���� ����ϴ� Text
    [SerializeField]
    private Slider sliderHP;                        // �÷��̾��� ü���� ����ϴ� Slider
    [SerializeField]
    private Image imageBloodScreen;                 // �÷��̾ ���ݹ޾��� �� ȭ�鿡 ǥ�õǴ� Image
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        instance = this;
        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        // ��� ������ ��� ������ �̺�Ʈ ���
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
        imageWeaponIcon.sprite = weapon.WeaponSetting.weaponSprite;
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
        // weapon �� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
        // magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
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
        // ���� ��Ȱ��ȭ�ϰ�, currentMagazine ������ŭ Ȱ��ȭ
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

        // ü���� �������� ���� ȭ�鿡 ������ �̹����� ������� �ʵ��� return
        if (previous <= current) return;

        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
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
