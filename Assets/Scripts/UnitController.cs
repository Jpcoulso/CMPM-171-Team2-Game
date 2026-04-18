using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Vector3 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetPosition = Vector3.zero;
            Move(targetPosition);
        }
    }
    public void Move(Vector2 inputVector)
    {
        // Implement movement logic here using the inputVector
        // For example, you can use the inputVector to move the unit in the game world
        Debug.Log("Moving unit with input: " + inputVector);
        targetPosition = new Vector3(inputVector.x, inputVector.y, 0f);
    }
}
