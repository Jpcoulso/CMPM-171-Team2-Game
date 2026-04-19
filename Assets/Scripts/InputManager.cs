using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

public class InputManager : MonoBehaviour
{
    private InputAction RightClick;
    private InputAction LeftClick;

    void OnEnable()
    {
        RightClick = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        LeftClick = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        RightClick.performed += OnRightClick;
        LeftClick.performed += OnLeftClick;
        RightClick.Enable();
        LeftClick.Enable();
    }
    void OnDisable()
    {
        RightClick.performed -= OnRightClick;
        LeftClick.performed -= OnLeftClick;
        LeftClick.Disable();
        RightClick.Disable();
    }
    void OnRightClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPosition.z = 0; // Set z to 0 for 2D

        Debug.Log("Right Click at: " + mousePosition);
        Debug.Log("World Position: " + worldPosition);

        // Implement right-click logic here
        UnitController unitController = UnityEngine.Object.FindFirstObjectByType<UnitController>();
        if (unitController != null)
        {
            unitController.Move(worldPosition);
        }
    }
    void OnLeftClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Debug.Log("Left Click at: " + mousePosition);
        // Implement left-click logic here
    }
}
