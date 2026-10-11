using UnityEngine;
using TMPro;

public sealed class RaceInfoDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI positionDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI lapDisplayer;

    private void Start()
    {
        if(positionDisplayer == null)
        {
            Debug.LogError($"{nameof(positionDisplayer)} cannot be null.");
            return;
        }

        if (lapDisplayer == null)
        {
            Debug.LogError($"{nameof(lapDisplayer)} cannot be null.");
            return;
        }
    }

    public void OnRaceInfoValueChanged(string position, string lap)
    {
        positionDisplayer.SetText(position);
        lapDisplayer.SetText(lap);
    }
}
