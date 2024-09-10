using System;
// ������ ������ ���� ������ �� �������� ����ϴ� �������� ����ü�� ��� �����ϸ�
// ������ �߰�/������ �� ����ü�� �����ϱ� �빮�� �߰�/������ ���� ������ ������

public enum WeaponName { AssaultRifle = 0 }

[Serializable]
public struct WeaponSetting
{
    public WeaponName WeaponName;   // ���� �̸�
    public int currentAmmo;         // ���� ź�� ��
    public int maxAmmo;             // �ִ� ź�� ��
    public float attackRate;        // ���� �ӵ�
    public float attackDistance;    // ���� ��Ÿ�
    public bool isAutomaticAttack;  // ���� ���� ����
}