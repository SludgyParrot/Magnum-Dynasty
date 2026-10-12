using UnityEngine;

public sealed class MiniMapBlip : MonoBehaviour
{
    [SerializeField]
    private Transform target;


    [SerializeField]
    private MiniMap miniMap;


    [SerializeField]
    private RectTransform blipRectTransform;

    private void Start()
    {
        if (target == null) return;
        miniMap = GetComponentInParent<MiniMap>();

        if(miniMap == null)
        {
            Debug.Log($"{nameof(miniMap)} cannot be null.");
            return;
        }

        blipRectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (miniMap == null || target == null) return;
        blipRectTransform.anchoredPosition = miniMap.GetBlipPosition(target.position);
    }
}
