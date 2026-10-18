using UnityEngine;
using UnityEngine.Events;

public sealed class WeaponWheelItem : MonoBehaviour
{ 
    [SerializeField]
    private GameObject highlight;

    [SerializeField, Space(5)]
    private UnityEvent onInitialization =
        new UnityEvent();

   [SerializeField, Space(5)]
    private UnityEvent<UIViewState> onSelectionState = 
        new UnityEvent<UIViewState>();

    private void Start()
    {
        if (highlight == null)
        {
            Debug.LogError($"{nameof(highlight)} cannot be null.");
            return;
        }

        onInitialization?.Invoke();
    }

    public void Select()
    {
        highlight.SetActive(true);
        onSelectionState?.Invoke(UIViewState.Show);
    }

    public void Deselect()
    {
        highlight.SetActive(false);
        onSelectionState?.Invoke(UIViewState.Hide);
    }
}
