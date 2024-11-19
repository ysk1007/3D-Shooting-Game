using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float _rotCamXAxisSpeed = 5;    // ī�޶� x�� ȸ���ӵ�

    [SerializeField]
    private float _rotCamYAxisSpeed = 5;    // ī�޶� y�� ȸ���ӵ�

    private float _limitMinX = -80;      // ī�޶� x�� ȸ�� ���� (�ּ�)
    private float _limitMaxX = 50;      // ī�޶� x�� ȸ�� ���� (�ִ�)
    private float _eulerAngleX;
    private float _eulerAngleY;

    public void UpdateRotate(Transform player, float mouseX, float mouseY)
    {
        _eulerAngleY += mouseX * _rotCamYAxisSpeed;     // ���콺 ��/�� �̵����� ī�޶� y�� ȸ��
        _eulerAngleX -= mouseY * _rotCamXAxisSpeed;     // ���콺 ��/�Ʒ� �̵����� ī�޶� x�� ȸ��

        // ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
        _eulerAngleX = ClampAngle(_eulerAngleX, _limitMinX, _limitMaxX);

        transform.rotation = Quaternion.Euler(_eulerAngleX, _eulerAngleY, 0);
        player.rotation = Quaternion.Euler(0, _eulerAngleY, 0);
    }

    private float ClampAngle(float angle,float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
