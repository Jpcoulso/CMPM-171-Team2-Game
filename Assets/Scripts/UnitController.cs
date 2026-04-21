using System;
using UnityEngine;


public class UnitController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 destination;
    private bool hasDestination = false;

    [Header("Unit Class (optional — assigns a default if empty)")]
    [SerializeField] private UnitClassData unitClassData;

    private AbilityHolder abilityHolder;

    void Awake()
    {
        // Ensure this unit has an AbilityHolder component
        abilityHolder = GetComponent<AbilityHolder>();
        if (abilityHolder == null)
        {
            abilityHolder = gameObject.AddComponent<AbilityHolder>();
        }

        // If no class was assigned in the inspector, default to Warrior for testing
        if (unitClassData == null)
        {
            unitClassData = UnitClassDefaults.CreateWarrior();
            Debug.Log("No UnitClassData assigned — defaulting to Warrior.");
        }

        abilityHolder.Initialize(unitClassData);
    }

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
