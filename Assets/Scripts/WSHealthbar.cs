using UnityEngine;
using UnityEngine.UI;

public class WSHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    private Character character;
    private float fullWidth;

    void Awake()
    {
        character = GetComponentInParent<Character>();
        fill = transform.Find("BarOutline/BarFill").GetComponent<Image>();
        fullWidth = fill.rectTransform.sizeDelta.x;
    }

    void LateUpdate()
    {
        Debug.Log($"Updating health bar for {character.GetCharacterName()}: {character.CurrentHealth}/{character.MaxHealth}");
        float parentScaleX = character.transform.localScale.x;
        transform.localScale = new Vector3(
        Mathf.Sign(parentScaleX) / Mathf.Abs(parentScaleX), 1f, 1f);
        float pct = character.CurrentHealth / character.MaxHealth;
        fill.rectTransform.sizeDelta = new Vector2(pct * fullWidth, fill.rectTransform.sizeDelta.y);
        Debug.Log($"Health bar fill amount: {fill.fillAmount}");
    }
}