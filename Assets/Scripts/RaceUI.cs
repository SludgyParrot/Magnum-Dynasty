using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class RaceUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI racePosition;

    public void DisplayRacePosition(int position)
        => racePosition.SetText(position.ToString());
}
