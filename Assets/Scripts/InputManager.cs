using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using NUnit.Framework.Internal;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;
    private InputAction RightClick;
    private InputAction LeftClick;
    private InputAction Stop;

    void Awake()
    {
        var map = actions.FindActionMap("Player");
        LeftClick = map.FindAction("LeftClick");
        RightClick = map.FindAction("RightClick");
        Stop = map.FindAction("Stop");

    }
    void OnEnable()
    {
        RightClick.Enable();
        LeftClick.Enable();
        Stop.Enable();
    }
    void OnDisable()
    {
        LeftClick.Disable();
        RightClick.Disable();
        Stop.Disable();
    }
    void Update()
    {
        if (LeftClick.WasPressedThisFrame())
        {
            OnLeftClick();
        }
        if (RightClick.WasPressedThisFrame())
        {
            OnRightClick();
        }
        if (Stop.WasPressedThisFrame())
        {
            OnStop();
        }
    }
    void OnRightClick()
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
    void OnLeftClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Debug.Log("Left Click at: " + mousePosition);
        // Implement left-click logic here
    }
    void OnStop()
    {
        UnitController unitController = UnityEngine.Object.FindFirstObjectByType<UnitController>();
        if (unitController != null)
        {
            unitController.Stop();
        }
    }
}
