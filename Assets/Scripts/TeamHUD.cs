using UnityEngine;
using UnityEngine.UI;

// Displays 3 unit frames at the top-center of the screen.
// Each frame: icon (left) | name + 4 ability slots (right).
// Uses Unity layout groups so everything spaces correctly.
//
// Drop on any GameObject. Builds its own Canvas at runtime.
public class TeamHUD : MonoBehaviour
{
    private UnitFrameUI[] unitFrames = new UnitFrameUI[3];
    private Character[] trackedUnits = new Character[3];

    // Sizing — scaled up ~1.5x from original
    private const float FrameWidth = 510f;
    private const float FrameHeight = 144f;
    private const float Pad = 10f;
    private const float IconSize = 84f;
    private const float AbilitySize = 64f;
    private const float NameHeight = 24f;

    // Colors
    private static readonly Color FrameBG = new Color(0.06f, 0.06f, 0.08f, 0.95f);
    private static readonly Color SelectedBorder = new Color(0.95f, 0.82f, 0.25f, 1f);
    private static readonly Color NormalBorder = new Color(0.22f, 0.22f, 0.28f, 0.8f);
    private static readonly Color IconBG = new Color(0.16f, 0.16f, 0.2f, 1f);
    private static readonly Color IconSymCol = new Color(0.65f, 0.7f, 0.8f, 1f);
    private static readonly Color NameCol = new Color(0.92f, 0.92f, 0.92f, 1f);
    private static readonly Color AbReadyBG = new Color(0.18f, 0.22f, 0.3f, 1f);
    private static readonly Color AbCoolBG = new Color(0.06f, 0.06f, 0.06f, 0.92f);
    private static readonly Color AbBorderCol = new Color(0.3f, 0.3f, 0.35f, 0.5f);
    private static readonly Color KeyCol = new Color(0.92f, 0.85f, 0.35f, 1f);
    private static readonly Color CDTextCol = new Color(1f, 1f, 1f, 0.95f);
    private static readonly Color SymReady = new Color(0.82f, 0.82f, 0.88f, 0.85f);
    private static readonly Color SymCool = new Color(0.35f, 0.35f, 0.35f, 0.45f);
    private static readonly Color EmptySlot = new Color(0.1f, 0.1f, 0.1f, 0.5f);

    // Placeholder symbols
    private static readonly string[,] AbSymbols =
    {
        { "⚔", "☠", "⚡", "⭐" },
        { "☄", "❄", "⚛", "☀" },
        { "➳", "◆", "⇨", "☂" },
    };
    private static readonly string[] IconSymbols = { "♣", "♥", "♠" };

    void Awake() { BuildUI(); }
    void Start() { RefreshUnitList(); }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (trackedUnits[i] == null) { unitFrames[i].SetEmpty(); continue; }

            Character unit = trackedUnits[i];
            AbilityHolder holder = unit.GetComponent<AbilityHolder>();

            for (int s = 0; s < 4; s++)
                unitFrames[i].UpdateAbilitySlot(s, holder?.GetAbility(s));

            bool sel = SelectionManager.Instance != null
                && SelectionManager.Instance.currentlySelected != null
                && SelectionManager.Instance.currentlySelected.gameObject == unit.gameObject;
            unitFrames[i].SetSelected(sel);
        }
    }

    public void RefreshUnitList()
    {
        CharacterSelector[] all = FindObjectsByType<CharacterSelector>(FindObjectsSortMode.None);
        for (int i = 0; i < 3; i++)
        {
            trackedUnits[i] = i < all.Length ? all[i].GetComponent<Character>() : null;
            if (trackedUnits[i] != null)
            {
                var hero = trackedUnits[i] as Hero;
            unitFrames[i].SetUnitName(trackedUnits[i].GetCharacterName());
            if (hero != null) unitFrames[i].SetIcons(hero.HeroData);
            unitFrames[i].Show(true);
            }
            else
            {
                unitFrames[i].SetEmpty();
                unitFrames[i].Show(false); // hide frames with no unit
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  BUILD
    // ═══════════════════════════════════════════════════════════

    private void BuildUI()
    {
        // Canvas
        var canvasGO = new GameObject("TeamHUDCanvas");
        canvasGO.transform.SetParent(transform);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Top-center row container
        var row = new GameObject("FrameRow");
        row.transform.SetParent(canvasGO.transform, false);
        var rowRT = row.AddComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0.5f, 1f);
        rowRT.anchorMax = new Vector2(0.5f, 1f);
        rowRT.pivot = new Vector2(0.5f, 1f);
        rowRT.anchoredPosition = new Vector2(0f, -10f);
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 14f;
        hlg.childAlignment = TextAnchor.UpperCenter;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        var rowFitter = row.AddComponent<ContentSizeFitter>();
        rowFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        rowFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        for (int i = 0; i < 3; i++)
            unitFrames[i] = CreateUnitFrame(row.transform, i);
    }

    private UnitFrameUI CreateUnitFrame(Transform parent, int idx)
    {
        var f = new UnitFrameUI();

        // ── Outer border (this is what the top-level HLG sizes) ──
        var borderGO = MakePanel(parent, $"Border_{idx}", FrameWidth + 4f, FrameHeight + 4f, NormalBorder);
        borderGO.AddComponent<LayoutElement>().preferredWidth = FrameWidth + 4f;
        borderGO.GetComponent<LayoutElement>().preferredHeight = FrameHeight + 4f;
        f.border = borderGO.GetComponent<Image>();

        // ── Frame BG centered inside border ──
        var frameGO = MakePanel(borderGO.transform, "Frame", 0f, 0f, FrameBG);
        var frt = frameGO.GetComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = new Vector2(2f, 2f);
        frt.offsetMax = new Vector2(-2f, -2f);
        f.root = frameGO;

        // All children positioned with anchors — no layout groups inside the frame.

        // ── Icon (left side, vertically centered) ──
        var iconGO = MakePanel(frameGO.transform, "Icon", 0f, 0f, IconBG);
        var irt = iconGO.GetComponent<RectTransform>();
        irt.anchorMin = new Vector2(0f, 0.5f);
        irt.anchorMax = new Vector2(0f, 0.5f);
        irt.pivot = new Vector2(0f, 0.5f);
        irt.anchoredPosition = new Vector2(Pad, 0f);
        irt.sizeDelta = new Vector2(IconSize, IconSize);
        f.iconImage = iconGO.GetComponent<Image>();

        var iconTxt = MakeText(iconGO.transform, "Sym", IconSymbols[idx], 40, TextAnchor.MiddleCenter, IconSymCol);
        Stretch(iconTxt);
        f.iconSymbolText = iconTxt.GetComponent<Text>();

        // ── Right-side content area (anchored from icon edge to right edge) ──
        float contentLeft = Pad + IconSize + 8f; // left pad + icon + gap

        // Name (top of content area)
        var nameGO = MakeText(frameGO.transform, "Name", "—", 20, TextAnchor.MiddleLeft, NameCol);
        var nrt = nameGO.GetComponent<RectTransform>();
        nrt.anchorMin = new Vector2(0f, 1f);
        nrt.anchorMax = new Vector2(1f, 1f);
        nrt.pivot = new Vector2(0f, 1f);
        nrt.offsetMin = new Vector2(contentLeft, 0f);  // left
        nrt.offsetMax = new Vector2(-Pad, -Pad);       // right, top
        nrt.sizeDelta = new Vector2(nrt.sizeDelta.x, NameHeight);
        // Fix: set anchored height properly
        nrt.anchorMin = new Vector2(0f, 1f);
        nrt.anchorMax = new Vector2(1f, 1f);
        nrt.offsetMin = new Vector2(contentLeft, -Pad - NameHeight);
        nrt.offsetMax = new Vector2(-Pad, -Pad);
        nameGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        f.nameText = nameGO.GetComponent<Text>();

        // Ability row (bottom of content area, anchored to bottom)
        float abSlotFull = AbilitySize + 2f; // slot + border
        float abRowWidth = (abSlotFull * 4) + (4f * 3); // 4 slots + 3 gaps
        float abBottom = Pad;
        float abTop = abBottom + abSlotFull;

        // Modifier label width — shift ability row right on units 2 & 3
        float modLabelWidth = (idx == 1 || idx == 2) ? 52f : 0f;
        float abRowLeft = contentLeft + modLabelWidth;

        // Use an HLG only for the ability row — it's simple enough
        var abRow = new GameObject("AbilityRow");
        abRow.transform.SetParent(frameGO.transform, false);
        var art = abRow.AddComponent<RectTransform>();
        art.anchorMin = new Vector2(0f, 0f);
        art.anchorMax = new Vector2(0f, 0f);
        art.pivot = new Vector2(0f, 0f);
        art.anchoredPosition = new Vector2(abRowLeft, abBottom);
        art.sizeDelta = new Vector2(abRowWidth, abSlotFull);
        var abHLG = abRow.AddComponent<HorizontalLayoutGroup>();
        abHLG.spacing = 4f;
        abHLG.childAlignment = TextAnchor.MiddleLeft;
        abHLG.childControlWidth = false;
        abHLG.childControlHeight = false;
        abHLG.childForceExpandWidth = false;
        abHLG.childForceExpandHeight = false;

        // Modifier label for unit 2 (SHIFT+) and unit 3 (CTRL+)
        if (idx == 1 || idx == 2)
        {
            string modLabel = idx == 1 ? "SHIFT+" : "CTRL+";
            var modGO = MakeText(frameGO.transform, "ModLabel", modLabel, 14, TextAnchor.MiddleCenter, KeyCol);
            var modRT = modGO.GetComponent<RectTransform>();
            modRT.anchorMin = new Vector2(0f, 0f);
            modRT.anchorMax = new Vector2(0f, 0f);
            modRT.pivot = new Vector2(0.5f, 0.5f);
            // Centered in the gap between icon and ability row
            modRT.anchoredPosition = new Vector2(contentLeft + modLabelWidth * 0.5f, abBottom + abSlotFull * 0.5f);
            modRT.sizeDelta = new Vector2(modLabelWidth, 14f);
            modGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        }

        string[] keys = { "Q", "W", "E", "R" };
        for (int s = 0; s < 4; s++)
            f.abilitySlots[s] = CreateAbilitySlot(abRow.transform, idx, s, keys[s]);

        return f;
    }

    private AbilitySlotData CreateAbilitySlot(Transform parent, int unitIdx, int slotIdx, string key)
    {
        var slot = new AbilitySlotData();

        // Border
        var borderGO = MakePanel(parent, $"Ab_{key}", AbilitySize + 2f, AbilitySize + 2f, AbBorderCol);
        borderGO.AddComponent<LayoutElement>().preferredWidth = AbilitySize + 2f;
        borderGO.GetComponent<LayoutElement>().preferredHeight = AbilitySize + 2f;

        // BG
        var bgGO = MakePanel(borderGO.transform, "BG", AbilitySize, AbilitySize, AbReadyBG);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0.5f, 0.5f);
        bgRT.anchorMax = new Vector2(0.5f, 0.5f);
        bgRT.anchoredPosition = Vector2.zero;
        slot.background = bgGO.GetComponent<Image>();

        // Sprite icon (hidden by default, shown when SetIcon is called)
        var iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(bgGO.transform, false);
        var iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.1f, 0.1f);
        iconRT.anchorMax = new Vector2(0.9f, 0.9f);
        iconRT.sizeDelta = Vector2.zero;
        iconRT.anchoredPosition = Vector2.zero;
        var iconImg = iconGO.AddComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.enabled = false; // hidden until a sprite is assigned
        slot.iconImage = iconImg;

        // Symbol
        string sym = AbSymbols[unitIdx % 3, slotIdx];
        var symGO = MakeText(bgGO.transform, "Sym", sym, 28, TextAnchor.MiddleCenter, SymReady);
        Stretch(symGO);
        symGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 2f);
        slot.symbolText = symGO.GetComponent<Text>();

        // Cooldown overlay
        var ovGO = MakePanel(bgGO.transform, "CDOverlay", 0f, 0f, AbCoolBG);
        Stretch(ovGO);
        var ovImg = ovGO.GetComponent<Image>();
        ovImg.type = Image.Type.Filled;
        ovImg.fillMethod = Image.FillMethod.Radial360;
        ovImg.fillOrigin = 2;
        ovImg.fillClockwise = false;
        ovImg.fillAmount = 0f;
        slot.cooldownOverlay = ovImg;

        // CD text
        var cdGO = MakeText(bgGO.transform, "CDText", "", 18, TextAnchor.MiddleCenter, CDTextCol);
        Stretch(cdGO);
        cdGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        slot.cooldownText = cdGO.GetComponent<Text>();

        // Key label (bottom-right)
        var keyGO = MakeText(bgGO.transform, "Key", key, 14, TextAnchor.LowerRight, KeyCol);
        var keyRT = keyGO.GetComponent<RectTransform>();
        keyRT.anchorMin = Vector2.zero;
        keyRT.anchorMax = Vector2.one;
        keyRT.offsetMin = new Vector2(0f, 2f);
        keyRT.offsetMax = new Vector2(-3f, 0f);
        keyGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        slot.keyText = keyGO.GetComponent<Text>();

        return slot;
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

    // ═══════════════════════════════════════════════════════════
    //  DATA CLASSES
    // ═══════════════════════════════════════════════════════════

    private class AbilitySlotData
    {
        public Image background;
        public Image cooldownOverlay;
        public Image iconImage;     // assigned sprite icon (if any)
        public Text keyText;
        public Text cooldownText;
        public Text symbolText;

        public void SetIcon(Sprite sprite)
        {
            if (iconImage != null && sprite != null)
            {
                iconImage.sprite = sprite;
                iconImage.color = Color.white;
                iconImage.enabled = true;
                symbolText.text = ""; // hide placeholder symbol
            }
        }

        public void UpdateState(Ability ability)
        {
            if (ability == null)
            {
                background.color = EmptySlot;
                cooldownOverlay.fillAmount = 0f;
                cooldownText.text = "";
                symbolText.color = new Color(0.25f, 0.25f, 0.25f, 0.3f);
                return;
            }

            if (ability.IsReady)
            {
                background.color = AbReadyBG;
                cooldownOverlay.fillAmount = 0f;
                cooldownText.text = "";
                symbolText.color = SymReady;
            }
            else
            {
                background.color = AbCoolBG;
                cooldownOverlay.fillAmount = ability.CooldownRemaining / ability.cooldownDuration;
                float r = ability.CooldownRemaining;
                cooldownText.text = r >= 10f ? Mathf.CeilToInt(r).ToString() : r.ToString("F1");
                symbolText.color = SymCool;
            }
        }
    }

    private class UnitFrameUI
    {
        public GameObject root;
        public Image border;
        public Image iconImage;
        public Text iconSymbolText;
        public Text nameText;
        public AbilitySlotData[] abilitySlots = new AbilitySlotData[4];

        public void SetUnitName(string n) { nameText.text = n; }

        public void SetIcons(HeroData data)
        {
            if (data == null) return;

            // Unit portrait
            if (data.portrait != null)
            {
                iconImage.sprite = data.portrait;
                iconImage.color = Color.white;
                iconSymbolText.text = ""; // hide placeholder symbol
            }

            // Ability icons
            Sprite[] abIcons = { data.qIcon };
            for (int i = 0; i < 4; i++)
            {
                if (abIcons[i] != null)
                {
                    abilitySlots[i].SetIcon(abIcons[i]);
                }
            }
        }

        // Show or hide the entire frame (border is the top-level GO)
        public void Show(bool visible)
        {
            border.gameObject.SetActive(visible);
        }

        public void SetEmpty()
        {
            nameText.text = "—";
            for (int i = 0; i < 4; i++) abilitySlots[i]?.UpdateState(null);
        }

        public void UpdateAbilitySlot(int i, Ability a) { abilitySlots[i]?.UpdateState(a); }

        public void SetSelected(bool s)
        {
            border.color = s ? SelectedBorder : NormalBorder;
        }
    }
}
