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
    protected WeaponType weaponType;            // 무기 종류
    [SerializeField]
    protected WeaponSetting weaponSetting;        // 무기 설정

    protected float lastAttackTime = 0;           // 마지막 발사시간 체크용
    protected bool isReload = false;              // 재장전 중인지 체크
    protected bool isAttack = false;              // 공격 여부 체크용
    protected AudioSource audioSource;           // 사운드 재생 컴포넌트

    protected MemoryPool memoryPool;            // 무기 풀링

    [SerializeField]
    protected PlayerAnimatorController animator;  // 애니메이션 재생 제어
    [SerializeField]
    protected AnimatorOverrideController animatorController; // 무기별 애니메이터 컨트롤러

    // 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

    // 외부에서 필요한 정보를 열함기 위해 정의한 Get Property's
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
        audioSource.Stop();         // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체 후
        audioSource.Play();         // 사운드 재생
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
