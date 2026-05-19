using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] //required for OnMouseDown to work

public class CharacterSelector : MonoBehaviour
{
    [Header("Selection Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.yellow;
    private SpriteRenderer _spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.color = _normalColor;
        
    }
    public void Select()
    {
        _spriteRenderer.color = _selectedColor;
        Debug.Log(gameObject.name + " selected.");
        // I think this is where we add code to display UI abilites
    }
    public void Deselect()
    {
         _spriteRenderer.color = _normalColor;
        Debug.Log(gameObject.name + " deselected.");
    }
}
