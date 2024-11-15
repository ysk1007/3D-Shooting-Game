using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������Ʈ�� �����ϸ� �ش� ������Ʈ�� ���� �߰� ��.
[RequireComponent(typeof(CharacterController))]

public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;       // �̵��ӵ�
    private Vector3 _moveForce;     // �̵� ��

    [SerializeField]
    private float jumpForce;        // ���� ��
    [SerializeField]
    private float gravity;          // �߷� ���

    public float MoveSpeed
    {
        set => _moveSpeed = Mathf.Max(0, value);
        get => _moveSpeed;
    }

    private CharacterController _characterController;       // �÷��̾� �̵� ���� ������Ʈ

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ����� �������� �߷¸�ŭ y�� �̵��ӵ� ����
        if(!_characterController.isGrounded)
        {
            _moveForce.y += gravity * Time.deltaTime;
        }

        // 1�ʴ� moveForce �ӷ����� �̵�
        _characterController.Move(_moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        // �̵� ���� = ĳ������ ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // �̵� �� = �̵����� * �ӵ�
        _moveForce = new Vector3(direction.x * _moveSpeed, _moveForce.y, direction.z * _moveSpeed);
    }

    public void Jump()
    {
        // �÷��̾ �ٴڿ� ���� ���� ���� ����
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
