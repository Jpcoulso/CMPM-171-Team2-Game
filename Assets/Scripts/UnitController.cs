using System;
using UnityEngine;


public class UnitController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 destination;
    private bool hasDestination = false;
    public void Move(Vector2 inputVector)
    {
        // Implement movement logic here
        destination = new Vector3(inputVector.x, inputVector.y, 0);
        hasDestination = true;
    }
    public void Stop()
    {
        hasDestination = false;
    }
    void Update()
    {
        if (!hasDestination)
        {
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        
    }
}
