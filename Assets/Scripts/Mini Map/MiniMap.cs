using UnityEngine;

public sealed class MiniMap : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float zoomRatio;

    private RectTransform mapRect;

    public RectTransform MapRect
    {
        get
        {
            if (mapRect == null)
                mapRect = GetComponent<RectTransform>();
            return mapRect;
        }
    }

    public float ZoomRatio => zoomRatio;
    public float MapRadius => Mathf.Min(MapRect.rect.width, MapRect.rect.height) / 2.0f;

    private Vector2 screenSpacePosition;

    private bool outOfBounds = false;

    public (Vector2 position, bool outOfBounds) GetBlipPosition(Vector3 targetPosition)
    {
        Vector3 worldPositionOffset = targetPosition - target.position;
        screenSpacePosition.x = worldPositionOffset.x;
        screenSpacePosition.y = worldPositionOffset.z;
        screenSpacePosition *= zoomRatio;

        screenSpacePosition = GetRotationVector(screenSpacePosition, target.eulerAngles.y);

     
        if(screenSpacePosition.magnitude > MapRadius)
        {
            outOfBounds = false;
            screenSpacePosition = screenSpacePosition.normalized * MapRadius;
        }
        else
            outOfBounds= true;

        return (screenSpacePosition, outOfBounds);
    }

    private Vector2 GetRotationVector(Vector2 vector, float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2((vector.x * cos) - (vector.y * sin), (vector.x * sin) + (vector.y * cos));
    }
}
