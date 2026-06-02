using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] //required for OnMouseDown to work

public class CharacterSelector : MonoBehaviour
{
    [Header("Selection Colors")]
    [SerializeField] private Color _selectedColor = Color.yellow;
    private SpriteRenderer _spriteRenderer;
    private GameObject highlight;
    // private Color _baseTint; // stores the hero's tint so we can restore it on deselect

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        highlight = transform.Find("SelectionHighlight")?.gameObject;
        // Remember whatever tint the hero already has (set by Hero.Start from HeroData)
        // _baseTint = _spriteRenderer.color;
    }
    public void Select()
    {
        // _spriteRenderer.color = _selectedColor;
        highlight.SetActive(true);
        Debug.Log(gameObject.name + " selected.");
    }
    public void Deselect()
    {
        // _spriteRenderer.color = _baseTint;
        highlight.SetActive(false);
        Debug.Log(gameObject.name + " deselected.");
    }
}
