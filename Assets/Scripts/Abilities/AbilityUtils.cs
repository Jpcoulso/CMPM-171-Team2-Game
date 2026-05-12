using UnityEngine;
using UnityEngine.InputSystem;

// AbilityUtils.cs — Shared helpers for ability targeting.

public static class AbilityUtils
{
    // Returns a normalized direction from the unit toward the mouse cursor.
    // Falls back to Vector2.right if the camera or owner is missing.
    public static Vector2 GetAimDirection(GameObject owner)
    {
        if (Camera.main == null || owner == null)
            return Vector2.right;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = (Vector2)(mouseWorld - owner.transform.position);

        if (dir.sqrMagnitude < 0.01f)
            return Vector2.right;

        return dir.normalized;
    }
}
