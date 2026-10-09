using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class RaceManager : SingletonInstance<RaceManager>
{
    [SerializeField, Space(5)]
    private List<Vehicle> vehicles = 
        new List<Vehicle>();

    [field: SerializeField, Space(5)]
    public int TotalLaps { get; private set; }

    public List<Vehicle> Vehicles => vehicles;

    private void Start()
    {
        if(vehicles.Count == 0)
            vehicles = FindObjectsOfType<Vehicle>().ToList();
    }

    public int GetRacePosition(Vehicle vehicle)
    {
        var results = vehicles.OrderByDescending(x => x.CurrentLap).ThenByDescending(x => x.PathIndex).ThenBy(x => x.DistanceToWaypoint).ToList();
        vehicles = results;
        return vehicles.IndexOf(vehicle) + 1;
    }
}
