using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public sealed class ActionInput
{
    public string Name;

    [Space(5)]
    public InputActionReference Reference;

    [Space(5)]
    public InputAction Action;
}

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset input;

    [SerializeField, Space(5)]
    private List<ActionInput> actions = 
        new List<ActionInput>();

    [SerializeField, Space(5)]
    private UnityEvent<InputAction, bool> onInputEvent = 
        new UnityEvent<InputAction, bool>();

    [SerializeField, Space(5)]
    private UnityEvent<Vector2, bool> onInputVector2Event =
       new UnityEvent<Vector2, bool>();

    private void Start()
    {
        if (actions.Count == 0) return;

        if(input != null)
            input.Enable();

        foreach (var input in actions)
        {
            input.Reference.action.performed += context => SendInputActionEvent(input.Action, context, true);
            input.Reference.action.canceled += context => SendInputActionEvent(input.Action, context, false);
        }
    }

    private void SendInputActionEvent(InputAction action, UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed)
    {
        switch(action)
        {
            case InputAction.Accelerate:
            case InputAction.Brake:
                onInputEvent?.Invoke(action, performed);
                break;
            case InputAction.Steer:
                var results = obj.ReadValue<Vector2>();
                onInputVector2Event?.Invoke(results, performed);
                break;
        }
    }

}
