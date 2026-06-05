using UnityEngine;

[RequireComponent(typeof(Collider2D))] //required for OnMouseDown to work

public class CharacterSelector : MonoBehaviour
{
    [Header("Selection Colors")]
    [SerializeField] private Color _selectedColor;
    private SpriteRenderer _mainSpriteRenderer;
    private GameObject highlight;
    private SpriteRenderer[] highlightRenderers;

    void Start()
    {
        _mainSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Transform highlightTransform = transform.Find("SelectionHighlight");
        if (highlightTransform != null)
        {
            highlight = highlightTransform.gameObject;
            // The highlight object itself might not have the renderer, but its children (Outline, Circle) do
            highlightRenderers = highlight.GetComponentsInChildren<SpriteRenderer>(true);
        }
    }

    public void Select()
    {
        if (highlight != null)
        {
            highlight.SetActive(true);
            UpdateHighlightColor();
        }
        Debug.Log(gameObject.name + " selected.");
    }

    public void Deselect()
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }
        Debug.Log(gameObject.name + " deselected.");
    }

    private void UpdateHighlightColor()
    {
        if (highlightRenderers == null || highlightRenderers.Length == 0) return;

        // Default to the standard color (Yellow)
        Color targetColor = _selectedColor;

        // Switch to Blue if colorblind mode is active
        if (GameManager.Instance != null && GameManager.Instance.isColorblindMode)
        {
            targetColor = Color.yellow;
        }

        foreach (var renderer in highlightRenderers)
        {
            if (renderer != null)
            {
                // Preserve the original alpha of the specific part (Circle vs Outline)
                renderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, renderer.color.a);
            }
        }
    }
}
