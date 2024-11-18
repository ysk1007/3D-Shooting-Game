using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private WeaponKnife weaponKnife;

    [SerializeField]
    private WeaponGrenade weaponGrenade;

    public Animator Animator { get { return animator; } }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    // 애니메이션이 현재 재생중인지 반환
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void ChangeAnimator(AnimatorOverrideController newAnimator)
    {
        animator.runtimeAnimatorController = newAnimator;
    }

    public void StartWeaponKnfireCollider()
    {
        weaponKnife.StartWeaponKnfireCollider();
    }

    public void SpawnGrenadeProjectile()
    {
        weaponGrenade.SpawnGrenadeProjectile();
    }
}
