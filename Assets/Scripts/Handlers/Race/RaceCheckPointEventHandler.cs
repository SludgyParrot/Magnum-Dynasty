using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class RaceCheckPointEvent
{
    [SerializeField]
    private string eventName;

    [field: SerializeField, Space(5)]
    public int Lap { get; private set; }

    [field: SerializeField, Space(5)]
    public UnityEvent OnTriggeredEvent { get; private set; }
}

[DisallowMultipleComponent]
public sealed class RaceCheckPointEventHandler : MonoBehaviour
{
    [SerializeField]
    private List<RaceCheckPointEvent> checkPointEvents = 
        new List<RaceCheckPointEvent>();

    public void OnCheckPointReached(int checkpointIndex)
    {
        if (checkpointIndex <= 0) return;

        var checkPointEventResults = GetRaceCheckPointEvent(checkpointIndex);

        if(!string.IsNullOrWhiteSpace(checkPointEventResults.errorMessage))
        {
            Debug.LogError(checkPointEventResults.errorMessage);
            return;
        }

        checkPointEventResults.checkPointEvent.OnTriggeredEvent?.Invoke();
    }

    private (RaceCheckPointEvent checkPointEvent, string errorMessage) GetRaceCheckPointEvent(int lap)
    {
        RaceCheckPointEvent checkPointEvent = checkPointEvents.FirstOrDefault(x => x.Lap == lap);
        string errorMessage = checkPointEvent == null ? $"{checkPointEvent} not found in {nameof(checkPointEvents)} for {gameObject.name}." : string.Empty;
        return (checkPointEvent,  errorMessage);
    }
}
