using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct RacePointColor
{
    public string Name;
    public Color Color;
    public RacePointType PointType;

    public RacePointColor(string name, Color color, RacePointType point)
    {
        Name = name;
        Color = color;
        PointType = point;
    }
}

[RequireComponent(typeof(BoxCollider))]
public sealed class RaceActionEventPointHandler : MonoBehaviour
{
    [SerializeField]
    private RacePointType racePoint;

    [SerializeField, Space(5)]
    private List<RacePointColor> colors = new List<RacePointColor>
    {
        new RacePointColor("Check Point", Color.yellow, RacePointType.CheckPoint),
        new RacePointColor("Acceleration Point", Color.green, RacePointType.Acceleration),
        new RacePointColor("Brake Point", Color.red, RacePointType.Brake),

    };

    [SerializeField, Space(5)]
    private List<Vehicle> vehicles = new List<Vehicle>();

    [Header("Brake Point Settings")]
    [SerializeField, Space()]
    private float maxSpeedLimit;


    private BoxCollider trigger;

    private void OnDrawGizmos()
    {
        if(trigger == null)
            trigger = GetComponent<BoxCollider>();

        Gizmos.color = GetRacePointColor(racePoint);
        Gizmos.DrawCube(transform.position, trigger.size);
    }

    private void OnTriggerEnter(Collider other)
    {
        var vehicle = other.GetComponent<Vehicle>();

        if (vehicle == null) return;

        if(vehicles.Contains(vehicle)) return;

        vehicles.Add(vehicle);
    }

    private void OnTriggerStay()
    {
        foreach(var vehicle in vehicles)
        {
            if(vehicle.IsPlayer) continue;

            if(racePoint == RacePointType.Brake)
            {
                if (vehicle.CurrentSpeed > maxSpeedLimit)
                    vehicle.ApplyBrakes();
                else
                    vehicle.ApplyAcceleration();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var vehicle = other.GetComponent<Vehicle>();

        if (vehicle == null) return;

        if (!vehicles.Contains(vehicle))
            return;

        switch (racePoint)
        {
            case RacePointType.Acceleration:
            case RacePointType.Brake:
                if(!vehicle.IsPlayer)
                    vehicle.ApplyAcceleration();
                break;
            case RacePointType.CheckPoint:
                vehicle.IncrementLaps();
                break;
        }

        vehicles.Remove(vehicle);
    }

    private Color GetRacePointColor(RacePointType pointType)
        => colors.FirstOrDefault(x => x.PointType == pointType).Color;
}
