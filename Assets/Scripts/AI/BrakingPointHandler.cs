using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class BrakingPointHandler : MonoBehaviour
{
    [SerializeField]
    private float maxSpeedLimit;

    [SerializeField, Space(5)]
    private List<Vehicle> vehicles = new List<Vehicle>();

    [SerializeField, Space(5)]
    private Color gizmoColor = Color.red;

    private BoxCollider brakePointTrigger;

    private void OnDrawGizmos()
    {
        if(brakePointTrigger == null)
            brakePointTrigger = GetComponent<BoxCollider>();

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, brakePointTrigger.size);
    }

    private void OnTriggerEnter(Collider other)
    {
        var vehicle = other.GetComponent<Vehicle>();

        if (vehicle == null || vehicle.IsPlayer) return;

        if(vehicles.Contains(vehicle)) return;

        vehicles.Add(vehicle);
    }

    private void OnTriggerStay()
    {
        if(vehicles.Count == 0) return;

        foreach(var vehicle in vehicles)
        {
            if(vehicle.CurrentSpeed > maxSpeedLimit)
                vehicle.ApplyBrakes();
            else
                vehicle.ApplyAcceleration();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var vehicle = other.GetComponent<Vehicle>();

        if (vehicle == null || vehicle.IsPlayer) return;

        if(vehicles.Contains(vehicle))  
            vehicles.Remove(vehicle);

        vehicle.ApplyAcceleration();
    }
}
