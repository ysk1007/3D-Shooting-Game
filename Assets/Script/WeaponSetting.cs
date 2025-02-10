using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// 무기의 종류가 여러 종류일 때 공용으로 사용하는 변수들은 구조체로 묶어서 정의하면
// 변수가 추가/삭제될 때 구조체에 선언하기 대문에 추가/삭제에 대한 관리가 용이함

public class Opctions
{

}

public enum WeaponName { AssaultRifle = 0, SawGun, Shotgun, Minigun, ChemicalGun, FlameThrower, /*CombatKnife, HandGrenade,*/ Pistol }

[Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;   // 무기 이름
    public int weaponLevel;         // 무기 레벨
    public string weaponName;       // 무기 이름
    public float damage;            // 무기 공격력
    public float critical;          // 무기 치명타율
    public int currentMagazine;     // 현재 탄창 수
    public int maxMagazine;         // 최대 탄창 수
    public int currentAmmo;         // 현재 탄약 수
    public int maxAmmo;             // 최대 탄약 수
    public float attackRate;        // 공격 속도
    public float attackDistance;    // 공격 사거리
    public float bulletSpeed;       // 총알 속도
    public int bulletPenetration;   // 총알 관통력
    public bool isAutomaticAttack;  // 연속 공격 여부
    public Sprite weaponSprite;     // 무기 이미지

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