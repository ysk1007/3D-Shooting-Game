using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Main = 0, Sub = 2, Melee, Throw }

[Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("WeaponBase")]
    [SerializeField]
    protected WeaponType weaponType;            // ���� ����
    [SerializeField]
    protected WeaponSetting weaponSetting;        // ���� ����

    protected float lastAttackTime = 0;           // ������ �߻�ð� üũ��
    protected bool isReload = false;              // ������ ������ üũ
    protected bool isAttack = false;              // ���� ���� üũ��
    protected AudioSource audioSource;           // ���� ��� ������Ʈ

    protected MemoryPool memoryPool;            // ���� Ǯ��

    [SerializeField]
    protected PlayerAnimatorController animator;  // �ִϸ��̼� ��� ����
    [SerializeField]
    protected AnimatorOverrideController animatorController; // ���⺰ �ִϸ����� ��Ʈ�ѷ�

    // �ܺο��� �̺�Ʈ �Լ� ����� �� �� �ֵ��� public ����
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

    // �ܺο��� �ʿ��� ������ ���Ա� ���� ������ Get Property's
    public PlayerAnimatorController Animator => animator;
    public AnimatorOverrideController AnimatorController => animatorController;
    public WeaponName WeaponName => weaponSetting.WeaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;
    public WeaponSetting WeaponSetting { get => weaponSetting; set => weaponSetting = value; }

    public MemoryPool MemoryPool => memoryPool;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        if (!audioSource) return;
        audioSource.Stop();         // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü ��
        audioSource.Play();         // ���� ���
    }

    public void Setup(MemoryPool memoryPool)
    {
        this.memoryPool = memoryPool;
        audioSource = GetComponent<AudioSource>();
        animator = PlayerManager.instance.PlayerAnimatorController;
    }

    public virtual void IncreaseMagazine(int magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ?
                                        MaxMagazine : CurrentMagazine + magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }
}
