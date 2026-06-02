using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // Durumlar
    public enum AppState { Splash, Scanning, FishFound }
    private AppState currentState = AppState.Splash;

    // Bekleyen bilgi isteği (splash sırasında gelirse)
    private bool hasPendingFish = false;
    private string pendingName, pendingDesc;
    private bool isTransitioning = false;

    // Ana UI elemanları
    private Canvas mainCanvas;
    private CanvasGroup splashGroup, scanGroup, infoGroup;
    private GameObject splashPanel, scanPanel, infoPanel;
    private Text fishNameText, fishDescText, statusText, scanHintText;
    private Image scanCornerTL, scanCornerTR, scanCornerBL, scanCornerBR;
    private Image scanLine;
    private float scanLineY = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        BuildUI();
    }

    void Start() => StartCoroutine(ShowSplashThenScan());

    void Update()
    {
        if (currentState == AppState.Scanning)
            AnimateScanLine();
    }

    // ═══════════════════════════════════════════════════════════
    //  GENEL API
    // ═══════════════════════════════════════════════════════════

    /// <summary>Balık bulunduğunda çağır</summary>
    public void ShowFishInfo(string name, string description)
    {
        Debug.Log($"[UIManager] ShowFishInfo çağrıldı: {name}, durum: {currentState}");

        // Splash sırasında geldiyse beklet
        if (currentState == AppState.Splash)
        {
            hasPendingFish = true;
            pendingName = name;
            pendingDesc = description;
            return;
        }

        if (isTransitioning) return;

        // Zaten bir balık gösteriliyorsa, bilgiyi güncelle (geçiş animasyonu olmadan)
        if (currentState == AppState.FishFound)
        {
            fishNameText.text = name;
            fishDescText.text = description;
            return;
        }

        currentState = AppState.FishFound;
        fishNameText.text = name;
        fishDescText.text = description;
        StartCoroutine(TransitionTo(infoGroup, scanGroup));
    }

    /// <summary>Balık kaybolduğunda çağır</summary>
    public void HideFishInfo()
    {
        Debug.Log($"[UIManager] HideFishInfo çağrıldı, durum: {currentState}");
        hasPendingFish = false;
        if (currentState != AppState.FishFound || isTransitioning) return;
        currentState = AppState.Scanning;
        StartCoroutine(TransitionTo(scanGroup, infoGroup));
    }

    // ═══════════════════════════════════════════════════════════
    //  UI OLUŞTURMA
    // ═══════════════════════════════════════════════════════════

    void BuildUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("ModernUI_Canvas");
        canvasObj.transform.SetParent(transform);
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        BuildSplash(canvasObj.transform);
        BuildScanOverlay(canvasObj.transform);
        BuildInfoPanel(canvasObj.transform);

        // Başlangıçta sadece splash görünsün
        splashGroup.alpha = 1; splashGroup.gameObject.SetActive(true);
        scanGroup.alpha = 0;   scanGroup.gameObject.SetActive(false);
        infoGroup.alpha = 0;   infoGroup.gameObject.SetActive(false);
    }

    // ── SPLASH EKRANI ────────────────────────────────────────

    void BuildSplash(Transform parent)
    {
        splashPanel = CreatePanel(parent, "SplashPanel");
        splashGroup = splashPanel.GetComponent<CanvasGroup>();
        var bg = splashPanel.GetComponent<Image>();
        bg.sprite = UITheme.CreateGradientPanel(64, 64, 0, UITheme.PrimaryDark, new Color32(2, 12, 30, 255));
        bg.type = Image.Type.Sliced;

        // İkon (daire)
        var iconObj = CreateChild(splashPanel.transform, "Icon", new Vector2(0.5f, 0.6f), new Vector2(120, 120));
        var iconImg = iconObj.AddComponent<Image>();
        iconImg.sprite = UITheme.CreateCircle(128, UITheme.Accent);
        iconObj.AddComponent<PulseAnim>();

        // İkon iç sembol (küçük daire)
        var innerIcon = CreateChild(iconObj.transform, "InnerIcon", new Vector2(0.5f, 0.5f), new Vector2(60, 60));
        var innerImg = innerIcon.AddComponent<Image>();
        innerImg.sprite = UITheme.CreateCircle(64, UITheme.PrimaryDark);

        // Başlık
        var titleObj = CreateChild(splashPanel.transform, "Title", new Vector2(0.5f, 0.45f), new Vector2(800, 60));
        var title = titleObj.AddComponent<Text>();
        SetupText(title, "🐠 DENİZALTI MACERALARI 🐙", UITheme.FontTitle, UITheme.TextPrimary, TextAnchor.MiddleCenter);
        title.fontStyle = FontStyle.Bold;

        // Alt başlık
        var subObj = CreateChild(splashPanel.transform, "Subtitle", new Vector2(0.5f, 0.39f), new Vector2(800, 40));
        var sub = subObj.AddComponent<Text>();
        SetupText(sub, "Kartını tut, canlıları keşfet!", UITheme.FontSubtitle, UITheme.Accent, TextAnchor.MiddleCenter);

        // Alt çizgi dekoratif
        var lineObj = CreateChild(splashPanel.transform, "Line", new Vector2(0.5f, 0.35f), new Vector2(200, 3));
        var lineImg = lineObj.AddComponent<Image>();
        lineImg.color = UITheme.Accent;

        // Yükleniyor
        var loadObj = CreateChild(splashPanel.transform, "Loading", new Vector2(0.5f, 0.2f), new Vector2(400, 30));
        var loadTxt = loadObj.AddComponent<Text>();
        SetupText(loadTxt, "🔍 Macera başlıyor...", UITheme.FontCaption, UITheme.TextMuted, TextAnchor.MiddleCenter);
    }

    // ── TARAMA OVERLAY ───────────────────────────────────────

    void BuildScanOverlay(Transform parent)
    {
        scanPanel = CreatePanel(parent, "ScanPanel");
        scanGroup = scanPanel.GetComponent<CanvasGroup>();
        var bg = scanPanel.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0); // Tamamen saydam - kamera görünsün

        // Üst durum çubuğu
        var topBar = CreateChild(scanPanel.transform, "TopBar", new Vector2(0.5f, 1f), Vector2.zero);
        var topRT = topBar.GetComponent<RectTransform>();
        topRT.anchorMin = new Vector2(0, 0.92f); topRT.anchorMax = new Vector2(1, 1);
        topRT.offsetMin = Vector2.zero; topRT.offsetMax = Vector2.zero;
        var topBg = topBar.AddComponent<Image>();
        topBg.sprite = UITheme.CreateGradientPanel(64, 64, 0, new Color32(8, 27, 51, 200), new Color32(8, 27, 51, 0));
        topBg.type = Image.Type.Sliced;

        // Uygulama adı (üstte)
        var appName = CreateChild(topBar.transform, "AppName", new Vector2(0.5f, 0.5f), new Vector2(600, 40));
        var appTxt = appName.AddComponent<Text>();
        SetupText(appTxt, "🐠 Denizaltı Macerası", UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleCenter);

        // Tarama köşeleri
        float cornerSize = 80f;
        float margin = 0.15f;
        scanCornerTL = CreateCorner(scanPanel.transform, "TL", new Vector2(margin, 0.7f), cornerSize, 0);
        scanCornerTR = CreateCorner(scanPanel.transform, "TR", new Vector2(1f - margin, 0.7f), cornerSize, 90);
        scanCornerBR = CreateCorner(scanPanel.transform, "BR", new Vector2(1f - margin, 0.4f), cornerSize, 180);
        scanCornerBL = CreateCorner(scanPanel.transform, "BL", new Vector2(margin, 0.4f), cornerSize, 270);

        // Tarama çizgisi
        var lineObj = CreateChild(scanPanel.transform, "ScanLine", new Vector2(0.5f, 0.55f), new Vector2(500, 3));
        scanLine = lineObj.AddComponent<Image>();
        scanLine.color = UITheme.ScanLine;

        // Tarama ipucu
        var hintBg = CreateChild(scanPanel.transform, "HintBg", new Vector2(0.5f, 0.3f), new Vector2(500, 50));
        var hintBgImg = hintBg.AddComponent<Image>();
        hintBgImg.sprite = UITheme.CreateRoundedRect(64, 64, 16, UITheme.GlassBackground);
        hintBgImg.type = Image.Type.Sliced;

        var hintObj = CreateChild(hintBg.transform, "HintText", new Vector2(0.5f, 0.5f), new Vector2(480, 40));
        scanHintText = hintObj.AddComponent<Text>();
        SetupText(scanHintText, "📷  Kartını kameraya göster!", UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleCenter);

        // Alt durum göstergesi
        var bottomBar = CreateChild(scanPanel.transform, "BottomBar", new Vector2(0.5f, 0f), Vector2.zero);
        var bottomRT = bottomBar.GetComponent<RectTransform>();
        bottomRT.anchorMin = new Vector2(0, 0); bottomRT.anchorMax = new Vector2(1, 0.12f);
        bottomRT.offsetMin = Vector2.zero; bottomRT.offsetMax = Vector2.zero;
        var bottomBg = bottomBar.AddComponent<Image>();
        bottomBg.sprite = UITheme.CreateGradientPanel(64, 64, 0, new Color32(8, 27, 51, 0), new Color32(8, 27, 51, 220));
        bottomBg.type = Image.Type.Sliced;

        // Durum noktası
        var dotObj = CreateChild(bottomBar.transform, "Dot", new Vector2(0.4f, 0.5f), new Vector2(12, 12));
        var dotImg = dotObj.AddComponent<Image>();
        dotImg.sprite = UITheme.CreateCircle(16, UITheme.Success);
        dotObj.AddComponent<PulseAnim>();

        var statusObj = CreateChild(bottomBar.transform, "Status", new Vector2(0.55f, 0.5f), new Vector2(300, 30));
        statusText = statusObj.AddComponent<Text>();
        SetupText(statusText, "🔍 Arıyorum...", UITheme.FontCaption, UITheme.TextSecondary, TextAnchor.MiddleLeft);
    }

    // ── BİLGİ PANELİ (Çocuklara Yönelik) ──────────────────

    void BuildInfoPanel(Transform parent)
    {
        infoPanel = CreatePanel(parent, "InfoPanel");
        infoGroup = infoPanel.GetComponent<CanvasGroup>();
        var bg = infoPanel.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0);

        // Üst durum çubuğu - eğlenceli
        var topBar2 = CreateChild(infoPanel.transform, "TopBar2", new Vector2(0.5f, 1f), Vector2.zero);
        var topRT2 = topBar2.GetComponent<RectTransform>();
        topRT2.anchorMin = new Vector2(0, 0.90f); topRT2.anchorMax = new Vector2(1, 1);
        topRT2.offsetMin = Vector2.zero; topRT2.offsetMax = Vector2.zero;
        var topBg2 = topBar2.AddComponent<Image>();
        topBg2.sprite = UITheme.CreateGradientPanel(64, 64, 0,
            new Color32(0, 180, 100, 220), new Color32(0, 130, 80, 0));
        topBg2.type = Image.Type.Sliced;

        var foundLabel = CreateChild(topBar2.transform, "FoundLabel", new Vector2(0.5f, 0.5f), new Vector2(700, 50));
        var foundTxt = foundLabel.AddComponent<Text>();
        SetupText(foundTxt, "🎉 Harika! Bir canlı buldun!", UITheme.FontSubtitle, Color.white, TextAnchor.MiddleCenter);
        foundTxt.fontStyle = FontStyle.Bold;

        // Alt bilgi kartı - daha büyük, çocuklar için
        var card = CreateChild(infoPanel.transform, "Card", new Vector2(0.5f, 0f), Vector2.zero);
        var cardRT = card.GetComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0.04f, 0.01f); cardRT.anchorMax = new Vector2(0.96f, 0.48f);
        cardRT.offsetMin = Vector2.zero; cardRT.offsetMax = Vector2.zero;
        var cardBg = card.AddComponent<Image>();
        cardBg.sprite = UITheme.CreateBorderedPanel(64, 64, 24,
            new Color32(15, 35, 70, 220),
            new Color32(255, 200, 50, 120), 3);
        cardBg.type = Image.Type.Sliced;

        // Renkli üst şerit (gökkuşağı efekti)
        var accentLine = CreateChild(card.transform, "AccentLine", new Vector2(0.5f, 1f), Vector2.zero);
        var accentRT = accentLine.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0.05f, 0.94f); accentRT.anchorMax = new Vector2(0.95f, 0.97f);
        accentRT.offsetMin = Vector2.zero; accentRT.offsetMax = Vector2.zero;
        var accentImg = accentLine.AddComponent<Image>();
        accentImg.sprite = UITheme.CreateGradientPanel(64, 8, 4,
            new Color32(255, 100, 100, 255), new Color32(100, 200, 255, 255));
        accentImg.type = Image.Type.Sliced;

        // Balık adı - büyük ve eğlenceli
        var nameObj = CreateChild(card.transform, "FishName", new Vector2(0.5f, 0.86f), new Vector2(700, 60));
        fishNameText = nameObj.AddComponent<Text>();
        SetupText(fishNameText, "🐟 Mavi Yüzgeçli Orkinos", UITheme.FontSubtitle,
            new Color32(255, 220, 80, 255), TextAnchor.MiddleCenter);
        fishNameText.fontStyle = FontStyle.Bold;

        // Eğlenceli etiket
        var tagObj = CreateChild(card.transform, "Tag", new Vector2(0.5f, 0.74f), new Vector2(280, 36));
        var tagBg = tagObj.AddComponent<Image>();
        tagBg.sprite = UITheme.CreateRoundedRect(64, 36, 16, new Color32(255, 100, 50, 60));
        tagBg.type = Image.Type.Sliced;
        var tagTxtObj = CreateChild(tagObj.transform, "TagText", new Vector2(0.5f, 0.5f), new Vector2(270, 32));
        var tagTxt = tagTxtObj.AddComponent<Text>();
        SetupText(tagTxt, "⚡ Süper Hızlı Yüzücü!", UITheme.FontCaption,
            new Color32(255, 180, 80, 255), TextAnchor.MiddleCenter);

        // Açıklama - büyük alan, çocuk dostu
        var descObj = CreateChild(card.transform, "Desc", new Vector2(0.5f, 0.42f), new Vector2(680, 200));
        fishDescText = descObj.AddComponent<Text>();
        SetupText(fishDescText,
            "🏎️ Okyanusun en hızlı yüzücüsü!\n" +
            "Saatte 80 km hızla yüzer — bir araba kadar hızlı!\n\n" +
            "📏 2 metre boyunda olabilir — belki senden bile uzun!",
            UITheme.FontBody, UITheme.TextPrimary, TextAnchor.UpperCenter);

        // Ses göstergesi - eğlenceli
        var audioObj = CreateChild(card.transform, "AudioStatus", new Vector2(0.5f, 0.08f), new Vector2(350, 36));
        var audioBg = audioObj.AddComponent<Image>();
        audioBg.sprite = UITheme.CreateRoundedRect(64, 36, 14, new Color32(0, 200, 150, 50));
        audioBg.type = Image.Type.Sliced;
        var audioTxtObj = CreateChild(audioObj.transform, "AudioText", new Vector2(0.5f, 0.5f), new Vector2(340, 32));
        var audioTxt = audioTxtObj.AddComponent<Text>();
        SetupText(audioTxt, "🔊 Dinle ve öğren!", UITheme.FontCaption,
            new Color32(100, 255, 200, 255), TextAnchor.MiddleCenter);
    }

    // ═══════════════════════════════════════════════════════════
    //  ANİMASYONLAR
    // ═══════════════════════════════════════════════════════════

    IEnumerator ShowSplashThenScan()
    {
        yield return new WaitForSeconds(2.5f);
        currentState = AppState.Scanning;
        yield return StartCoroutine(TransitionTo(scanGroup, splashGroup));

        // Splash sırasında balık tespit edildiyse hemen göster
        if (hasPendingFish)
        {
            hasPendingFish = false;
            ShowFishInfo(pendingName, pendingDesc);
        }
    }

    IEnumerator TransitionTo(CanvasGroup show, CanvasGroup hide)
    {
        isTransitioning = true;
        show.gameObject.SetActive(true);
        float t = 0;
        while (t < UITheme.AnimDuration)
        {
            t += Time.deltaTime;
            float p = t / UITheme.AnimDuration;
            float ease = p * p * (3f - 2f * p); // smoothstep
            hide.alpha = 1f - ease;
            show.alpha = ease;
            yield return null;
        }
        hide.alpha = 0; hide.gameObject.SetActive(false);
        show.alpha = 1;
        isTransitioning = false;
    }

    void AnimateScanLine()
    {
        scanLineY += Time.deltaTime * 0.15f;
        if (scanLineY > 1f) scanLineY = 0f;
        float y = Mathf.Lerp(0.4f, 0.7f, scanLineY);
        var rt = scanLine.rectTransform;
        rt.anchorMin = new Vector2(0.2f, y);
        rt.anchorMax = new Vector2(0.8f, y);
        scanLine.color = new Color(UITheme.ScanLine.r, UITheme.ScanLine.g, UITheme.ScanLine.b,
            Mathf.Sin(scanLineY * Mathf.PI) * 0.8f);
    }

    // ═══════════════════════════════════════════════════════════
    //  YARDIMCI METODLAR
    // ═══════════════════════════════════════════════════════════

    GameObject CreatePanel(Transform parent, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        obj.AddComponent<Image>();
        obj.AddComponent<CanvasGroup>();
        return obj;
    }

    GameObject CreateChild(Transform parent, string name, Vector2 anchor, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor;
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;
        return obj;
    }

    Image CreateCorner(Transform parent, string name, Vector2 anchor, float size, float rotation)
    {
        var obj = CreateChild(parent, "Corner_" + name, anchor, new Vector2(size, size));
        var img = obj.AddComponent<Image>();
        img.sprite = CreateCornerSprite();
        img.color = UITheme.Accent;
        obj.transform.localEulerAngles = new Vector3(0, 0, rotation);
        return img;
    }

    Sprite CreateCornerSprite()
    {
        int s = 64;
        int thickness = 4;
        int len = 32;
        Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        Color[] px = new Color[s * s];
        Color clear = new Color(0, 0, 0, 0);
        for (int i = 0; i < px.Length; i++) px[i] = clear;

        for (int x = 0; x < len; x++)
            for (int y = s - thickness; y < s; y++)
                px[y * s + x] = Color.white;

        for (int y = s - len; y < s; y++)
            for (int x = 0; x < thickness; x++)
                px[y * s + x] = Color.white;

        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0, 1));
    }

    void SetupText(Text t, string content, int size, Color color, TextAnchor align)
    {
        t.text = content;
        t.font = Font.CreateDynamicFontFromOSFont("Arial", size);
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
    }
}
