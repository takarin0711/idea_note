using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGesture : MonoBehaviour
{
    public bool isMovingDown = false;
    public bool isLeftMove = false;
    public bool isRightMove = false;

    private float sweepRate = 100.0f;

    private float previousCameraAngleD;
    private float previousCameraAngleR;
    private float previousCameraAngleL;

    void Start()
    {
        previousCameraAngleD = CameraAngleFromGround();
        previousCameraAngleR = CameraAngleFromGround();
        previousCameraAngleL = CameraAngleFromGround();
    }

    // Update is called once per frame
    void Update()
    {
        isMovingDown = DetectMovingDown();
        isLeftMove = DetectLeftMove();
        isRightMove = DetectRightMove();
    }

    private bool DetectMovingDown()
    {
        float angle = CameraAngleFromGround();
        float deltaAngle = previousCameraAngleD - angle;
        float rate = deltaAngle / Time.deltaTime;
        previousCameraAngleD = angle;
        return (rate >= sweepRate);
    }

    private bool DetectLeftMove()
    {
        float angle = CameraAngleToLeft();
        float deltaAngle = previousCameraAngleL - angle;
        float rate = deltaAngle / Time.deltaTime;
        previousCameraAngleL = angle;
        return (rate >= sweepRate);
    }

    private bool DetectRightMove()
    {
        float angle = CameraAngleToRight();
        float deltaAngle = previousCameraAngleR - angle;
        float rate = deltaAngle / Time.deltaTime;
        previousCameraAngleR = angle;
        return (rate >= sweepRate);
    }

    private float CameraAngleFromGround()
    {
        return Vector3.Angle(Vector3.down, Camera.main.transform.rotation * Vector3.forward);
    }

    private float CameraAngleToRight()
    {
        return Vector3.Angle(Vector3.right, Camera.main.transform.rotation * Vector3.forward);
    }

    private float CameraAngleToLeft()
    {
        return Vector3.Angle(Vector3.left, Camera.main.transform.rotation * Vector3.forward);
    }
}