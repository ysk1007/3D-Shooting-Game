using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴포넌트로 적용하면 해당 컴포넌트도 같이 추가 됨.
[RequireComponent(typeof(CharacterController))]

public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;       // 이동속도
    private Vector3 _moveForce;     // 이동 힘

    [SerializeField]
    private float jumpForce;        // 점프 힘
    [SerializeField]
    private float gravity;          // 중력 계수

    public float MoveSpeed
    {
        set => _moveSpeed = Mathf.Max(0, value);
        get => _moveSpeed;
    }

    private CharacterController _characterController;       // 플레이어 이동 제어 컴포넌트

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 허공에 떠있으면 중력만큼 y축 이동속도 감소
        if(!_characterController.isGrounded)
        {
            _moveForce.y += gravity * Time.deltaTime;
        }

        // 1초당 moveForce 속력으로 이동
        _characterController.Move(_moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // 이동 힘 = 이동방향 * 속도
        _moveForce = new Vector3(direction.x * _moveSpeed, _moveForce.y, direction.z * _moveSpeed);
    }

    public void Jump()
    {
        // 플레이어가 바닥에 있을 때만 점프 가능
        if (_characterController.isGrounded)
        {
            _moveForce.y = jumpForce;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemBase>().Use(gameObject);
        }
    }
}
