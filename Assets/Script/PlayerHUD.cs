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
    [SerializeField]
    private Status status;                      // �÷��̾��� ���� (�̵��ӵ�, ü��)

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

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;        // źâ Ui ������

    [SerializeField]
    private Transform magazineParent;        // źâ Ui�� ��ġ�Ǵ� Panel

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
        SetupWeapon();
        SetupMagazine();

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
        status.onHPEvent.AddListener(UpdateHPHUD);
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
    private void SetupMagazine()
    {
        // weapon �� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
        // magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);

            magazineList.Add(clone);
        }

        // weapon�� ��ϵǾ� �ִ� ���� źâ ������ŭ ������Ʈ Ȱ��ȭ
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            magazineList[i].SetActive(true);
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
