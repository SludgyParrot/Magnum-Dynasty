using UnityEngine;
using UnityEngine.UI;

public sealed class MiniMapBlip : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField, Space(5)]
    private bool changeOutOfBoundsColor;

    [SerializeField, Space(5)]
    private Image blipTexture;

    [SerializeField, Space(5)]
    private Color defaultColor = Color.white,
        outOfBoundsColor = Color.white;

    private MiniMap miniMap;
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

        if(blipTexture == null)
            blipTexture = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        if (miniMap == null || target == null) return;

        var positionData = miniMap.GetBlipPosition(target.position);
        blipRectTransform.anchoredPosition = positionData.position;

        if (changeOutOfBoundsColor)
        {
            if(positionData.outOfBounds)
                blipTexture.color = outOfBoundsColor;
            else
                blipTexture.color = defaultColor;
        }
    }
}
