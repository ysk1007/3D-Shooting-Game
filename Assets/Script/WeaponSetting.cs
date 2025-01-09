using System;
using UnityEngine;
// ������ ������ ���� ������ �� �������� ����ϴ� �������� ����ü�� ��� �����ϸ�
// ������ �߰�/������ �� ����ü�� �����ϱ� �빮�� �߰�/������ ���� ������ ������
public enum WeaponName { AssaultRifle = 0, SawGun, Pistol ,CombatKnife, HandGrenade}

[Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;   // ���� �̸�
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
    public bool isAutomaticAttack;  // ���� ���� ����
    public Sprite weaponSprite;     // ���� �̹���
}