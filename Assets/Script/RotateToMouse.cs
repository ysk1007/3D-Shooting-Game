using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float _rotCamXAxisSpeed = 5;    // 카메라 x축 회전속도

    [SerializeField]
    private float _rotCamYAxisSpeed = 5;    // 카메라 y축 회전속도

    private float _limitMinX = -80;      // 카메라 x축 회전 범위 (최소)
    private float _limitMaxX = 50;      // 카메라 x축 회전 범위 (최대)
    private float _eulerAngleX;
    private float _eulerAngleY;

    public void UpdateRotate(Transform player, float mouseX, float mouseY)
    {
        _eulerAngleY += mouseX * _rotCamYAxisSpeed;     // 마우스 좌/우 이동으로 카메라 y축 회전
        _eulerAngleX -= mouseY * _rotCamXAxisSpeed;     // 마우스 위/아래 이동으로 카메라 x축 회전

        // 카메라 x축 회전의 경우 회전 범위를 설정
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
