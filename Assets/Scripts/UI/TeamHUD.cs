using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TeamHUD
// A single unit panel anchored to the bottom-center of the screen.
// Shows ONLY the currently selected unit: its icon + one clickable
// ability button per ability the unit has. Buttons display a radial
// cooldown overlay + countdown text and activate the ability on click.
//
// Drop this component on any GameObject in the combat scene. It builds
// its own Canvas at runtime — no scene wiring required.
public class TeamHUD : MonoBehaviour
{
    // ── Layout ──
    private const float PanelHeight   = 132f;
    private const float Pad           = 12f;
    private const float IconSize      = 96f;
    private const float AbilitySize   = 72f;
    private const float AbilityGap    = 8f;
    private const float BottomMargin  = 16f;

    // ── Colors ──
    private static readonly Color PanelBG     = new Color(0.06f, 0.06f, 0.08f, 0.92f);
    private static readonly Color PanelBorder = new Color(0.95f, 0.82f, 0.25f, 1f);
    private static readonly Color IconBG      = new Color(0.16f, 0.16f, 0.2f, 1f);
    private static readonly Color NameCol     = new Color(0.95f, 0.95f, 0.95f, 1f);
    private static readonly Color AbReadyBG   = new Color(0.20f, 0.26f, 0.36f, 1f);
    private static readonly Color AbCoolBG    = new Color(0.07f, 0.07f, 0.08f, 1f);
    private static readonly Color AbBorderCol = new Color(0.32f, 0.32f, 0.38f, 0.8f);
    private static readonly Color OverlayCol  = new Color(0.04f, 0.04f, 0.05f, 0.78f);
    private static readonly Color KeyCol      = new Color(0.95f, 0.85f, 0.35f, 1f);
    private static readonly Color CDTextCol   = new Color(1f, 1f, 1f, 0.97f);
    private static readonly Color SymReady    = new Color(0.88f, 0.9f, 0.96f, 0.9f);
    private static readonly Color SymCool     = new Color(0.45f, 0.45f, 0.5f, 0.6f);

    private static readonly string[] Keys = { "Q", "W", "E", "R", "T", "Y" };

    // So other systems (InputManager) can ask "is the cursor over the HUD?"
    public static TeamHUD Instance { get; private set; }

    // ── Runtime refs ──
    private Canvas canvas;
    private RectTransform panel;          // the bottom-center panel
    private Image panelImage;
    private Image iconImage;
    private Text iconFallback;            // shown if no portrait
    private Transform abilityRow;         // holds the ability buttons
    private readonly List<AbilitySlot> slots = new List<AbilitySlot>();

    private Hero currentHero;             // the hero the panel is currently built for

    void Awake()
    {
        Instance = this;
        BuildBaseUI();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        Debug.Log("[TeamHUD] Built. Waiting for a hero to be selected. " +
                  (SelectionManager.Instance == null
                      ? "WARNING: no SelectionManager in scene — selection will never reach the HUD."
                      : "SelectionManager OK."));
    }

    void Update()
    {
        Hero selected = GetSelectedHero();

        // Rebuild the panel only when the selected hero changes
        if (selected != currentHero)
        {
            currentHero = selected;
            if (currentHero == null) ShowPanel(false);
            else { BuildAbilityButtons(currentHero); ShowPanel(true); }
        }

        if (currentHero == null) return;

        // The hero could die while selected
        if (currentHero.IsDead)
        {
            currentHero = null;
            ShowPanel(false);
            return;
        }

        RefreshCooldowns();
    }

    private Hero GetSelectedHero()
    {
        if (SelectionManager.Instance == null) return null;
        var sel = SelectionManager.Instance.currentlySelected;
        if (sel == null) return null;
        return sel.GetComponent<Hero>();
    }

    // True if the cursor is currently over the visible HUD panel.
    // Used to stop HUD clicks from registering as world clicks (deselecting the hero).
    public bool IsPointerOverPanel(Vector2 screenPos)
    {
        if (panel == null || !panel.gameObject.activeInHierarchy) return false;
        // ScreenSpaceOverlay canvas → pass null camera.
        return RectTransformUtility.RectangleContainsScreenPoint(panel, screenPos, null);
    }

    // ═══════════════════════════════════════════════════════════
    //  BUILD — base canvas + panel (created once)
    // ═══════════════════════════════════════════════════════════

    private void BuildBaseUI()
    {
        var canvasGO = new GameObject("TeamHUDCanvas");
        canvasGO.transform.SetParent(transform);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // render above gameplay / map UI
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel border (bottom-center). Auto-sizes to its content. ──
        var borderGO = MakePanel(canvasGO.transform, "Panel", 0f, 0f, PanelBorder);
        panel = borderGO.GetComponent<RectTransform>();
        panel.anchorMin = new Vector2(0.5f, 0f);
        panel.anchorMax = new Vector2(0.5f, 0f);
        panel.pivot = new Vector2(0.5f, 0f);
        panel.anchoredPosition = new Vector2(0f, BottomMargin);
        var fitter = borderGO.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var hlg = borderGO.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(3, 3, 3, 3); // border thickness
        hlg.spacing = 0f;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // ── Inner background row: [ icon ][ ability buttons ] ──
        var innerGO = MakePanel(borderGO.transform, "Inner", 0f, PanelHeight, PanelBG);
        var innerHLG = innerGO.AddComponent<HorizontalLayoutGroup>();
        innerHLG.padding = new RectOffset((int)Pad, (int)Pad, (int)Pad, (int)Pad);
        innerHLG.spacing = Pad;
        innerHLG.childAlignment = TextAnchor.MiddleLeft;
        innerHLG.childControlWidth = true;
        innerHLG.childControlHeight = true;
        innerHLG.childForceExpandWidth = false;
        innerHLG.childForceExpandHeight = false;
        var innerFitter = innerGO.AddComponent<ContentSizeFitter>();
        innerFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        innerFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        panelImage = innerGO.GetComponent<Image>();

        // ── Unit icon ──
        var iconGO = MakePanel(innerGO.transform, "Icon", IconSize, IconSize, IconBG);
        var iconLE = iconGO.AddComponent<LayoutElement>();
        iconLE.preferredWidth = IconSize;
        iconLE.preferredHeight = IconSize;
        iconImage = iconGO.GetComponent<Image>();
        iconImage.preserveAspect = true;

        var fb = MakeText(iconGO.transform, "Fallback", "", 44, TextAnchor.MiddleCenter, NameCol);
        Stretch(fb);
        fb.GetComponent<Text>().fontStyle = FontStyle.Bold;
        iconFallback = fb.GetComponent<Text>();

        // ── Ability row container ──
        var rowGO = new GameObject("AbilityRow");
        rowGO.transform.SetParent(innerGO.transform, false);
        rowGO.AddComponent<RectTransform>();
        var rowHLG = rowGO.AddComponent<HorizontalLayoutGroup>();
        rowHLG.spacing = AbilityGap;
        rowHLG.childAlignment = TextAnchor.MiddleLeft;
        rowHLG.childControlWidth = false;
        rowHLG.childControlHeight = false;
        rowHLG.childForceExpandWidth = false;
        rowHLG.childForceExpandHeight = false;
        var rowFitter = rowGO.AddComponent<ContentSizeFitter>();
        rowFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        rowFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        abilityRow = rowGO.transform;

        ShowPanel(false);
    }

    // ═══════════════════════════════════════════════════════════
    //  BUILD — ability buttons (rebuilt per selected hero)
    // ═══════════════════════════════════════════════════════════

    private void BuildAbilityButtons(Hero hero)
    {
        // Clear old buttons
        slots.Clear();
        for (int i = abilityRow.childCount - 1; i >= 0; i--)
            Destroy(abilityRow.GetChild(i).gameObject);

        // Unit icon
        Sprite portrait = hero.HeroData != null ? hero.HeroData.portrait : null;
        if (portrait != null)
        {
            iconImage.sprite = portrait;
            iconImage.color = Color.white;
            iconFallback.text = "";
        }
        else
        {
            iconImage.color = IconBG;
            iconFallback.text = ShortName(hero.GetCharacterName());
        }

        // One button per ability handler (order matches ability slot index)
        AbilityHandler[] handlers = hero.GetComponents<AbilityHandler>();
        for (int s = 0; s < handlers.Length; s++)
        {
            string key = s < Keys.Length ? Keys[s] : "";
            slots.Add(CreateAbilityButton(hero, s, key, handlers[s]));
        }
    }

    private AbilitySlot CreateAbilityButton(Hero hero, int slotIndex, string key, AbilityHandler handler)
    {
        var slot = new AbilitySlot { handler = handler };

        // Border
        var borderGO = MakePanel(abilityRow, $"Ability_{key}", AbilitySize + 4f, AbilitySize + 4f, AbBorderCol);
        var le = borderGO.AddComponent<LayoutElement>();
        le.preferredWidth = AbilitySize + 4f;
        le.preferredHeight = AbilitySize + 4f;

        // Button background (this is the clickable target)
        var bgGO = MakePanel(borderGO.transform, "BG", AbilitySize, AbilitySize, AbReadyBG);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0.5f, 0.5f);
        bgRT.anchorMax = new Vector2(0.5f, 0.5f);
        bgRT.anchoredPosition = Vector2.zero;
        slot.background = bgGO.GetComponent<Image>();

        var btn = bgGO.AddComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;
        var cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
        cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;
        int captured = slotIndex;
        Hero capturedHero = hero;
        btn.onClick.AddListener(() => OnAbilityClicked(capturedHero, captured));
        slot.button = btn;

        // Ability icon
        var iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(bgGO.transform, false);
        var iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.12f, 0.12f);
        iconRT.anchorMax = new Vector2(0.88f, 0.88f);
        iconRT.sizeDelta = Vector2.zero;
        iconRT.anchoredPosition = Vector2.zero;
        var iconImg = iconGO.AddComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.raycastTarget = false;
        Sprite abIcon = handler != null && handler.Data != null ? handler.Data.icon : null;
        if (abIcon != null) { iconImg.sprite = abIcon; iconImg.color = Color.white; }
        else iconImg.enabled = false;
        slot.iconImage = iconImg;

        // Ability-key fallback letter (shown only when no icon)
        var sym = MakeText(bgGO.transform, "Sym", abIcon == null ? key : "", 26, TextAnchor.MiddleCenter, SymReady);
        Stretch(sym);
        sym.GetComponent<Text>().raycastTarget = false;
        sym.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 2f);
        slot.symbolText = sym.GetComponent<Text>();

        // Radial cooldown overlay
        var ovGO = MakePanel(bgGO.transform, "CDOverlay", 0f, 0f, OverlayCol);
        Stretch(ovGO);
        var ovImg = ovGO.GetComponent<Image>();
        ovImg.type = Image.Type.Filled;
        ovImg.fillMethod = Image.FillMethod.Radial360;
        ovImg.fillOrigin = 2; // top
        ovImg.fillClockwise = false;
        ovImg.fillAmount = 0f;
        ovImg.raycastTarget = false;
        slot.cooldownOverlay = ovImg;

        // Cooldown countdown text
        var cd = MakeText(bgGO.transform, "CDText", "", 22, TextAnchor.MiddleCenter, CDTextCol);
        Stretch(cd);
        cd.GetComponent<Text>().fontStyle = FontStyle.Bold;
        cd.GetComponent<Text>().raycastTarget = false;
        slot.cooldownText = cd.GetComponent<Text>();

        // Key label (bottom-right corner)
        var keyGO = MakeText(bgGO.transform, "Key", key, 16, TextAnchor.LowerRight, KeyCol);
        var keyRT = keyGO.GetComponent<RectTransform>();
        keyRT.anchorMin = Vector2.zero;
        keyRT.anchorMax = Vector2.one;
        keyRT.offsetMin = new Vector2(0f, 3f);
        keyRT.offsetMax = new Vector2(-4f, 0f);
        keyGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        keyGO.GetComponent<Text>().raycastTarget = false;

        return slot;
    }

    private void OnAbilityClicked(Hero hero, int slotIndex)
    {
        if (hero == null || hero.IsDead) return;
        hero.UseAbility(slotIndex);
    }

    // ═══════════════════════════════════════════════════════════
    //  UPDATE — cooldown visuals
    // ═══════════════════════════════════════════════════════════

    private void RefreshCooldowns()
    {
        foreach (var slot in slots)
        {
            AbilityHandler h = slot.handler;
            if (h == null) continue;

            if (!h.IsOnCooldown)
            {
                slot.background.color = AbReadyBG;
                slot.cooldownOverlay.fillAmount = 0f;
                slot.cooldownText.text = "";
                if (slot.symbolText != null) slot.symbolText.color = SymReady;
            }
            else
            {
                slot.background.color = AbCoolBG;
                float dur = h.Data != null ? Mathf.Max(0.1f, h.Data.cooldownDuration) : 1f;
                slot.cooldownOverlay.fillAmount = Mathf.Clamp01(h.CooldownTimer / dur);
                float r = h.CooldownTimer;
                slot.cooldownText.text = r >= 10f ? Mathf.CeilToInt(r).ToString() : r.ToString("F1");
                if (slot.symbolText != null) slot.symbolText.color = SymCool;
            }
        }
    }

    private void ShowPanel(bool visible)
    {
        if (panel != null) panel.gameObject.SetActive(visible);
    }

    // ═══════════════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════════════

    private GameObject MakePanel(Transform parent, string name, float w, float h, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(w, h);
        go.AddComponent<Image>().color = col;
        return go;
    }

    private GameObject MakeText(Transform parent, string name, string content, int size, TextAnchor align, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.text = content;
        t.fontSize = size;
        t.alignment = align;
        t.color = col;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return go;
    }

    private void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private string ShortName(string n)
    {
        if (string.IsNullOrEmpty(n)) return "?";
        return n.Substring(0, 1).ToUpper();
    }

    // ═══════════════════════════════════════════════════════════
    //  DATA
    // ═══════════════════════════════════════════════════════════

    private class AbilitySlot
    {
        public AbilityHandler handler;
        public Button button;
        public Image background;
        public Image iconImage;
        public Image cooldownOverlay;
        public Text cooldownText;
        public Text symbolText;
    }
}
