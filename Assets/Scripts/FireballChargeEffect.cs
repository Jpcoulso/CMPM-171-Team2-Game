using UnityEngine;
using UnityEngine.InputSystem;

// FireballChargeEffect.cs
// Attached to the hero during Fireball charge-up.
// Shows a growing red ring indicator at the cursor position.
// Destroyed when the charge is released.

public class FireballChargeEffect : MonoBehaviour
{
    private GameObject indicator;
    private SpriteRenderer indicatorSR;
    private float chargeTimer;
    private float maxChargeTime;
    private float minRadius;
    private float maxRadius;
    private Vector3 lockedPosition;

    public void Initialize(float maxCharge, float minRad, float maxRad)
    {
        maxChargeTime = maxCharge;
        minRadius = minRad;
        maxRadius = maxRad;
        chargeTimer = 0f;

        // Lock the target position to wherever the cursor is RIGHT NOW
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        lockedPosition = Camera.main.ScreenToWorldPoint(mouseScreen);
        lockedPosition.z = 0f;

        // Create the ring indicator at the locked position
        indicator = new GameObject("FireballChargeIndicator");
        indicatorSR = indicator.AddComponent<SpriteRenderer>();
        indicatorSR.sprite = AbilitySpriteCache.GetRing(64, 0.8f);
        indicatorSR.color = new Color(1f, 0.3f, 0f, 0.7f); // orange
        indicatorSR.sortingLayerName = "Foreground";
        indicatorSR.sortingOrder = 999;
        indicator.transform.position = lockedPosition;

        UpdateIndicator();
    }

    private void Update()
    {
        chargeTimer += Time.deltaTime;
        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        if (indicator == null) return;

        // Scale the ring based on charge time (lerp from min to max radius)
        float t = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float currentRadius = Mathf.Lerp(minRadius, maxRadius, t);
        float diameter = currentRadius * 2f;
        indicator.transform.localScale = new Vector3(diameter, diameter, 1f);

        // Pulse the alpha slightly so the player knows it's charging
        float alpha = 0.5f + 0.3f * Mathf.Sin(chargeTimer * 6f);
        indicatorSR.color = new Color(1f, 0.3f, 0f, alpha);
    }

    public Vector3 GetTargetPosition()
    {
        return lockedPosition;
    }

    public void Cleanup()
    {
        if (indicator != null)
            Destroy(indicator);
        Destroy(this); // remove just the component
    }

    private void OnDestroy()
    {
        // Safety cleanup in case the component is destroyed externally
        if (indicator != null)
            Destroy(indicator);
    }

}
