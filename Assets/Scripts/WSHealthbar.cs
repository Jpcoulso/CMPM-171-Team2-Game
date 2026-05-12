using UnityEngine;
using UnityEngine.UI;

public class WSHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    private Character character;

    void Awake()
    {
        character = GetComponentInParent<Character>();
        fill = transform.Find("BarOutline/BarFill").GetComponent<Image>();
    }

    void LateUpdate()
    {
        Debug.Log($"Updating health bar for {character.GetCharacterName()}: {character.CurrentHealth}/{character.MaxHealth}");
        fill.fillAmount = character.CurrentHealth / character.MaxHealth;
    }
}