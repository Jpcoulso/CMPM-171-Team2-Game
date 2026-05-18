using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Update()
    {
        goldText.text = $"{GameManager.Instance.currentGold}G";
    }
}