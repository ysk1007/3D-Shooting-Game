using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// ������ ������ ���� ������ �� �������� ����ϴ� �������� ����ü�� ��� �����ϸ�
// ������ �߰�/������ �� ����ü�� �����ϱ� �빮�� �߰�/������ ���� ������ ������

public class Opctions
{

}

public enum WeaponName { AssaultRifle = 0, SawGun, Shotgun, Minigun, ChemicalGun, FlameThrower, /*CombatKnife, HandGrenade,*/ Pistol }

[Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;   // ���� �̸�
    public int weaponLevel;         // ���� ����
    public string weaponName;       // ���� �̸�
    public float damage;            // ���� ���ݷ�
    public float critical;          // ���� ġ��Ÿ��
    public int currentMagazine;     // ���� źâ ��
    public int maxMagazine;         // �ִ� źâ ��
    public int currentAmmo;         // ���� ź�� ��
    public int maxAmmo;             // �ִ� ź�� ��
    public float attackRate;        // ���� �ӵ�
    public float attackDistance;    // ���� ��Ÿ�
    public float bulletSpeed;       // �Ѿ� �ӵ�
    public int bulletPenetration;   // �Ѿ� �����
    public bool isAutomaticAttack;  // ���� ���� ����
    public Sprite weaponSprite;     // ���� �̹���

    //public List<Opctions> 

    public void RandomLevel()
    {
        weaponLevel = Random.Range(0, 9);
    }

    public int GetPrice => (weaponLevel + 1) * 300;

    public int GetUpgradePrice => (weaponLevel + 1) * 400;

    public WeaponBaseData ToData()
    {
        return new WeaponBaseData
        {
            weaponSetting = this
        };
    }

    public void FromJson(string json)
    {
        WeaponBaseData data = JsonUtility.FromJson<WeaponBaseData>(json);
        this = data.weaponSetting;
    }
}