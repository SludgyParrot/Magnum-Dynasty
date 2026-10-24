using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerManager : SingletonInstance<PlayerManager>
{
    [SerializeField]
    private Vehicle player;

    [SerializeField, Space(5)]
    private PlayerSelectedVehicleEvent vehicleSelectionEventConfig;

    private Vehicle selectedPlayer;
    public Vehicle Player => selectedPlayer;

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnInputEvent);

    private void OnDisable()
       => EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnInputEvent);

    private void OnInputEvent(EventConfig inputEvent)
    {
        if (inputEvent is InputFloatValueEvent leftTriggerButton)
        {
            if (leftTriggerButton.Input == InputFloat.LeftTrigger)
                Player.Brake(leftTriggerButton);
        }

        if (inputEvent is InputFloatValueEvent rightTriggerButton)
        {
            if (rightTriggerButton.Input == InputFloat.RightTrigger)
                Player.Accelerate(rightTriggerButton);
        }
      
        if (inputEvent is InputVectorValueEvent leftAnalogStick)
        {
            if (leftAnalogStick.Input == InputVector.LeftJoystick)
                Player.Steer(leftAnalogStick);
        }

        if (inputEvent is InputActionEvent headLightsButton)
        {
            if (headLightsButton.Input == InputButton.DPadUp)
                Player.ToggleHeadLights(headLightsButton);
        }
    }

    private void Start()
    {
        SetPlayerVehicle(player);
    }

    private void LateUpdate()
    {
        if(player == null) return;
        EventsManager.Instance.OnSpeedOMeterValueChanged?.Invoke(player.SpeedKPH, player.MaxSpeed);
    }

    private void OnMisteryBoxEvent(ScreenUIEventConfig eventConfig)
        => EventsManager.Instance.OnTriggeredEvent?.Invoke(eventConfig);

    public void SetPlayerVehicle(Vehicle newPlayer)
    {
        if (vehicleSelectionEventConfig == null ||
            newPlayer == null) return;

        if (selectedPlayer != null)
        {
            selectedPlayer.InitializeInputs(false);
            selectedPlayer.OnMisteryBoxAppliedEvent?.RemoveListener(OnMisteryBoxEvent);
        }

        // newPlayer.InitializeInputs(true);
        newPlayer.OnMisteryBoxAppliedEvent?.AddListener(OnMisteryBoxEvent);

        selectedPlayer = newPlayer;
        vehicleSelectionEventConfig.SetSelectedVehicle(selectedPlayer);
        EventsManager.Instance.OnTriggeredEvent?.Invoke(vehicleSelectionEventConfig);
    }

    public void InitializeInputs(List<EventConfig> inputs)
    {
        if(selectedPlayer == null) return;
        selectedPlayer.InitializeInputs(inputs);
    }
}
