using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : WeaponBase
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
            PlayerManager.gameObject.GetComponent<PlayerHUD>().WeaponAddListener(this);
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
            //StartCoroutine("OnMuzzleFlashEffect");

            // ���� ���� ���
            PlaySound(audioClipFire);

            // ź�� ����
            //casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            //TwoStepRayCast();
            for (int i = 0; i < 15; i++)
            {
                Vector3 vc = playerManager.TargetPosition + new Vector3(Random.Range(-0.8f,0.8f), Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f));
                BulletMemoryPool.instance.SpawnBullet(playerManager, weaponSetting, bulletSpawnPoint.position, bulletSpawnPoint.rotation, vc);
            }
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
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

    /*private void TwoStepRayCast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ (Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // ���� ��Ÿ�(attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // ù��° Raycast�������� ����� targetpoint�� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            //impactMemoryPool.SpawnImpact(hit);
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);

        bulletMemoryPool.SpawnBullet(weaponSetting.WeaponName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }*/
}
