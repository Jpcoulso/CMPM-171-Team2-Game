using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] //required for OnMouseDown to work

public class CharacterSelector : MonoBehaviour
{
    [Header("Selection Colors")]
    [SerializeField] private Color _selectedColor = Color.yellow;
    private SpriteRenderer _spriteRenderer;
    private Color _baseTint; // stores the hero's tint so we can restore it on deselect

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Remember whatever tint the hero already has (set by Hero.Start from HeroData)
        _baseTint = _spriteRenderer.color;
    }
    public void Select()
    {
        _spriteRenderer.color = _selectedColor;
        Debug.Log(gameObject.name + " selected.");
    }
    public void Deselect()
    {
        _spriteRenderer.color = _baseTint;
        Debug.Log(gameObject.name + " deselected.");
    }
}
