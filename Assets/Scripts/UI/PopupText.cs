using UnityEngine;
using TMPro;
using System.Collections;

public class ScreenText : MonoBehaviour
{
    public static ScreenText Instance;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private float deafultDuration = 2f;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        Instance = this;
        displayText.gameObject.SetActive(false);
    }

    public void Show(string message, float duration = -1f)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(ShowRoutine(message, duration < 0 ? deafultDuration : duration));
    }

    public void ShowCountdown(string prefix, int seconds, System.Action onComplete = null)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(CountdownRoutine(prefix, seconds, onComplete));
    }

    public void Hide()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        displayText.gameObject.SetActive(false);
    }

    IEnumerator ShowRoutine(string message, float duration)
    {
        displayText.text = message;
        displayText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        displayText.gameObject.SetActive(false);
    }

    IEnumerator CountdownRoutine(string prefix, int seconds, System.Action onComplete)
    {
        displayText.gameObject.SetActive(true);
        for (int i = seconds; i > 0; i--)
        {
            displayText.text = $"{prefix} {i}...";
            yield return new WaitForSeconds(1f);
        }
        displayText.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}