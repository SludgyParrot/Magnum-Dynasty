using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public sealed class ActionInput
{
    [SerializeField]
    private string Name;

    [field: SerializeField, Space(5)]
    public InputActionReference Reference { get; private set; }

    [field: SerializeField, Space(5)]
    public InputEvent ActionEvent { get; private set; }
}

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset input;

    [SerializeField, Space(5)]
    private List<ActionInput> actions = 
        new List<ActionInput>();

    [SerializeField, Space(5)]
    private UnityEvent<InputEvent> onInputEvent = 
        new UnityEvent<InputEvent>();

    private void OnEnable()
    {
        if (actions.Count == 0) return;

        if(input != null)
            input.Enable();

        foreach (var input in actions)
            OnRegisterInputs(input);
    }

    private void OnDisable()
    {
        if (actions.Count == 0) return;

        if (input != null)
            input.Disable();

        foreach (var input in actions)
            OnRegisterInputs(input, false);
    }

    private void OnRegisterInputs(ActionInput input, bool register = true)
    {
        if (input.ActionEvent == null)
        {
            Debug.LogError($"{nameof(input.ActionEvent)} cannot be null for: {gameObject.name}");
            return;
        }

        if (register)
        {
            input.Reference.action.performed += context => SendInputActionEvent(input.ActionEvent, context, true);
            input.Reference.action.canceled += context => SendInputActionEvent(input.ActionEvent, context, false);
        }
        else
        {
            input.Reference.action.performed -= context => SendInputActionEvent(input.ActionEvent, context, true);
            input.Reference.action.canceled -= context => SendInputActionEvent(input.ActionEvent, context, false);
        }
    }

    private void SendInputActionEvent(InputEvent actionEvent, UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed)
    {
        if (actionEvent is InputActionEvent action)
        {
            action.SetPerformed(performed);
            onInputEvent?.Invoke(action);
            EventsManager.Instance.OnTriggeredEvent?.Invoke(action);
        }

        if(actionEvent is InputFloatValueEvent floatValue)
        {
            floatValue.SetValue(obj.ReadValue<float>());
            onInputEvent?.Invoke(floatValue);
            EventsManager.Instance.OnTriggeredEvent?.Invoke(floatValue);
        }

        if(actionEvent is InputVectorValueEvent vectorValue)
        {
            vectorValue.SetValue(obj.ReadValue<Vector2>());
            onInputEvent?.Invoke(vectorValue);
            EventsManager.Instance.OnTriggeredEvent?.Invoke(vectorValue);
        }
    }
}
