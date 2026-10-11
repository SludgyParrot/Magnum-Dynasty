using UnityEngine;
using TMPro;

public sealed class SpeedOMeterDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI speedOMeterKPHDisplayer;

    [SerializeField, Space(5)]
    private Transform speedOmeterNeedle;

    [SerializeField, Space(5)]
    private float minSpeedOMeterNeedleAngle, 
        maxSpeedOMeterNeedleAngle;

    private void OnEnable()
        => EventsManager.Instance.OnSpeedOMeterValueChanged.AddListener(SetSpeedOMeterKPH);

    private void OnDisable()
        => EventsManager.Instance.OnSpeedOMeterValueChanged.RemoveListener(SetSpeedOMeterKPH);

    private void Start()
    {
        if (speedOMeterKPHDisplayer == null)
        {
            Debug.LogError($"{nameof(speedOMeterKPHDisplayer)} cannot be null.");
            return;
        }

        if (speedOmeterNeedle == null)
        {
            Debug.LogError($"{nameof(speedOmeterNeedle)} cannot be null.");
            return;
        }
    }

    private void SetSpeedOMeterKPH(float speedOMeterKPH, float maxSpeed)
    {
        speedOMeterKPH = Mathf.Clamp(speedOMeterKPH, 0.0f, maxSpeed);
        float needleSpeed = speedOMeterKPH / maxSpeed;
        float needleAngle = Mathf.Lerp(minSpeedOMeterNeedleAngle, maxSpeedOMeterNeedleAngle, needleSpeed);

        speedOmeterNeedle.localRotation = Quaternion.Euler(0.0f, 0.0f, needleAngle);
        var speedToDisplay = Mathf.RoundToInt(speedOMeterKPH).ToString();
        speedOMeterKPHDisplayer.SetText(speedToDisplay);
    }
}
