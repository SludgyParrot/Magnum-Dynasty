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
    private MatchStartedEvent matchStartedEventConfig;

    [SerializeField, Space(5)]
    private MatchCompletedEvent matchCompletionEventConfig;

    [SerializeField, Space(5)]
    private UnityEvent<string> onRaceTimeValueChangedEvent;

    [SerializeField, Space(5)]
    private UnityEvent<string, string> onRaceInfoValueChangedEvent;

    public List<Vehicle> Vehicles => vehicles;

    private List<RaceData> raceDataList = 
        new List<RaceData>();

    private float currentRaceTime;

    private Vehicle selectedVehicle;
    public bool RaceInProgress { get; private set; }
    public string RaceTimeStamp { get; private set; }

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnInputEvents);

    private void OnDisable()
        => EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnInputEvents);

    private void OnInputEvents(EventConfig config)
    {
        if (config == null) return;

        if (config is PlayerSelectedVehicleEvent selectedVehicleEvent)
            selectedVehicle = selectedVehicleEvent.SelectedVehicle;
    }

    private void Start()
    {
        if(vehicles.Count == 0)
            vehicles = FindObjectsOfType<Vehicle>().ToList();

        RaceInProgress = true;
    }

    private void LateUpdate()
    {
        if(selectedVehicle == null || 
            !RaceInProgress) return;

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

        int racePosition = GetRacePosition(selectedVehicle);
        var position = string.Format(racePositionInfoStringFormat, racePosition, vehicles.Count);
        onRaceInfoValueChangedEvent?.Invoke(position, string.Format(raceLapInfoStringFormat, selectedVehicle.CurrentLap, TotalLaps));
    }

    private void OnRaceTimeUpdate()
    {
        currentRaceTime += Time.deltaTime;

        var minutes = Mathf.FloorToInt(currentRaceTime / 60.0f);
        var seconds = Mathf.FloorToInt(currentRaceTime % 60.0f);
        var milliseconds = Mathf.FloorToInt((currentRaceTime * 100.0f) % 100.0f);
        RaceTimeStamp = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        onRaceTimeValueChangedEvent?.Invoke(RaceTimeStamp);
    }

    private void StartRace()
    {
        if (matchStartedEventConfig == null) return;
        EventsManager.Instance.OnTriggeredEvent?.Invoke(matchStartedEventConfig);
    }

    private void CompleteRace()
    {
        if (matchCompletionEventConfig == null) return;
        EventsManager.Instance.OnTriggeredEvent?.Invoke(matchCompletionEventConfig);
    }

    public int GetRacePosition(Vehicle vehicle)
    {
        vehicles.Sort((x, y) => 
        {
            if(x.CurrentLap != y.CurrentLap) 
                return y.CurrentLap.CompareTo(x.CurrentLap);

            if(x.PathIndex != y.PathIndex)
                return y.PathIndex.CompareTo(x.PathIndex);

            return x.DistanceToWaypoint.CompareTo(y.DistanceToWaypoint);
        
        });
        return vehicles.IndexOf(vehicle) + 1;
    }

    public RaceOutcomeStatus GetRaceOutcomeStatus(Vehicle vehicle)
    {
        var racePosition = GetRacePosition(vehicle);
        return racePosition == 1 ? RaceOutcomeStatus.Won : RaceOutcomeStatus.Lost;
    }

    public void SetMatchStatus(RaceData data)
    {
        if(data == null) return;

        if(data.IsPlayer)
        {
            switch (data.Status)
            {
                case RaceStatus.Started:
                    StartRace();
                    break;
                case RaceStatus.InProgress:
                    break;
                case RaceStatus.Completed:
                    CompleteRace();
                    break;
            }
        }

       if(raceDataList.Contains(data)) return;
       raceDataList.Add(data);
    }

    public (RaceData data, string errorMessage) GetPlayerRaceData()
    {
        RaceData data = raceDataList.FirstOrDefault(x => x.IsPlayer);
        string errorMessage = data == null? $"{data} was not found in {nameof(raceDataList)} for: {gameObject.name}" : string.Empty;
        return (data, errorMessage);
    }

    public void ClearRaceData()
        => raceDataList.Clear();
}
