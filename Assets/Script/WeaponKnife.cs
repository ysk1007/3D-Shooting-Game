using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    private WeaponKnifeCollider weaponKnifeCollider;

    private void OnEnable()
    {
        isAttack = false;

        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신하다
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신하다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    private void Awake()
    {
        // 처음 탄창 수는 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void Start()
    {
        //base.Setup(GunMemoryPool.instance.GunPool[(int)WeaponName]);
    }

    [PunRPC]
    public override void Setup(int callerViewID)
    {

        PhotonView callerView = PhotonView.Find(callerViewID);
        this.memoryPool = callerView.GetComponent<GunMemoryPool>();

        audioSource = GetComponent<AudioSource>();
        animator = PlayerManager.instance.PlayerAnimatorController;
        PlayerHUD.instance.WeaponAddListener(this);
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (isAttack == true) return;

        // 연속 공격
        if (weaponSetting.isAutomaticAttack == true)
        {
            StartCoroutine("OnAttackLoop");
        }
        // 단일 공격
        else
        {
            StartCoroutine("OnAttack");
        }
    }
    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
    }
    public override void StartReload()
    {
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            yield return StartCoroutine("OnAttack");
        }
    }

    private IEnumerator OnAttack()
    {
        isAttack = true;

        // 공격 애니메이션 재생
        animator.Play("Fire", -1, 0);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;

                yield break;
            }

            yield return null;
        }
    }

    public void StartWeaponKnfireCollider()
    {
        weaponKnifeCollider.StartCollider(weaponSetting.damage);
    }
}
