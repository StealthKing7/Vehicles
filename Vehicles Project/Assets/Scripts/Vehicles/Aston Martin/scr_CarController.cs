using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
public class scr_CarController : MonoBehaviour
{
    CarInput carInput;
    Vector2 Movement;
    float currentAcceleration;
    float currentSteerAngle;
    bool isBreaking;
    [SerializeField] CarSettings carSettings;
    [SerializeField] List<WheelCollider> wheelColliders;
    [SerializeField] List<Transform> wheelTransforms;


    private void Awake()
    {
        carInput = new CarInput();
        carInput.Car.Movement.performed += e => Movement = e.ReadValue<Vector2>();
        carInput.Car.BreakPressed.performed += e => isBreaking = true;
        carInput.Car.BreakReleased.performed += e => isBreaking = false;
        carInput.Enable();

    }
    private void Start()
    {
        if (carSettings.AutoDrive)
        {
            currentAcceleration = carSettings.Acceleration * 1;
        }
    }
    private void FixedUpdate()
    {
        HandleMotor();
        Steer();

    }
    void HandleMotor()
    {
        if (!carSettings.AutoDrive)
        {
            currentAcceleration = carSettings.Acceleration * Movement.y;
        }
        if (isBreaking)
        {
            wheelColliders.ForEach(x => x.brakeTorque = carSettings.BrakingForce);
        }
        else
        {
            wheelColliders.ForEach(x => x.brakeTorque = 0);
        }
        if (carSettings.FourWheelDrive)
        {
            wheelColliders.ForEach(x => x.motorTorque = currentAcceleration);
        }
        else
        {
            wheelColliders[0].motorTorque = currentAcceleration;
            wheelColliders[1].motorTorque = currentAcceleration;
        }

    }
    void Steer()
    {
        currentSteerAngle = carSettings.MaxSteerAngle * Movement.x;
        wheelColliders[0].steerAngle = currentSteerAngle;
        wheelColliders[1].steerAngle = currentSteerAngle;
        for(int i = 0; i < wheelColliders.Count; i++)
        {
            for(int j = 0; j < wheelTransforms.Count; j++)
            {
                TurnWheels(wheelColliders[i], wheelTransforms[j]);
            }
        }
    }
    void TurnWheels(WheelCollider col,Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }
}
