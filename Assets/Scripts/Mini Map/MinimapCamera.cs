using UnityEngine;

[DisallowMultipleComponent]
public sealed class MinimapCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField, Space(5)]
    private float followSpeed, 
        rotationDamping;

    private Vector3 followVector;

    private void LateUpdate()
    {
        followVector = transform.position;
        followVector = Vector3.Lerp(followVector, target.position, followSpeed * Time.deltaTime);
        followVector.y = transform.position.y;
        transform.position = followVector;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z), rotationDamping * Time.deltaTime);
    }
}
