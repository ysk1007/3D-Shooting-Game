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

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����ϴ�
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����ϴ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    private void Awake()
    {
        // ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // ó�� ź ���� �ִ�� ����
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

        // ���� ����
        if (weaponSetting.isAutomaticAttack == true)
        {
            StartCoroutine("OnAttackLoop");
        }
        // ���� ����
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

        // ���� �ִϸ��̼� ���
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
