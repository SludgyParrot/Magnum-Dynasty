using UnityEngine;

[RequireComponent (typeof(Camera))]
public sealed class CarCameraController : MonoBehaviour
{
    [SerializeField]
    private Vehicle car;

    [SerializeField, Space(5)]
    private float distance, height;

    [SerializeField, Space(5)]
    private float heightDamping, rotationDamping;

    [SerializeField, Space(5)]
    private float cameraFieldOfView;

    [SerializeField, Space(5)]
    private float zoomRatio;

     private Camera carCamera;

    private Vector3 cameraPositionDelta;
    private Vector3 cameraRotationDelta;

    private void Start()
        => carCamera = GetComponent<Camera>();

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraFOV();
    }

    private void UpdateCameraPosition()
    {
        float carRotationAngle = cameraRotationDelta.y;
        float carHeight = car.transform.position.y + height;

        float cameraRotationAngle = transform.eulerAngles.y;
        float cameraHeight = transform.position.y;

        float targetRotationAngle = Mathf.LerpAngle(cameraRotationAngle, carRotationAngle, rotationDamping * Time.deltaTime);
        float targetHeight = Mathf.Lerp(cameraHeight, carHeight, heightDamping * Time.deltaTime);

        var targetRotation = Quaternion.Euler(0.0f, targetRotationAngle, 0.0f);

        transform.position = car.transform.position;

        transform.position -= targetRotation * Vector3.forward * distance;

        cameraPositionDelta.x = transform.position.x;
        cameraPositionDelta.y = targetHeight;
        cameraPositionDelta.z = transform.position.z;

        transform.position = cameraPositionDelta;
        transform.LookAt(car.transform);
    }

    private void UpdateCameraFOV()
    {
        var cameraInversedDirection = car.GetMovingDirection();

        if(cameraInversedDirection.z < -0.5f)
        {
            cameraRotationDelta = car.transform.eulerAngles;
            cameraRotationDelta.y -= 180;
        }
        else
            cameraRotationDelta = car.transform.eulerAngles;

        float acceleration = car.CurrentSpeed;
        carCamera.fieldOfView = cameraFieldOfView + acceleration * zoomRatio;
    }
}
