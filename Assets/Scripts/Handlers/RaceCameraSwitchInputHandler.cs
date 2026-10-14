using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class RaceCameraSwitchInputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    [SerializeField, Space(5)]
    private InputActionReference cameraViewChangeInput;

    [SerializeField, Space(5)]
    private UnityEvent<CameraViewType> onCameraViewChangedEvent;

    private bool switched;

    private void Start()
    {
        if (inputActions == null) return;

        inputActions.Enable();

        if (cameraViewChangeInput != null)
            cameraViewChangeInput.action.performed += SwitchCameraView;
    }

    private void SwitchCameraView(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        switched = !switched;

        if (switched)
            onCameraViewChangedEvent?.Invoke(CameraViewType.RacingCockpit);
        else
            onCameraViewChangedEvent?.Invoke(CameraViewType.Racing);
    }
}
