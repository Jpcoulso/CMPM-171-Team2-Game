using UnityEngine;

// Requires a Collider2D on the same GameObject for OnMouseDown/Up/Drag to fire
[RequireComponent(typeof(Collider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    // How close to the destination before the character stops
    public float arrivalThreshold = 0.1f;

    [Header("Visual Feedback")]
    // Tint applied when this character is selected
    public Color selectedColor = new Color(0.6f, 1f, 0.6f);
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    [Header("Drag Line")]
    // Assign the child LineRenderer object here in the Inspector
    public LineRenderer dragLine;

    private Vector2 targetPosition;
    private bool isMoving = false;

    // Are we currently being dragged from?
    private bool isDragging = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Start at our own position so we don't slide on Start
        targetPosition = transform.position;

        // Hide the drag line by default
        if (dragLine != null)
            dragLine.enabled = false;
    }

    void Update()
    {
        // Move toward the target position each frame
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Stop once we arrive
            if (Vector2.Distance(transform.position, targetPosition) < arrivalThreshold)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        // While dragging, update the preview line from this character to the mouse
        if (isDragging && dragLine != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            dragLine.SetPosition(0, transform.position);
            dragLine.SetPosition(1, mouseWorld);
        }
    }

    // --- Mouse Events (require Collider2D on this GameObject) ---

    // Fired when the player clicks down ON this character
    void OnMouseDown()
    {
        // Select this character
        GameManager.Instance.SelectCharacter(this);

        // Begin a drag
        isDragging = true;

        if (dragLine != null)
        {
            dragLine.enabled = true;
            dragLine.SetPosition(0, transform.position);
            dragLine.SetPosition(1, transform.position);
        }
    }

    // Fired when the player releases the mouse button
    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // Hide the preview line
        if (dragLine != null)
            dragLine.enabled = false;

        // Only move if this character is the selected one
        if (GameManager.Instance.SelectedCharacter == this)
        {
            // Convert mouse screen position to world position
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            // Only send the character to the destination if the player
            // actually dragged a meaningful distance (avoids accidental moves on a click)
            float dragDistance = Vector2.Distance(transform.position, mouseWorld);
            if (dragDistance > 0.3f)
            {
                targetPosition = mouseWorld;
                isMoving = true;
            }
        }
    }

    // --- Called by GameManager ---

    public void OnSelected()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = selectedColor;
    }

    public void OnDeselected()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        // Stop any active drag if deselected externally
        isDragging = false;
        if (dragLine != null)
            dragLine.enabled = false;
    }

    // Optional: stop movement from another script (e.g. entering combat)
    public void StopMovement()
    {
        isMoving = false;
        targetPosition = transform.position;
    }
}