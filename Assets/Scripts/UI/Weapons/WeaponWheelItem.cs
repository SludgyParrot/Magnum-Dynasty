using UnityEngine;

public sealed class WeaponWheelItem : MonoBehaviour
{
    [SerializeField]
    private GameObject highlight;

    private void Start()
    {
        if(highlight == null)
        {
            Debug.LogError($"{nameof(highlight)} cannot be null.");
            return;
        }
    }

    public void Select()
    {
        highlight.SetActive(true);
    }

    public void Deselect()
        => highlight.SetActive(false);
}
