using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift; // 달리기 키

    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; // 점프 키

    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R;  // 탄 재장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                // 걷기 사운드
    [SerializeField]
    private AudioClip audioClipRun;                 // 달리기 사운드

    [SerializeField]
    private RotateToMouse _rotateToMouse;           // 마우스 이동으로 카메라 회전
    private MovementCharacterController _movement;  // 키보드 입력으로 플레이어 이동, 점프
    private Status _status;                         // 이동속도 등의 플레이어 정보
    private AudioSource audioSource;                // 사운드 재생 제어
    private WeaponBase weapon;                      // 모든 무기가 상속받는 기반 클래스

    [SerializeField]
    private PlayerAnimatorController playerAnimatorController;


    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정하고, 현재 위치에 고정시킨다
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _movement = GetComponent<MovementCharacterController>();
        _status = GetComponent<Status>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        _rotateToMouse.UpdateRotate(this.transform,mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 이동중 일 때 (걷기 or 뛰기)
        if( x != 0 || z != 0)
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없다
            if (z > 0) isRun = Input.GetKey(keyCodeRun);
            _movement.MoveSpeed = isRun == true ? _status.RunSpeed : _status.WalkSpeed;
            playerAnimatorController.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // 재생중일 때는 다시 재생하지 않도록 isPlayer으로 체크해서 재생
            if( audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // 제자리에 멈춰있을 때
        else
        {
            _movement.MoveSpeed = 0;
            playerAnimatorController.MoveSpeed = 0;

            // 사운드 재생 멈춤
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        _movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(keyCodeJump))
        {
            _movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }

        if (Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
    }

    public void TakeDamage(float damage)
    {
        _status.DecreaseHP(damage);
        bool isDie = _status.isDie();

        if (isDie)
        {
            Debug.Log("GameOver");
        }
    }

    public void SwitcningWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
        playerAnimatorController.ChangeAnimator(newWeapon.AnimatorController);
    }
}
