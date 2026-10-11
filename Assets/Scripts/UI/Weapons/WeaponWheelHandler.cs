using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class WeaponWheelHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputAction;

    [SerializeField, Space(5)]
    private InputActionReference scrollLeftInput,
        scrollRightInput;

    [SerializeField, Space(5)]
    private List<WeaponWheelItem> wheelItems = 
        new List<WeaponWheelItem>();

    private int currentScrollIndex = 0;

    private void Start()
    {
        if (inputAction == null) return;

        inputAction.Enable();

        if (scrollRightInput != null)
            scrollRightInput.action.performed += context => Scroll(context);

        if (scrollLeftInput != null)
            scrollLeftInput.action.performed += context => Scroll(context, false);

        SelectItemAtIndex(currentScrollIndex);
    }

    private void Scroll(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool scrollRight = true)
    {
        if(scrollRight)
        {
            if (currentScrollIndex < wheelItems.Count - 1)
                currentScrollIndex++;
            else
                currentScrollIndex = 0;
        }
        else
        {
            if (currentScrollIndex > 0)
                currentScrollIndex--;
            else
                currentScrollIndex = wheelItems.Count - 1;
        }

        SelectItemAtIndex(currentScrollIndex);
    }

    private void SelectItemAtIndex(int slectionIndex)
    {
        for (int i = 0; i < wheelItems.Count; i++)
        {
            if (i == slectionIndex)
                wheelItems[i].Select();
            else
                wheelItems[i].Deselect();
        }
    }
}
