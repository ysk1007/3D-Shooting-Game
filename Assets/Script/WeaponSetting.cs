using System;
// ������ ������ ���� ������ �� �������� ����ϴ� �������� ����ü�� ��� �����ϸ�
// ������ �߰�/������ �� ����ü�� �����ϱ� �빮�� �߰�/������ ���� ������ ������
[Serializable]
public struct WeaponSetting
{
    public float attackRate;        // ���� �ӵ�
    public float attackDistance;    // ���� ��Ÿ�
    public bool isAutomaticAttack;  // ���� ���� ����
}