using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class BetterCarCameraController : MonoBehaviour
{
    [SerializeField]
    private Vehicle car;

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

    private Camera carCamera;

    private float cameraRotationAngle;
    private float cameraHeight;

    private void Awake()
    {
        carCamera = GetComponent<Camera>();

        cameraRotationAngle = car.transform.eulerAngles.y;
        cameraHeight = car.transform.position.y + height;

        Rigidbody rb = car.GetComponent<Rigidbody>();

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraFOV();
    }

    private void UpdateCameraPosition()
    {
        Vector3 movingDirection = car.GetMovingDirection();

        float targetRotationAngle = car.transform.eulerAngles.y;

        // Reverse camera direction when car is moving backwards.
        if (movingDirection.z < -0.5f)
            targetRotationAngle += 180f;

        cameraRotationAngle = Mathf.LerpAngle(
            cameraRotationAngle,
            targetRotationAngle,
            rotationDamping * Time.deltaTime
        );

        float targetHeight = car.transform.position.y + height;

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
            car.transform.position -
            rotation * Vector3.forward * distance;

        targetPosition.y = cameraHeight;

        transform.position = targetPosition;

        transform.LookAt(
            car.transform.position + Vector3.up * height * 0.5f
        );
    }

    private void UpdateCameraFOV()
    {
        float speed = Mathf.Abs(car.CurrentSpeed);

        carCamera.fieldOfView =
            cameraFieldOfView +
            speed * zoomRatio;
    }
}