using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public sealed class Wheel : MonoBehaviour
{
    [SerializeField]
    private Transform wheelTransform;

    [field: SerializeField]
    public bool Steering {  get; private set; }

    private WheelCollider wheelCollider;

    private bool initialized;

    private void Start()
    {
        if(wheelTransform == null)
        {
            Debug.LogWarning("Wheel transform cannot be null.");
            return;
        }

        wheelCollider = GetComponent<WheelCollider>();
        initialized = true;
    }

    public void UpdateWheel()
    {
        if (!initialized) return;

        wheelCollider.GetWorldPose(out var pos, out var rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    public void Steer(float steerAngle)
    {
        if(!Steering) return;
        wheelCollider.steerAngle = steerAngle;
    }

    public void AddTorque(float torque)
        => wheelCollider.motorTorque = torque;

    public void AddBrakeTorque(float brake)
        => wheelCollider.brakeTorque = brake;
}
