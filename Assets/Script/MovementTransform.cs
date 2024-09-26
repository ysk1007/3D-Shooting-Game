using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    /// <summary>
    /// �̵� ������ �����Ǹ� �˾Ƽ� �̵��ϵ��� ��
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ܺο��� �Ű������� �̵� ������ ����
    /// </summary>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
