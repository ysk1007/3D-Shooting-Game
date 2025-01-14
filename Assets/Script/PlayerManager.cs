using Cinemachine;
using Photon.Pun;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviourPun
{
    public static PlayerManager instance;
    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator anim;

    [Header("Aim")]
    [SerializeField]
    private CinemachineVirtualCamera aimCam;
    [SerializeField]
    private GameObject aimImage;
    [SerializeField]
    private GameObject aimObj;
    [SerializeField]
    private float aimObjDis = 10f;
    [SerializeField]
    private LayerMask targetLayer;
    private Vector3 targetPosition;
    [SerializeField]
    private Transform playerCameraRoot;
    Vector3 defPos = new Vector3(0, 1.7f, 0);
    Vector3 aimPos = new Vector3(1, 1.7f, 0);

    [Header("IK")]
    [SerializeField]
    private Rig handRig;
    [SerializeField]
    private Rig aimRig;

    [SerializeField]
    private Animator weaponAnimatorController;      // 무기 별 애니메이터
    [SerializeField]
    private WeaponInfoPopup weaponInfoPopup;

    [SerializeField]
    private PlayerAnimatorController playerAnimatorController; // 플레이어 애니메이터 컨트롤러
    private Status status;                         // 이동속도 등의 플레이어 정보
    private WeaponBase weapon;                      // 모든 무기가 상속받는 기반 클래스
    [SerializeField]
    private WeaponGrenade weaponGrenade;

    public Transform AimObj => aimObj.transform;

    public Vector3 TargetPosition => targetPosition;

    public PlayerAnimatorController PlayerAnimatorController => playerAnimatorController;
    public WeaponInfoPopup WeaponInfoPopup => weaponInfoPopup;
    private void Awake()
    {
        instance = this;
        GameManager.instance.enemyMemoryPool.Targets.Add(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<Status>();
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AimCheck();
    }

    private void AimCheck()
    {
        if (input.reroad)
        {
            StartReroad();
        }

        if (controller.isReroad)
        {
            return;
        }

        if (input.aim)
        {
            playerCameraRoot.localPosition = aimPos;
            AimControll(true);

            anim.SetLayerWeight(1, 1);

            targetPosition = Vector3.zero;
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 50f, targetLayer))
            {
                targetPosition = hit.point;
                aimObj.transform.position = hit.point;
            }
            else
            {
                targetPosition = camTransform.position + camTransform.forward * aimObjDis;
                aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDis;
            }

            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward,aimDir,Time.deltaTime * 50f);

            SetRigWeight(1);

            if (input.shoot)
            {
                anim.SetBool("Shoot", true);
                weapon.StartWeaponAction();
            }
            else
            {
                anim.SetBool("Shoot", false);
                weapon.StopWeaponAction();
            }
        }
        else
        {
            playerCameraRoot.localPosition = defPos;
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    public void StartReroad()
    {
        input.reroad = false;

        if (controller.isReroad || weapon.CurrentMagazine <= 0)
        {
            return;
        }

        AimControll(false);
        SetRigWeight(0);
        anim.SetLayerWeight(1, 1);
        anim.SetTrigger("Reroad");
        weapon.StartReload();
        controller.isReroad = true;
    }

    public void Reroad()
    {
        Debug.Log("리로딩");
        controller.isReroad = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
    }

    private void SetRigWeight(float weight)
    {
        aimRig.weight = weight;
        handRig.weight = weight;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        status.DecreaseHP(damage);
        bool isDie = status.isDie();

        if (isDie)
        {
            Debug.Log("GameOver");
        }
    }

    public void SwitcningWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
        weaponAnimatorController.runtimeAnimatorController = weapon.AnimatorController;
    }

    public void SpawnGrenadeProjectile()
    {
        weaponGrenade.SpawnGrenadeProjectile();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemBase>().Use(gameObject);
        }
    }
}
