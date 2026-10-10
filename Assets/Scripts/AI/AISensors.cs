using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class Sensor
{
    [SerializeField]
    private string name;

    [SerializeField, Space(5)]
    private Transform origin;

    [SerializeField, Space(5)]
    private float length;

    [SerializeField, Space(5)]
    private SensorType sensorType;

    [SerializeField, Space(5)]
    private Color sensorColor;

    [SerializeField, Space(5)]
    private LayerMask interactableLayer;

    public SensorType SensorType => sensorType;

    public (SensorType sensorType, bool detected, RaycastHit hitInfo) GetDetectionResults()
    {
        if(Physics.Raycast(origin.position, origin.forward, out RaycastHit hitinfo, length, interactableLayer))
        {
            Debug.DrawLine(origin.position, hitinfo.point, sensorColor);
            return (sensorType, true, hitinfo);
        }
        return (sensorType, false, new RaycastHit());
    }
}

public sealed class AISensors : MonoBehaviour
{
    [SerializeField]
    private List<Sensor> sensors = 
        new List<Sensor>();

    [SerializeField, Space(5)]
    private UnityEvent<SensorType, RaycastHit> onSensorDetectedEvent;

    private bool hasSensors;

    private void Start()
        => hasSensors = sensors.Count > 0;

    private void Update()
    {
        if (!hasSensors) return;
        OnSensorDetections();
    }

    private void OnSensorDetections()
    {
        for (int i = 0; i < sensors.Count; i++)
        {
            Sensor sensor = sensors[i];

            var detectionResults = sensor.GetDetectionResults();
            if (detectionResults.detected)
                onSensorDetectedEvent?.Invoke(sensor.SensorType, detectionResults.hitInfo);
            else
                onSensorDetectedEvent?.Invoke(SensorType.None, detectionResults.hitInfo);
        }
    }
}
