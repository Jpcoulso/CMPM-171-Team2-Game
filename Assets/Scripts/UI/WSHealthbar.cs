using UnityEngine;
using UnityEngine.UI;

public class WSHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    private Character character;
    private float fullWidth;
    private Color defaultColor;

    void Awake()
    {
        character = GetComponentInParent<Character>();
        fill = transform.Find("BarOutline/BarFill").GetComponent<Image>();
        fullWidth = fill.rectTransform.sizeDelta.x;
        if (fill != null)
        {
            defaultColor = fill.color;
        }
    }

    void LateUpdate()
    {
        float parentScaleX = character.transform.localScale.x;
        transform.localScale = new Vector3(
            Mathf.Sign(parentScaleX) / Mathf.Abs(parentScaleX), 1f, 1f);
        
        UpdateColor();

        float pct = character.CurrentHealth / character.MaxHealth;
        fill.rectTransform.sizeDelta = new Vector2(pct * fullWidth, fill.rectTransform.sizeDelta.y);
    }

    private void UpdateColor()
    {
        if (GameManager.Instance != null && GameManager.Instance.isColorblindMode)
        {
            if (character is Hero)
            {
                fill.color = Color.yellow;
            }
            else if (character is Enemy)
            {
                fill.color = Color.blue;
            }
        }
        else if (fill != null)
        {
            fill.color = defaultColor;
        }
    }
}
