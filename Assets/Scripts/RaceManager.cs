using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class RaceManager : SingletonInstance<RaceManager>
{
    [SerializeField, Space(5)]
    private List<Vehicle> vehicles = 
        new List<Vehicle>();

    [field: SerializeField, Space(5)]
    public int TotalLaps { get; private set; }

    [SerializeField, Space(5)]
    private string racePositionInfoStringFormat, 
        raceLapInfoStringFormat;


   [SerializeField, Space(5)]
    private UnityEvent<string> onRaceTimeValueChangedEvent;

    [SerializeField, Space(5)]
    private UnityEvent<string, string> onRaceInfoValueChangedEvent;

    public List<Vehicle> Vehicles => vehicles;

    private Vehicle playerVehicle;

    public Vehicle PlayerVehicle
    {
        get
        {
            if (playerVehicle == null) 
                playerVehicle = vehicles.FirstOrDefault(x => x.IsPlayer);
            return playerVehicle;
        }
    }

    public bool RaceInProgress { get; private set; }

    private float currentRaceTime;

    private void Start()
    {
        if(vehicles.Count == 0)
            vehicles = FindObjectsOfType<Vehicle>().ToList();

        RaceInProgress = true;
    }

    private void Update()
    {
        if(!RaceInProgress) return;
        OnRaceTimeUpdate();
        OnUpdateRaceInfo();
    }

    private void OnUpdateRaceInfo()
    {
        if (string.IsNullOrWhiteSpace(racePositionInfoStringFormat) || string.IsNullOrWhiteSpace(raceLapInfoStringFormat))
        {
            Debug.LogError($"{racePositionInfoStringFormat} or {raceLapInfoStringFormat} cannot be null or contains white spaces.");
            return;
        }

        int racePosition = GetRacePosition(PlayerVehicle);
        var position = string.Format(racePositionInfoStringFormat, racePosition, vehicles.Count);
        var lap = string.Format(raceLapInfoStringFormat, PlayerVehicle.CurrentLap, TotalLaps);
        onRaceInfoValueChangedEvent?.Invoke(position, lap);
    }

    private void OnRaceTimeUpdate()
    {
        currentRaceTime += Time.deltaTime;

        var minutes = Mathf.FloorToInt(currentRaceTime / 60.0f);
        var seconds = Mathf.FloorToInt(currentRaceTime % 60.0f);
        var milliseconds = Mathf.FloorToInt((currentRaceTime * 100.0f) % 100.0f);

        var timeFormat = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        onRaceTimeValueChangedEvent?.Invoke(timeFormat);
    }

    public int GetRacePosition(Vehicle vehicle)
    {
        var results = vehicles.OrderByDescending(x => x.CurrentLap).ThenByDescending(x => x.PathIndex).ThenBy(x => x.DistanceToWaypoint).ToList();
        vehicles = results;
        return vehicles.IndexOf(vehicle) + 1;
    }

    public void SetPlayerVehicle(Vehicle vehicle)
        => playerVehicle = vehicle;
}
