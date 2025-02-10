using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFlameThrower : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect;       // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;         // ź�� ���� ��ġ
    [SerializeField]
    private Transform bulletSpawnPoint;         // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;  // ���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire;            // ���� ����
    [SerializeField]
    private AudioClip audioClipReload;          // ������ ����

    private Camera mainCamera;                  // ���� �߻�

    private void Awake()
    {

        mainCamera = Camera.main;

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
        if (PlayerManager != null)
        {
            PlayerManager.gameObject.GetComponent<PlayerHUD>().WeaponAddListener(this);
        }
    }


    private void OnEnable()
    {
        // ���� ���� ���� ���
        PlaySound(audioClipTakeOutWeapon);
        //muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����ϴ�
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����ϴ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    public override void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����
        if (isReload == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // �ܹ� ����
            else
            {
                OnAttack();
            }
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        muzzleFlashEffect.SetActive(false);
        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // ���� �׼� ���߿� 'R' Ű�� ���� �������� �õ��Ͽ� ���� �׼� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �ٰ����� ���� ������ �� ����.
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ���
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            // ���ݽ� źȯ -1, Ui ������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0);

            // �ѱ� ����Ʈ ���
            //muzzleFlashEffect.SetActive(true);
            // ���� ���� ���
            PlaySound(audioClipFire);

            // ź�� ����
            //casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            //TwoStepRayCast();
            BulletMemoryPool.instance.SpawnBullet(playerManager, weaponSetting, bulletSpawnPoint.position, bulletSpawnPoint.rotation, playerManager.TargetPosition);
        }
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        // ������ �ִϸ��̼�, ���� ���
        animator.OnReload();
        PlaySound(audioClipReload);
        while (true)
        {
            // ���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� Idle Walk Run Blend �̸�
            // ������ �ִϸ��̼�(, ����) ����� ����Ǿ��ٴ� ��
            if (audioSource.isPlaying == false && !animator.CurrentAnimationIs("Reloading"))
            {
                isReload = false;

                // ���� źâ ���� 1 ���� ��Ű��, �ٲ� źâ ������ Text Ui�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź ���� �ִ�� �����ϰ�, �ٲ� ź �� ������ Text Ui�� ������Ʈ
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                //PlayerManager.instance.Reroad();
                yield break;
            }

            yield return null;
        }
    }
}
