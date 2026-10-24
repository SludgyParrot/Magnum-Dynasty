using System;

[Serializable]
public sealed class RaceData
{
    public string RacerName { get; private set; }

    public int Position { get; private set; }

    public string CompletionTime { get; private set; }

    public RaceStatus Status { get; private set; }

    public RaceOutcomeStatus Outcome { get; private set; }

    public bool IsPlayer { get; private set; }

    public RaceData(){}

    public RaceData(string racerName, 
        int position, 
        string completionTimeStamp,
        RaceStatus status, 
        RaceOutcomeStatus outcome, 
        bool isPlayer = false)
    {
        RacerName = racerName;
        Position = position;
        CompletionTime = completionTimeStamp;
        Status = status;
        Outcome = outcome;
        IsPlayer = isPlayer;
    }

    public void SetIsPlayer(bool isPlayer) 
        => IsPlayer = isPlayer;
}
