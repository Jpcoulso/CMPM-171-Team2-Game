using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Displays up to 3 unit frames at the top-center of the screen.
/// Each frame: portrait (left) | name + health bar + 4 ability icons (right).
/// Modifier labels (SHIFT+, CTRL+) shown on units 2 and 3.
/// Reads all data from Hero / AbilityHandler / HeroData / SquadManager.
/// Builds its own Canvas at runtime — just drop on any GameObject.
/// </summary>
public class TeamHUD : MonoBehaviour
{
    private UnitFrameUI[] unitFrames = new UnitFrameUI[3];
    private Hero[] trackedHeroes = new Hero[3];

    // ── Layout constants ──
    private const float FrameWidth  = 510f;
    private const float FrameHeight = 144f;
    private const float BorderSize  = 3f;
    private const float Pad         = 10f;
    private const float PortraitSize = 84f;
    private const float AbilitySlotSize = 64f;
    private const float AbilityGap  = 6f;
    private const float HealthBarH  = 18f;
    private const float NameH       = 24f;
    private const float ModLabelW   = 52f;
    private const float FrameGap    = 14f;

    // ── Colors ──
    private static readonly Color ColFrameBG       = new Color(0.08f, 0.08f, 0.10f, 0.95f);
    private static readonly Color ColBorderNormal   = new Color(0.25f, 0.25f, 0.30f, 0.8f);
    private static readonly Color ColBorderSelected = new Color(0.95f, 0.82f, 0.25f, 1f);
    private static readonly Color ColPortraitBG     = new Color(0.12f, 0.12f, 0.15f, 1f);
    private static readonly Color ColHealthBG       = new Color(0.10f, 0.10f, 0.10f, 1f);
    private static readonly Color ColHealthGreen    = new Color(0.20f, 0.78f, 0.35f, 1f);
    private static readonly Color ColHealthRed      = new Color(0.88f, 0.20f, 0.20f, 1f);
    private static readonly Color ColAbilityBG      = new Color(0.12f, 0.12f, 0.15f, 1f);
    private static readonly Color ColAbilityCoolBG  = new Color(0.04f, 0.04f, 0.04f, 0.85f);
    private static readonly Color ColAbilityBorder  = new Color(0.30f, 0.30f, 0.35f, 0.6f);
    private static readonly Color ColKeyLabel       = new Color(0.92f, 0.85f, 0.35f, 1f);
    private static readonly Color ColCDText         = new Color(1f, 1f, 1f, 0.95f);
    private static readonly Color ColName           = new Color(0.95f, 0.95f, 0.95f, 1f);
    private static readonly Color ColHPText         = new Color(1f, 1f, 1f, 0.9f);
    private static readonly Color ColEmpty          = new Color(0.08f, 0.08f, 0.08f, 0.5f);

    // ── Placeholder symbols ──
    private static readonly string[] PortraitSymbols = { "I", "II", "III" };

    // ══════════════════════════════════════════════════════════
    //  LIFECYCLE
    // ══════════════════════════════════════════════════════════

    void Awake()  { BuildUI(); }
    void Start()  { RefreshUnitList(); }

    void Update()
    {
        // Re-sync squad every frame in case heroes register late
        if (SquadManager.Instance != null)
        {
            var squad = SquadManager.Instance.GetSquad();
            for (int i = 0; i < 3; i++)
            {
                Hero hero = (i < squad.Count) ? squad[i] : null;

                // Detect if the hero at this slot changed
                if (trackedHeroes[i] != hero)
                {
                    trackedHeroes[i] = hero;
                    if (hero != null)
                    {
                        unitFrames[i].SetUnitName(hero.GetCharacterName());
                        unitFrames[i].SetIcons(hero.HeroDataAsset);
                        unitFrames[i].Show(true);
                    }
                    else
                    {
                        unitFrames[i].SetEmpty();
                        unitFrames[i].Show(false);
                    }
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (trackedHeroes[i] == null) { unitFrames[i].SetEmpty(); continue; }

            Hero hero = trackedHeroes[i];

            // Health from Character base class
            unitFrames[i].UpdateHealth(hero.CurrentHealth, hero.MaxHealth);

            // Abilities from AbilityHandler list
            var handlers = hero.AbilityHandlers;
            for (int s = 0; s < 4; s++)
            {
                AbilityHandler handler = (s < handlers.Count) ? handlers[s] : null;
                unitFrames[i].UpdateAbility(s, handler);
            }

            // Selection highlight
            bool sel = SelectionManager.Instance != null
                    && SelectionManager.Instance.currentlySelected != null
                    && SelectionManager.Instance.currentlySelected.gameObject == hero.gameObject;
            unitFrames[i].SetSelected(sel);
        }
    }

    public void RefreshUnitList()
    {
        if (SquadManager.Instance == null) return;

        var squad = SquadManager.Instance.GetSquad();
        for (int i = 0; i < 3; i++)
        {
            trackedHeroes[i] = (i < squad.Count) ? squad[i] : null;
            if (trackedHeroes[i] != null)
            {
                unitFrames[i].SetUnitName(trackedHeroes[i].GetCharacterName());
                unitFrames[i].SetIcons(trackedHeroes[i].HeroDataAsset);
                unitFrames[i].Show(true);
            }
            else
            {
                unitFrames[i].SetEmpty();
                unitFrames[i].Show(false);
            }
        }
    }

    // ══════════════════════════════════════════════════════════
    //  BUILD UI
    // ══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        var canvasGO = new GameObject("TeamHUDCanvas");
        canvasGO.transform.SetParent(transform);
        var canvas         = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var row   = NewGO("FrameRow", canvasGO.transform);
        var rowRT = row.AddComponent<RectTransform>();
        rowRT.anchorMin        = new Vector2(0.5f, 1f);
        rowRT.anchorMax        = new Vector2(0.5f, 1f);
        rowRT.pivot            = new Vector2(0.5f, 1f);
        rowRT.anchoredPosition = new Vector2(0f, -10f);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing                = FrameGap;
        hlg.childAlignment         = TextAnchor.UpperCenter;
        hlg.childControlWidth      = false;
        hlg.childControlHeight     = false;
        hlg.childForceExpandWidth  = false;
        hlg.childForceExpandHeight = false;

        var fitter = row.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        for (int i = 0; i < 3; i++)
            unitFrames[i] = BuildFrame(row.transform, i);
    }

    private UnitFrameUI BuildFrame(Transform parent, int idx)
    {
        var f = new UnitFrameUI();
        float totalW = FrameWidth + BorderSize * 2;
        float totalH = FrameHeight + BorderSize * 2;

        // Border
        var borderGO = Panel(parent, "UnitFrame_" + idx, totalW, totalH, ColBorderNormal);
        var le = borderGO.AddComponent<LayoutElement>();
        le.preferredWidth  = totalW;
        le.preferredHeight = totalH;
        f.border = borderGO.GetComponent<Image>();

        // Inner frame
        var frame = Panel(borderGO.transform, "Inner", 0, 0, ColFrameBG);
        StretchInside(frame, BorderSize);
        f.root = frame;

        // ── PORTRAIT ──
        var portrait = Panel(frame.transform, "Portrait", 0, 0, ColPortraitBG);
        var prt = portrait.GetComponent<RectTransform>();
        prt.anchorMin        = new Vector2(0f, 0.5f);
        prt.anchorMax        = new Vector2(0f, 0.5f);
        prt.pivot            = new Vector2(0f, 0.5f);
        prt.anchoredPosition = new Vector2(Pad, 0f);
        prt.sizeDelta        = new Vector2(PortraitSize, PortraitSize);
        f.portraitImage = portrait.GetComponent<Image>();

        var pTxt = Label(portrait.transform, "Sym", PortraitSymbols[idx], 36,
                         TextAnchor.MiddleCenter, new Color(0.5f, 0.5f, 0.6f, 0.6f));
        StretchFull(pTxt);
        f.portraitSymbol = pTxt.GetComponent<Text>();

        // ── RIGHT CONTENT ──
        float contentL = Pad + PortraitSize + Pad;

        // Name
        var nameGO = Label(frame.transform, "Name", "—", 18, TextAnchor.MiddleLeft, ColName);
        var nrt = nameGO.GetComponent<RectTransform>();
        nrt.anchorMin = new Vector2(0f, 1f);
        nrt.anchorMax = new Vector2(1f, 1f);
        nrt.offsetMin = new Vector2(contentL, -Pad - NameH);
        nrt.offsetMax = new Vector2(-Pad, -Pad);
        nameGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        f.nameText = nameGO.GetComponent<Text>();

        // Health bar
        float hpTop = Pad + NameH + 4f;
        var hpBG = Panel(frame.transform, "HPBG", 0, 0, ColHealthBG);
        var hrt = hpBG.GetComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0f, 1f);
        hrt.anchorMax = new Vector2(1f, 1f);
        hrt.offsetMin = new Vector2(contentL, -hpTop - HealthBarH);
        hrt.offsetMax = new Vector2(-Pad, -hpTop);

        var hpFill = Panel(hpBG.transform, "Fill", 0, 0, ColHealthGreen);
        StretchFull(hpFill);
        var fillImg = hpFill.GetComponent<Image>();
        fillImg.type       = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;
        f.healthFill = fillImg;

        var hpTxt = Label(hpBG.transform, "Text", "100/100", 12,
                          TextAnchor.MiddleCenter, ColHPText);
        StretchFull(hpTxt);
        hpTxt.GetComponent<Text>().fontStyle = FontStyle.Bold;
        f.healthText = hpTxt.GetComponent<Text>();

        // ── ABILITY ROW ──
        float abSlotTotal = AbilitySlotSize + 2f;
        float abRowW = abSlotTotal * 4 + AbilityGap * 3;
        float abY = Pad;

        bool hasModifier = (idx == 1 || idx == 2);
        float modOffset  = hasModifier ? ModLabelW : 0f;
        float abX = contentL + modOffset;

        var abRow = NewGO("AbilityRow", frame.transform);
        var abRT  = abRow.AddComponent<RectTransform>();
        abRT.anchorMin        = new Vector2(0f, 0f);
        abRT.anchorMax        = new Vector2(0f, 0f);
        abRT.pivot            = new Vector2(0f, 0f);
        abRT.anchoredPosition = new Vector2(abX, abY);
        abRT.sizeDelta        = new Vector2(abRowW, abSlotTotal);

        var abHLG = abRow.AddComponent<HorizontalLayoutGroup>();
        abHLG.spacing                = AbilityGap;
        abHLG.childAlignment         = TextAnchor.MiddleLeft;
        abHLG.childControlWidth      = false;
        abHLG.childControlHeight     = false;
        abHLG.childForceExpandWidth  = false;
        abHLG.childForceExpandHeight = false;

        if (hasModifier)
        {
            string modStr = idx == 1 ? "SHIFT+" : "CTRL+";
            var mod = Label(frame.transform, "Mod", modStr, 12,
                            TextAnchor.MiddleCenter, ColKeyLabel);
            var mrt = mod.GetComponent<RectTransform>();
            mrt.anchorMin        = new Vector2(0f, 0f);
            mrt.anchorMax        = new Vector2(0f, 0f);
            mrt.pivot            = new Vector2(0.5f, 0.5f);
            mrt.anchoredPosition = new Vector2(contentL + modOffset * 0.5f,
                                               abY + abSlotTotal * 0.5f);
            mrt.sizeDelta = new Vector2(ModLabelW, 16f);
            mod.GetComponent<Text>().fontStyle = FontStyle.Bold;
        }

        string[] keys = { "Q", "W", "E", "R" };
        for (int s = 0; s < 4; s++)
            f.abilitySlots[s] = BuildAbilitySlot(abRow.transform, s, keys[s]);

        return f;
    }

    private AbilitySlotUI BuildAbilitySlot(Transform parent, int slotIdx, string key)
    {
        var slot = new AbilitySlotUI();
        float outerSize = AbilitySlotSize + 2f;

        var outer = Panel(parent, "Slot_" + key, outerSize, outerSize, ColAbilityBorder);
        var outerLE = outer.AddComponent<LayoutElement>();
        outerLE.preferredWidth  = outerSize;
        outerLE.preferredHeight = outerSize;

        var inner = Panel(outer.transform, "BG", 0, 0, ColAbilityBG);
        StretchInside(inner, 1f);
        slot.background = inner.GetComponent<Image>();

        // Ability icon (full slot, hidden until sprite assigned)
        var iconGO = NewGO("Icon", inner.transform);
        iconGO.AddComponent<RectTransform>();
        StretchFull(iconGO);
        var iconImg = iconGO.AddComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.color   = Color.white;
        iconImg.enabled = false;
        slot.iconImage  = iconImg;

        // Placeholder symbol
        var symGO = Label(inner.transform, "Sym", key, 22,
                          TextAnchor.MiddleCenter, new Color(0.6f, 0.6f, 0.7f, 0.7f));
        StretchFull(symGO);
        slot.symbolText = symGO.GetComponent<Text>();

        // Cooldown overlay
        var cdOverlay = Panel(inner.transform, "CDOverlay", 0, 0, ColAbilityCoolBG);
        StretchFull(cdOverlay);
        var cdImg = cdOverlay.GetComponent<Image>();
        cdImg.type          = Image.Type.Filled;
        cdImg.fillMethod    = Image.FillMethod.Radial360;
        cdImg.fillOrigin    = 2;
        cdImg.fillClockwise = false;
        cdImg.fillAmount    = 0f;
        slot.cooldownOverlay = cdImg;

        // Cooldown text
        var cdTxt = Label(inner.transform, "CDText", "", 16,
                          TextAnchor.MiddleCenter, ColCDText);
        StretchFull(cdTxt);
        cdTxt.GetComponent<Text>().fontStyle = FontStyle.Bold;
        slot.cooldownText = cdTxt.GetComponent<Text>();

        // Key label (bottom-right)
        var keyLbl = Label(inner.transform, "Key", key, 12,
                           TextAnchor.LowerRight, ColKeyLabel);
        var krt = keyLbl.GetComponent<RectTransform>();
        krt.anchorMin = Vector2.zero;
        krt.anchorMax = Vector2.one;
        krt.offsetMin = new Vector2(0f, 2f);
        krt.offsetMax = new Vector2(-4f, 0f);
        keyLbl.GetComponent<Text>().fontStyle = FontStyle.Bold;
        slot.keyText = keyLbl.GetComponent<Text>();

        return slot;
    }

    // ══════════════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════════════

    private static GameObject NewGO(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static GameObject Panel(Transform parent, string name, float w, float h, Color col)
    {
        var go = NewGO(name, parent);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(w, h);
        go.AddComponent<Image>().color = col;
        return go;
    }

    private static GameObject Label(Transform parent, string name, string text, int fontSize,
                                     TextAnchor align, Color col)
    {
        var go = NewGO(name, parent);
        go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.text               = text;
        t.fontSize           = fontSize;
        t.alignment          = align;
        t.color              = col;
        t.font               = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow   = VerticalWrapMode.Overflow;
        return go;
    }

    private static void StretchFull(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private static void StretchInside(GameObject go, float inset)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }

    // ══════════════════════════════════════════════════════════
    //  DATA CLASSES
    // ══════════════════════════════════════════════════════════

    private class AbilitySlotUI
    {
        public Image background;
        public Image iconImage;
        public Image cooldownOverlay;
        public Text  symbolText;
        public Text  cooldownText;
        public Text  keyText;

        private bool hasIcon = false;

        public void SetIcon(Sprite sprite)
        {
            if (iconImage == null || sprite == null) return;
            iconImage.sprite        = sprite;
            iconImage.color         = Color.white;
            iconImage.enabled       = true;
            iconImage.preserveAspect = false; // fill the whole slot
            symbolText.text         = "";
            hasIcon = true;
        }

        /// <summary>
        /// Updates the slot display from an AbilityHandler (cooldown, icon tint).
        /// Pass null for empty/unassigned slots.
        /// </summary>
        public void UpdateState(AbilityHandler handler)
        {
            if (handler == null)
            {
                background.color           = ColEmpty;
                cooldownOverlay.fillAmount = 0f;
                cooldownText.text          = "";
                symbolText.color           = new Color(0.2f, 0.2f, 0.2f, 0.3f);
                if (iconImage != null) iconImage.color = new Color(0.2f, 0.2f, 0.2f, 0.3f);
                return;
            }

            if (!handler.IsOnCooldown)
            {
                // Ready — full brightness
                background.color           = hasIcon ? Color.clear : ColAbilityBG;
                cooldownOverlay.fillAmount = 0f;
                cooldownText.text          = "";
                symbolText.color           = Color.white;
                if (hasIcon) iconImage.color = Color.white;
            }
            else
            {
                // On cooldown — dimmed with radial sweep + timer text
                background.color = ColAbilityCoolBG;
                float cd = handler.Data.cooldownDuration;
                cooldownOverlay.fillAmount = cd > 0f ? handler.CooldownTimer / cd : 0f;
                float r = handler.CooldownTimer;
                cooldownText.text = r >= 10f ? Mathf.CeilToInt(r).ToString() : r.ToString("F1");
                symbolText.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                if (hasIcon) iconImage.color = new Color(0.4f, 0.4f, 0.4f, 0.7f);
            }
        }
    }

    private class UnitFrameUI
    {
        public GameObject root;
        public Image border;
        public Image portraitImage;
        public Text  portraitSymbol;
        public Text  nameText;
        public Image healthFill;
        public Text  healthText;
        public AbilitySlotUI[] abilitySlots = new AbilitySlotUI[4];

        public void SetUnitName(string n) { nameText.text = n; }

        /// <summary>
        /// Sets portrait and ability icons from the hero's ScriptableObject data.
        /// </summary>
        public void SetIcons(HeroData data)
        {
            if (data == null) return;

            // Portrait
            if (data.portrait != null)
            {
                portraitImage.sprite = data.portrait;
                portraitImage.color  = Color.white;
                portraitSymbol.text  = "";
            }

            // Ability icons (Q=0, W=1, E=2, R=3)
            for (int i = 0; i < 4 && i < data.abilities.Count; i++)
            {
                if (data.abilities[i] != null && data.abilities[i].icon != null)
                    abilitySlots[i].SetIcon(data.abilities[i].icon);
            }
        }

        public void Show(bool visible) { border.gameObject.SetActive(visible); }

        public void SetEmpty()
        {
            nameText.text = "—";
            healthFill.fillAmount = 0f;
            if (healthText != null) healthText.text = "";
            for (int i = 0; i < 4; i++) abilitySlots[i]?.UpdateState(null);
        }

        public void UpdateHealth(float cur, float max)
        {
            float p = max > 0f ? cur / max : 0f;
            healthFill.fillAmount = p;
            healthFill.color = p > 0.3f ? ColHealthGreen : ColHealthRed;
            if (healthText != null)
                healthText.text = Mathf.CeilToInt(cur) + "/" + Mathf.CeilToInt(max);
        }

        public void UpdateAbility(int i, AbilityHandler handler)
        {
            abilitySlots[i]?.UpdateState(handler);
        }

        public void SetSelected(bool sel)
        {
            border.color = sel ? ColBorderSelected : ColBorderNormal;
        }
    }
}
