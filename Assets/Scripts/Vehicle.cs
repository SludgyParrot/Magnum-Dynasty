using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public sealed class Gear
{
    [SerializeField]
    private string name;

    [SerializeField, Space(5)]
    private float gearSpeed;

    public float GearSpeed => gearSpeed;
}

[RequireComponent(typeof(RaceUI))]
[RequireComponent(typeof(AudioSource))]
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
    private Gear[] gears;

    [SerializeField, Space(5)]
    private DriveType driveType;

    [SerializeField, Space(5)]
    private AnimationCurve steering;

    [SerializeField, Space(5)]
    private float sensorDetectionSteerSensitivity = 0.5f;

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

    private UnityEvent<string, float> onSpeedOMeterValueChangedEvent;

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

    public float SpeedKPH => speedKPH;
    public float MaxSpeed => maxSpeed;

    private float currentSpeed = 0.0f;

    public float CurrentSpeed => currentSpeed;

    private float accelerationInputValue = 0;

    private const float SPEED_MULTIPLIER = 3.6f;

    private float currentHealth;

    private bool inputInitialized;

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

    private bool avoidingObstacle;
    private float sensorSteerAngle;

    [SerializeField, Space(5)]
    private float reverseDuration;

    [SerializeField]
    private float remainingReverseTime;

    private AudioSource engineSoundPlayer;

    [SerializeField]
    private List<EventConfig> initializeInputs =
        new List<EventConfig>();

    public UnityEvent<ScreenUIEventConfig> OnMisteryBoxAppliedEvent { get; private set; } = 
        new UnityEvent<ScreenUIEventConfig>();

    private void Start()
    {
        if(pathFinder == null)
            pathFinder = FindObjectOfType<PathFinder>();

        engineSoundPlayer = GetComponent<AudioSource>();

        carPhysicsBody = GetComponent<Rigidbody>();

        raceUI = GetComponent<RaceUI>();

        CurrentLap = 1;

        currentHealth = health;
        onHealthEvent?.Invoke(currentHealth);
    }

    private void Update()
    {
        if (currentHealth <= 0) return;

        if (inputInitialized)
        {
            if(speedKPH < maxSpeed)
                motorTorque = torque * accelerationInputValue;
            else
                motorTorque = 0.0f;
        }
        else
        {
            if (avoidingObstacle) return;

            if (!isBraking)
            {
                if(remainingReverseTime <= 0.0f)
                {
                    if (speedKPH < maxSpeed)
                        motorTorque = torque;
                    else
                        motorTorque = 0.0f;
                }
                else
                {
                    motorTorque = -torque;
                    remainingReverseTime -= 1 * Time.deltaTime;
                }
            }
        }

        PlayEngineSound();
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

            if (inputInitialized)
                wheel.Steer(GetSteerAngle());
            else
            {
                if (!avoidingObstacle)
                    wheel.Steer(remainingReverseTime > 0.0f ? -GetPathSteerAngle() : GetPathSteerAngle());
                else
                    wheel.Steer(remainingReverseTime > 0.0f? -sensorSteerAngle: sensorSteerAngle);
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

    private void GetNextPathIndex()
    {

        distance = (transform.position - pathFinder.Paths[currentPathIndex].position).magnitude;

        if(distance <= pathReachDistanceThreshold)
        {
            if (currentPathIndex < pathFinder.Paths.Count - 1)
                currentPathIndex++;
            else
            {
                currentPathIndex = 0;

                if (CurrentLap < RaceManager.Instance.TotalLaps)
                    CurrentLap++;
                else
                {
                    if(inputInitialized)
                    {
                        RaceManager.Instance.CompleteRace();
                        inputInitialized = false;
                    }
                }
            }

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

    private void PlayEngineSound()
    {
        if (gears.Length == 0) return;

        for (int i = 0; i < gears.Length; i++)
        {
            if (gears[i].GearSpeed > speedKPH) break;

            float minGearSpeed = 0.0f;
            float maxGearSpeed = 0.0f;

            if (i == 0)
                minGearSpeed = 0.0f;
            else
                minGearSpeed = gears[i - 1].GearSpeed;

            maxGearSpeed = gears[i].GearSpeed;

            float engineRPM = (currentSpeed - minGearSpeed / (maxGearSpeed - minGearSpeed)) / maxSpeed;
            engineSoundPlayer.pitch = engineRPM + 1;
        }
    }

    public void InitializeInputs(List<EventConfig> inputs)
    {
        if(inputs == null || inputs.Count == 0) return;
        initializeInputs = inputs;
        inputInitialized = true;
    }

    //public void InitializeInputs(bool initialize)
    //{
    //    if (inputAction == null) return;

    //    if (initialize)
    //    {
    //        inputAction.Enable();

    //        if (accelerationInput != null)
    //        {
    //            accelerationInput.action.performed += context => Accelerate(context);
    //            accelerationInput.action.canceled += context => Accelerate(context, false);
    //        }

    //        if (brakeInput != null)
    //        {
    //            brakeInput.action.performed += context => Brake(context);
    //            brakeInput.action.canceled += context => Brake(context, false);
    //        }

    //        if (steerInput != null)
    //        {
    //            steerInput.action.performed += context => Steer(context);
    //            steerInput.action.canceled += context => Steer(context, false);
    //        }

    //        if (heahLightsInput != null)
    //        {
    //            heahLightsInput.action.performed += context => ToggleHeadLights(context);
    //        }
    //    }
    //    else
    //    {

    //    }

    //    inputInitialized = initialize;
    //}

    public void InitializeInputs(bool initialize)
        => inputInitialized = initialize;

    public void Accelerate(InputFloatValueEvent inputEvent)
        => accelerationInputValue = inputEvent.Value;

    public void Brake(InputFloatValueEvent inputEvent)
    {
        accelerationInputValue = -inputEvent.Value;
        onVehicleLightEvent?.Invoke(VehicleLightType.Brake, inputEvent.Value > 0);
    }

    public void Steer(InputVectorValueEvent inputEvent)
    {
        if (inputEvent is InputVectorValueEvent leftAnalogStick)
        {
            if (!initializeInputs.Contains(leftAnalogStick) ||
                leftAnalogStick.Input != InputVector.LeftJoystick) return;

            steeringInput = leftAnalogStick.Value.x;
        }
    }

    public void ToggleHeadLights(InputActionEvent inputEvent)
    {
        lightsOn = !lightsOn;
        onVehicleLightEvent?.Invoke(VehicleLightType.Head, lightsOn);
    }

    public void Accelerate(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        if (performed)
            accelerationInputValue = 1.0f;
        else
            accelerationInputValue = 0.0f;
    }

    public void Brake(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        if (performed)
            accelerationInputValue = -1.0f;
        else
            accelerationInputValue = 0.0f;

        onVehicleLightEvent?.Invoke(VehicleLightType.Brake, performed);
    }

    public void Steer(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        var input = obj.ReadValue<Vector2>();
        steeringInput = input.x;
    }

    public void ToggleHeadLights(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed = true)
    {
        lightsOn = !lightsOn;
        onVehicleLightEvent?.Invoke(VehicleLightType.Head, lightsOn);
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

    public void ApplyMisteryBox(MisteryBoxType type, int value, ScreenUIEventConfig eventConfig)
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

        OnMisteryBoxAppliedEvent?.Invoke(eventConfig);
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

    public void OnSensorDetection(SensorType sensor, RaycastHit hitInfo)
    {
        if(inputInitialized) return;

        avoidingObstacle = false;

        switch (sensor)
        {
            case SensorType.LeftSideSensor:
                float leftSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle += leftSteerAngle * 0.5f;
                avoidingObstacle = true;
                break;
            case SensorType.RightSideSensor:
                float rightSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle -= rightSteerAngle * 0.5f;
                avoidingObstacle = true;
                break;
            case SensorType.FrontLeftAngledSensor:
                float leftAngledSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle += leftAngledSteerAngle * 0.8f;
                avoidingObstacle = true;
                break;
            case SensorType.FrontRightAngledSensor:
                float rightAngledSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle -= rightAngledSteerAngle * 0.8f;
                avoidingObstacle = true;
                break;
            case SensorType.FrontLeftSensor:
                float frontLeftSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle += frontLeftSteerAngle * 0.5f;
                if (speedKPH <= 1)
                    remainingReverseTime = reverseDuration;
                avoidingObstacle = true;
                break;
            case SensorType.FrontRightSensor:
                float frontRightSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                sensorSteerAngle -= frontRightSteerAngle * 0.5f;
                if (speedKPH <= 1)
                    remainingReverseTime = reverseDuration;
                avoidingObstacle = true;
                break;
            case SensorType.FrontMidSensor:
                float currentSteerAngle = 1 * steering.Evaluate(CurrentSpeed);
                if (hitInfo.normal.x < 0)
                    sensorSteerAngle -= currentSteerAngle * 0.5f;
                else
                    sensorSteerAngle += currentSteerAngle * 0.5f;

                if (speedKPH <= 1)
                    remainingReverseTime = reverseDuration;
                avoidingObstacle = true;
                break;
            default:
                sensorSteerAngle = 0.0f;
                break;
        }

        // Debug.Log($"~On sensor detected: {sensor} and send to vehicle with avoiding: {avoidingObstacle} and sensitivity: {sensorSteerAngle}.");
    }
}
