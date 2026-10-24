using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Camera))]
public sealed class CarCameraController : MonoBehaviour
{
    [SerializeField, Space(5)]
    private float distance, height;

    [SerializeField, Space(5)]
    private float heightDamping, rotationDamping;

    [SerializeField, Space(5)]
    private float cameraFieldOfView;

    [SerializeField, Space(5)]
    private float zoomRatio;

    private Vector3 cameraPositionDelta;
    private Vector3 cameraRotationDelta;

    private bool isRearView;

    private Vehicle selectedVehicle;

    private Camera carViewCamera;
    private Camera CarViewCamera
    {
        get
        {
            if (carViewCamera == null)
                carViewCamera = GetComponent<Camera>();
            return carViewCamera;
        }
    }

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnInputEvents);

    private void OnDisable()
        => EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnInputEvents);

    private void OnInputEvents(EventConfig config)
    {
        if (config == null) return;

        if(config is PlayerSelectedVehicleEvent selectedVehicleEvent)
            selectedVehicle = selectedVehicleEvent.SelectedVehicle;

        if (config is InputActionEvent rearViewButton)
        {
            if (rearViewButton.Input == InputButton.AButton)
                RearView(rearViewButton.Performed);
        }
    }

    private void RearView(bool performed)
        => isRearView = performed;

    private void LateUpdate()
    {
        if(selectedVehicle ==  null) return;

        UpdateCameraPosition();
        UpdateCameraFOV();
    }

    private void UpdateCameraPosition()
    {
        float carRotationAngle = cameraRotationDelta.y;
        float carHeight = selectedVehicle.transform.position.y + height;

        float cameraRotationAngle = transform.eulerAngles.y;
        float cameraHeight = transform.position.y;

        float targetRotationAngle = Mathf.LerpAngle(cameraRotationAngle, carRotationAngle, rotationDamping * Time.deltaTime);
        float targetHeight = Mathf.Lerp(cameraHeight, carHeight, heightDamping * Time.deltaTime);

        var targetRotation = Quaternion.Euler(0.0f, targetRotationAngle, 0.0f);

        transform.position = selectedVehicle.transform.position;

        transform.position -= targetRotation * Vector3.forward * distance;

        cameraPositionDelta.x = transform.position.x;
        cameraPositionDelta.y = targetHeight;
        cameraPositionDelta.z = transform.position.z;

        transform.position = cameraPositionDelta;
        transform.LookAt(selectedVehicle.transform);
    }

    private void UpdateCameraFOV()
    {
        var cameraInversedDirection = selectedVehicle.GetMovingDirection();

        if(cameraInversedDirection.z < -0.5f || isRearView)
        {
            cameraRotationDelta = selectedVehicle.transform.eulerAngles;
            cameraRotationDelta.y -= 180;
        }
        else
            cameraRotationDelta = selectedVehicle.transform.eulerAngles;

        float acceleration = selectedVehicle.CurrentSpeed;
        CarViewCamera.fieldOfView = cameraFieldOfView + acceleration * zoomRatio;
    }
}
