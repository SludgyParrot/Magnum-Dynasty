using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RaceUI))]
[RequireComponent(typeof(Rigidbody))]
public sealed class Vehicle : MonoBehaviour
{
    [SerializeField]
    private float torque;

    [SerializeField, Space(5)]
    private float brakeTorque;

    [SerializeField, Space(5)]
    private float maxSpeed;

    [SerializeField, Space(5)]
    private DriveType driveType;

    [SerializeField, Space(5)]
    private AnimationCurve steering;

    [SerializeField, Space(5)]
    private float sensorDetectionSteerSensitivity = 0.5f;

    [field: SerializeField, Space(5)]
    public bool IsPlayer {  get; private set; }

    [SerializeField, Space(5)]
    private InputActionAsset inputAction;

    [SerializeField, Space(5)]
    private InputActionReference accelerationInput, 
        brakeInput,
        steerInput, 
        heahLightsInput;

    [SerializeField, Space(5)]
    private List<Wheel> wheels = new List<Wheel>();

    [SerializeField, Space(5)]
    private PathFinder pathFinder;

    [SerializeField, Space(5)]
    private float pathReachDistanceThreshold = 5.0f;

    [SerializeField, Space(5)]
    private float health;

    [SerializeField, Space(5)]
    private UnityEvent onMachineGunAppliedEvent;

    [SerializeField, Space(5)]
    private UnityEvent<int> onMachineGunAmmoAppliedEvent;

    [SerializeField, Space(5)]
    private UnityEvent<float> onHealthEvent;

    [SerializeField, Space(5)]
    private UnityEvent<VehicleLightType, bool> onVehicleLightEvent;

    private RaceUI raceUI;

    private float motorTorque = 0.0f;
    private float steeringInput = 0.0f;

    private const int DIRECTION = 1;

    private Rigidbody carPhysicsBody;

    public Rigidbody CarPhysicsBody => carPhysicsBody;

    [field: SerializeField]
    public int CurrentLap {  get; private set; }

    [SerializeField]
    private float speedKPH;

    private float currentSpeed = 0.0f;

    public float CurrentSpeed => currentSpeed;

    private float accelerationInputValue = 0;

    private const float SPEED_MULTIPLIER = 3.6f;

    private float currentHealth;

    [SerializeField]
    private int currentPathIndex;

    [SerializeField]
    float distance;

    private Vector3 steerVector;

    [SerializeField]
    private bool isBraking = false;

    public int PathIndex => currentPathIndex;
    public float DistanceToWaypoint => distance;

    private bool lightsOn;

    private int sensorSteerDetectionflag;
    private float sensorSteerSensitivity;

    private void Start()
    {
        if (IsPlayer)
            InitializeInputs();

        if(pathFinder == null)
            pathFinder = FindObjectOfType<PathFinder>();

        carPhysicsBody = GetComponent<Rigidbody>();

        raceUI = GetComponent<RaceUI>();

        CurrentLap = 1;

        currentHealth = health;
        onHealthEvent?.Invoke(currentHealth);
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

        if(heahLightsInput  != null)
        {
            heahLightsInput.action.performed += context => ToggleHeadLights(context);
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

        onVehicleLightEvent?.Invoke(VehicleLightType.Brake, performed);
    }

    private void Steer(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        var input = obj.ReadValue<Vector2>();
        steeringInput = input.x;
    }

    private void ToggleHeadLights(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        lightsOn = !lightsOn;
        onVehicleLightEvent?.Invoke(VehicleLightType.Head, lightsOn);
    }

    private void Update()
    {
        if (currentHealth <= 0) return;

        if (IsPlayer)
        {
            if(speedKPH < maxSpeed)
                motorTorque = torque * accelerationInputValue;
            else
                motorTorque = 0.0f;
        }
        else
        {
            if (sensorSteerDetectionflag != 0) return;

            if (!isBraking)
            {
                if (speedKPH < maxSpeed)
                    motorTorque = torque;
                else
                    motorTorque = 0.0f;
            }
        }

        GetNextPathIndex();
        UpdateRaceUI();
    }

    private void FixedUpdate()
    {
        if (wheels.Count == 0) return;

        currentSpeed = CarPhysicsBody.velocity.magnitude;
        speedKPH = currentSpeed * SPEED_MULTIPLIER;

        foreach (Wheel wheel in wheels)
        {
            if(driveType == DriveType.FWD && wheel.Steering)
            {
                if(!isBraking)
                    wheel.AddTorque(motorTorque);
                else
                    wheel.AddBrakeTorque(brakeTorque);
            }
            else if(driveType == DriveType.RWD && !wheel.Steering)
            {
                if (!isBraking)
                    wheel.AddTorque(motorTorque);
                else
                    wheel.AddBrakeTorque(brakeTorque);
            }
            else
            {
                if (!isBraking)
                    wheel.AddTorque(motorTorque);
                else
                    wheel.AddBrakeTorque(brakeTorque);
            }

            if (IsPlayer)
                wheel.Steer(GetSteerAngle());
            else
            {
                if(sensorSteerDetectionflag == 0)
                    wheel.Steer(GetPathSteerAngle());
                else
                    wheel.Steer(GetSensorDetectionSteerAngle());
            }

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

    private float GetSensorDetectionSteerAngle()
    {
        var steerAngle = 1 * steering.Evaluate(CurrentSpeed);
        return steerAngle * sensorSteerSensitivity;
    }

    private void GetNextPathIndex()
    {

        distance = (transform.position - pathFinder.Paths[currentPathIndex].position).magnitude;

        if(distance <= pathReachDistanceThreshold)
        {
            if (currentPathIndex < pathFinder.Paths.Count - 1)
                currentPathIndex++;
            else
                currentPathIndex = 0;
        }
    }

    private void UpdateRaceUI()
    {
        int racePosition = RaceManager.Instance.GetRacePosition(this);
        raceUI.DisplayRacePosition(racePosition);
    }

    private float GetSteerAngle()
    {
        float angle = steeringInput * steering.Evaluate(CurrentSpeed);
        return angle;
    }

    public Vector3 GetMovingDirection()
        => transform.InverseTransformDirection(CarPhysicsBody.velocity);

    public void ApplyAcceleration()
    {
        isBraking = false;
    }

    public void ApplyBrakes()
    {
        isBraking = true;
    }

    public void IncrementLaps()
    {
        if(CurrentLap < RaceManager.Instance.TotalLaps)
            CurrentLap++;
    }

    public void ApplyMisteryBox(MisteryBoxType type, int value)
    {
        switch(type)
        {
            case MisteryBoxType.MachineGun:
                onMachineGunAppliedEvent?.Invoke();
                break;
            case MisteryBoxType.Ammo:
                onMachineGunAmmoAppliedEvent?.Invoke(value);
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth > 0.0f)
            currentHealth -= damage;
        else
        {
            currentHealth = 0.0f;
            RaceManager.Instance.Vehicles.Remove(this);
        }

        float healthValue = currentHealth / 100.0f;
        onHealthEvent?.Invoke(healthValue);

    }

    public void OnSensorDetection(SensorType sensor)
    {
        if(IsPlayer) return;

        sensorSteerDetectionflag = 0;

        switch (sensor)
        {
            case SensorType.LeftSideSensor:
                sensorSteerSensitivity += sensorDetectionSteerSensitivity;
                sensorSteerDetectionflag++;
                break;
            case SensorType.RightSideSensor:
                sensorSteerSensitivity -= sensorDetectionSteerSensitivity;
                sensorSteerDetectionflag++;
                break;
            default:
                sensorSteerSensitivity = 0.0f;
                break;
        }

        Debug.Log($"~On sensor detected: {sensor} and send to vehicle with flag: {sensorSteerDetectionflag} and sensitivity: {sensorSteerSensitivity}.");
    }
}
