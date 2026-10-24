using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class BetterCarCameraController : MonoBehaviour
{
    [SerializeField, Space(5)]
    private float distance = 6f;

    [SerializeField]
    private float height = 2.5f;

    [SerializeField, Space(5)]
    private float heightDamping = 5f;

    [SerializeField]
    private float rotationDamping = 5f;

    [SerializeField, Space(5)]
    private float cameraFieldOfView = 60f;

    [SerializeField]
    private float zoomRatio = 0.5f;

    private float cameraRotationAngle;
    private float cameraHeight;

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

        if (config is PlayerSelectedVehicleEvent selectedVehicleEvent)
        {
            selectedVehicle = selectedVehicleEvent.SelectedVehicle;
            InitializeVehicle();
        }

        if (config is InputActionEvent rearViewButton)
        {
            if (rearViewButton.Input == InputButton.AButton)
                RearView(rearViewButton.Performed);
        }
    }

    private void RearView(bool performed)
        => isRearView = performed;

    private void InitializeVehicle()
    {
        cameraRotationAngle = selectedVehicle.transform.eulerAngles.y;
        cameraHeight = selectedVehicle.transform.position.y + height;

        Rigidbody rb = selectedVehicle.GetComponent<Rigidbody>();

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void LateUpdate()
    {
        if(selectedVehicle == null) return;

        UpdateCameraPosition();
        UpdateCameraFOV();
    }

    private void UpdateCameraPosition()
    {
        Vector3 movingDirection = selectedVehicle.GetMovingDirection();

        float targetRotationAngle = selectedVehicle.transform.eulerAngles.y;

        // Reverse camera direction when car is moving backwards.
        if (movingDirection.z < -0.5f)
            targetRotationAngle += 180f;

        cameraRotationAngle = Mathf.LerpAngle(
            cameraRotationAngle,
            targetRotationAngle,
            rotationDamping * Time.deltaTime
        );

        float targetHeight = selectedVehicle.transform.position.y + height;

        cameraHeight = Mathf.Lerp(
            cameraHeight,
            targetHeight,
            heightDamping * Time.deltaTime
        );

        Quaternion rotation = Quaternion.Euler(
            0f,
            cameraRotationAngle,
            0f
        );

        Vector3 targetPosition =
            selectedVehicle.transform.position -
            rotation * Vector3.forward * distance;

        targetPosition.y = cameraHeight;

        transform.position = targetPosition;

        transform.LookAt(
            selectedVehicle.transform.position + Vector3.up * height * 0.5f
        );
    }

    private void UpdateCameraFOV()
    {
        float speed = Mathf.Abs(selectedVehicle.CurrentSpeed);

        CarViewCamera.fieldOfView =
            cameraFieldOfView +
            speed * zoomRatio;
    }
}