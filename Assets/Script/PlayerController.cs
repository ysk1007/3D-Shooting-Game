using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift; // �޸��� Ű

    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; // ���� Ű

    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R;  // ź ������ Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                // �ȱ� ����
    [SerializeField]
    private AudioClip audioClipRun;                 // �޸��� ����

    [SerializeField]
    private RotateToMouse _rotateToMouse;           // ���콺 �̵����� ī�޶� ȸ��
    private MovementCharacterController _movement;  // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status _status;                         // �̵��ӵ� ���� �÷��̾� ����
    private AudioSource audioSource;                // ���� ��� ����
    private WeaponBase weapon;                      // ��� ���Ⱑ ��ӹ޴� ��� Ŭ����

    [SerializeField]
    private PlayerAnimatorController playerAnimatorController;


    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� �����ϰ�, ���� ��ġ�� ������Ų��
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

        // �̵��� �� �� (�ȱ� or �ٱ�)
        if( x != 0 || z != 0)
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����
            if (z > 0) isRun = Input.GetKey(keyCodeRun);
            _movement.MoveSpeed = isRun == true ? _status.RunSpeed : _status.WalkSpeed;
            playerAnimatorController.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // ������� ���� �ٽ� ������� �ʵ��� isPlayer���� üũ�ؼ� ���
            if( audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // ���ڸ��� �������� ��
        else
        {
            _movement.MoveSpeed = 0;
            playerAnimatorController.MoveSpeed = 0;

            // ���� ��� ����
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
        bool isDie = _status.DecreaseHP(damage);

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
