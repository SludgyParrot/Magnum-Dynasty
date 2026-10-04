using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class Vehicle : MonoBehaviour
{
    [SerializeField]
    private float torque;

    [SerializeField, Space(5)]
    private float maxSpeed;

    [SerializeField, Space(5)]
    private DriveType driveType;

    [SerializeField, Space(5)]
    private AnimationCurve steering;

    [SerializeField, Space(5)]
    private bool isPlayer;

    [SerializeField, Space(5)]
    private InputActionAsset inputAction;

    [SerializeField, Space(5)]
    private InputActionReference accelerationInput, 
        brakeInput,
        steerInput;

    [SerializeField, Space(5)]
    private List<Wheel> wheels = new List<Wheel>();

    [SerializeField, Space(5)]
    private PathFinder pathFinder;

    private float motorTorque = 0.0f;
    private float steeringInput = 0.0f;

    private const int DIRECTION = 1;

    private Rigidbody carPhysicsBody;

    public Rigidbody CarPhysicsBody => carPhysicsBody;

    [SerializeField]
    private float speedKPH;

    private float currentSpeed = 0.0f;

    public float CurrentSpeed => currentSpeed;

    private float accelerationInputValue = 0;

    private const float SPEED_MULTIPLIER = 3.6f;

    private int currentPathIndex;

    private Vector3 steerVector;

    private void Start()
    {
        if(isPlayer)
            InitializeInputs();

        if(!isPlayer && pathFinder == null)
            throw new ArgumentNullException(nameof(pathFinder), "Cannot be null.");

        carPhysicsBody = GetComponent<Rigidbody>();
    }

    private void InitializeInputs()
    {
        if (inputAction == null) return;
        inputAction.Enable();

        if (accelerationInput != null)
        {
            accelerationInput.action.performed += context => Accelerate(context);
            accelerationInput.action.canceled += context => Accelerate(context, false);
        }

        if (brakeInput != null)
        {
            brakeInput.action.performed += context => Brake(context);
            brakeInput.action.canceled += context => Brake(context, false);
        }

        if (steerInput != null)
        {
            steerInput.action.performed += context => Steer(context);
            steerInput.action.canceled += context => Steer(context, false);
        }
    }

    private void Accelerate(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        if (performed)
            accelerationInputValue = 1.0f;
        else
            accelerationInputValue = 0.0f;
    }

    private void Brake(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        if (performed)
            accelerationInputValue = -1.0f;
        else
            accelerationInputValue = 0.0f;
    }

    private void Steer(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        var input = obj.ReadValue<Vector2>();
        steeringInput = input.x;
    }

    private void Update()
    {
        if (isPlayer)
        {
            if(speedKPH < maxSpeed)
                motorTorque = torque * accelerationInputValue;
            else
                motorTorque = 0.0f;
        }
        else
        {
            if (speedKPH < maxSpeed)
                motorTorque = torque;
            else
                motorTorque = 0.0f;

            GetNextPathIndex();
        }
    }

    private void FixedUpdate()
    {
        if (wheels.Count == 0) return;

        currentSpeed = CarPhysicsBody.velocity.magnitude;
        speedKPH = currentSpeed * SPEED_MULTIPLIER;

        foreach (Wheel wheel in wheels)
        {
            if(driveType == DriveType.FWD && wheel.Steering)
                wheel.AddTorque(motorTorque);
            else if(driveType == DriveType.RWD && !wheel.Steering)
                wheel.AddTorque(motorTorque);
            else
                wheel.AddTorque(motorTorque);

            if(isPlayer)
                wheel.Steer(GetSteerAngle());
            else
                wheel.Steer(GetPathSteerAngle());

            wheel.UpdateWheel();
        }
    }

    private float GetPathSteerAngle()
    {
        steerVector.x = pathFinder.Paths[currentPathIndex].position.x;
        steerVector.y = transform.position.y;
        steerVector.z = pathFinder.Paths[currentPathIndex].position.z;

        var inversedSteerVector = transform.InverseTransformPoint(steerVector);

        var steerAngle = 1 * steering.Evaluate(CurrentSpeed);
        var newSteerAngle = steerAngle * (inversedSteerVector.x / inversedSteerVector.magnitude);
        return newSteerAngle;
    }

    private void GetNextPathIndex()
    {
        float distance = (transform.position - pathFinder.Paths[currentPathIndex].position).magnitude;

        if(distance <= 0.5f)
        {
            currentPathIndex++;

            if (currentPathIndex >= pathFinder.Paths.Count)
                currentPathIndex = 0;
        }
    }

    private float GetSteerAngle()
    {
        float angle = steeringInput * steering.Evaluate(CurrentSpeed);
        return angle;
    }

    public Vector3 GetMovingDirection()
        => transform.InverseTransformDirection(CarPhysicsBody.velocity);
}
