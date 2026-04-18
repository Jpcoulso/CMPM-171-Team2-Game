using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction RightClick;
    private InputAction LeftClick;
    public UnitController unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void HandleAction(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move")
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            unit.Move(inputVector);
        }
    }
    private void inputRight()
    {
        if (RightClick.triggered)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            unit.Move(mousePos);
            Debug.Log("Right Clicked");
        }
    }
}
