using UnityEngine;

// Abstraction for enemy movement/pathfinding strategies.
// Melee enemies use DirectPursuitPath (beeline toward target).
// Ranged enemies will use KeepDistancePath (maintain range from target).
// Bosses might use custom patterns.
// Swap in A*, NavMesh, or any real pathfinding later by implementing
// this interface — no enemy code needs to change.
public interface IPathProvider
{
    // Given the enemy's current position and the target position,
    // return the direction the enemy should move this frame.
    // Returns Vector2.zero if the enemy should stop moving.
    Vector2 GetMoveDirection(Vector2 currentPosition, Vector2 targetPosition, float stoppingDistance);
}

// Simplest pathfinding: move directly toward the target.
// Good enough for melee enemies in open arenas.
public class DirectPursuitPath : IPathProvider
{
    public Vector2 GetMoveDirection(Vector2 currentPosition, Vector2 targetPosition, float stoppingDistance)
    {
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance <= stoppingDistance)
        {
            return Vector2.zero; // Close enough, stop
        }

        return (targetPosition - currentPosition).normalized;
    }
}

// Placeholder for ranged enemies: approach until within preferred range,
// then stop (or back up if too close). Implement fully when ranged enemies are built.
public class KeepDistancePath : IPathProvider
{
    private float preferredRange;
    private float tolerance;

    public KeepDistancePath(float preferredRange, float tolerance = 0.5f)
    {
        this.preferredRange = preferredRange;
        this.tolerance = tolerance;
    }

    public Vector2 GetMoveDirection(Vector2 currentPosition, Vector2 targetPosition, float stoppingDistance)
    {
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance > preferredRange + tolerance)
        {
            // Too far — move closer
            return (targetPosition - currentPosition).normalized;
        }
        else if (distance < preferredRange - tolerance)
        {
            // Too close — back away
            return (currentPosition - targetPosition).normalized;
        }

        return Vector2.zero;
    }
}
