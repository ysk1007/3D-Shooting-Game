using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMinigun : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect;       // 총구 이펙트 (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;         // 탄피 생성 위치
    [SerializeField]
    private Transform bulletSpawnPoint;         // 총알 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;  // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire;            // 공격 사운드
    [SerializeField]
    private AudioClip audioClipReload;          // 재장전 사운드

    private Camera mainCamera;                  // 광선 발사

    private void Awake()
    {

        mainCamera = Camera.main;

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
        if (PlayerManager != null)
            PlayerManager.gameObject.GetComponent<PlayerHUD>().WeaponAddListener(this);
    }


    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        //muzzleFlashEffect.SetActive(false);

        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신하다
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신하다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    public override void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없음
        if (isReload == true) return;

        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // 무기 액션 도중에 'R' 키를 눌러 재장전을 시도하여 무기 액션 종료 후 재장전
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
            // 뛰고있을 때는 공격할 수 없다.
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;

            // 탄 수가 없으면 공격 불가능
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            // 공격시 탄환 -1, Ui 업데이트
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // 무기 애니메이션 재생
            animator.Play("Fire", -1, 0);

            // 총구 이펙트 재생
            //StartCoroutine("OnMuzzleFlashEffect");

            // 공격 사운드 재생
            PlaySound(audioClipFire);

            // 탄피 생성
            //casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // 광선을 발사해 원하는 위치 공격 (+Impact Effect)
            //TwoStepRayCast();
            BulletMemoryPool.instance.SpawnBullet(playerManager, weaponSetting, bulletSpawnPoint.position, bulletSpawnPoint.rotation, playerManager.TargetPosition);
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

        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);

        while (true)
        {
            // 사운드가 재생중이 아니고, 현재 애니메이션이 Idle Walk Run Blend 이면
            // 재장전 애니메이션(, 사운드) 재생이 종료되었다는 뜻
            if (audioSource.isPlaying == false && !animator.CurrentAnimationIs("Reloading"))
            {
                isReload = false;

                // 현재 탄창 수를 1 감소 시키고, 바뀐 탄창 정보를 Text Ui에 업데이트
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text Ui에 업데이트
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

        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // 첫번째 Raycast연산으로 얻어진 targetpoint를 목표지점으로 설정하고,
        // 총구를 시작지점으로 하여 Raycast 연산
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            //impactMemoryPool.SpawnImpact(hit);
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);

        bulletMemoryPool.SpawnBullet(weaponSetting.WeaponName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }*/
}
