using UnityEngine;

/// <summary>
/// Denizaltı Canlıları AR uygulaması için modern tasarım sistemi.
/// Okyanus temalı renk paleti, tipografi ve yardımcı araçlar.
/// </summary>
public static class UITheme
{
    // ── Ana Renkler ──────────────────────────────────────────
    public static readonly Color PrimaryDark    = new Color32(8,  27,  51, 255);   // Derin okyanus
    public static readonly Color PrimaryMid     = new Color32(13, 42,  78, 255);   // Gece mavisi
    public static readonly Color PrimaryLight   = new Color32(22, 72, 130, 255);   // Okyanus mavisi
    public static readonly Color Accent         = new Color32(0, 210, 211, 255);   // Turkuaz / Teal
    public static readonly Color AccentGlow     = new Color32(0, 210, 211, 120);   // Parlak teal (glow)
    public static readonly Color AccentWarm     = new Color32(255, 167, 38, 255);  // Sıcak turuncu vurgu
    public static readonly Color Success        = new Color32(76, 217, 100, 255);  // Yeşil durum
    public static readonly Color Danger         = new Color32(255, 69,  58, 255);  // Kırmızı uyarı

    // ── Metin Renkleri ───────────────────────────────────────
    public static readonly Color TextPrimary    = new Color32(240, 248, 255, 255); // Beyaza yakın
    public static readonly Color TextSecondary  = new Color32(160, 190, 220, 255); // Soluk mavi
    public static readonly Color TextMuted      = new Color32(100, 130, 160, 255); // Çok soluk

    // ── Panel / Glassmorphism ────────────────────────────────
    public static readonly Color GlassBackground  = new Color32(10, 30, 60, 180);   // Yarı saydam koyu
    public static readonly Color GlassBorder      = new Color32(0, 210, 211, 80);   // Teal kenarlık
    public static readonly Color GlassHighlight   = new Color32(255, 255, 255, 15); // Üst parlama
    public static readonly Color OverlayDark      = new Color32(0, 0, 0, 160);      // Koyu overlay
    public static readonly Color ScanLine         = new Color32(0, 210, 211, 200);  // Tarama çizgisi

    // ── Tipografi Boyutları ───────────────────────────────────
    public const int FontTitle    = 42;
    public const int FontSubtitle = 28;
    public const int FontHeading  = 22;
    public const int FontBody     = 18;
    public const int FontCaption  = 14;
    public const int FontSmall    = 12;

    // ── Boyutlar ─────────────────────────────────────────────
    public const float PanelRadius       = 24f;
    public const float SmallRadius       = 12f;
    public const float ButtonRadius      = 16f;
    public const float AnimDuration      = 0.4f;
    public const float AnimDurationFast  = 0.2f;
    public const float AnimDurationSlow  = 0.8f;

    // ── Runtime Rounded Rectangle Sprite Oluşturucu ──────────
    /// <summary>
    /// Çalışma zamanında yuvarlak köşeli dikdörtgen sprite oluşturur.
    /// </summary>
    public static Sprite CreateRoundedRect(int width, int height, int radius, Color fillColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Köşe kontrolü
                bool inCorner = false;
                float dist = 0;

                if (x < radius && y < radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)); inCorner = true; }
                else if (x >= width - radius && y < radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)); inCorner = true; }
                else if (x < radius && y >= height - radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)); inCorner = true; }
                else if (x >= width - radius && y >= height - radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)); inCorner = true; }

                if (inCorner)
                {
                    if (dist <= radius - 1f)
                        pixels[y * width + x] = fillColor;
                    else if (dist <= radius)
                        pixels[y * width + x] = new Color(fillColor.r, fillColor.g, fillColor.b, fillColor.a * (radius - dist));
                    else
                        pixels[y * width + x] = clear;
                }
                else
                {
                    pixels[y * width + x] = fillColor;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100f,
            0, SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
    }

    /// <summary>
    /// Gradient efektli yuvarlak köşeli panel sprite oluşturur.
    /// </summary>
    public static Sprite CreateGradientPanel(int width, int height, int radius, Color topColor, Color bottomColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color rowColor = Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < width; x++)
            {
                bool inCorner = false;
                float dist = 0;

                if (x < radius && y < radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)); inCorner = true; }
                else if (x >= width - radius && y < radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)); inCorner = true; }
                else if (x < radius && y >= height - radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)); inCorner = true; }
                else if (x >= width - radius && y >= height - radius)
                { dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)); inCorner = true; }

                if (inCorner)
                {
                    if (dist <= radius - 1f)
                        pixels[y * width + x] = rowColor;
                    else if (dist <= radius)
                        pixels[y * width + x] = new Color(rowColor.r, rowColor.g, rowColor.b, rowColor.a * (radius - dist));
                    else
                        pixels[y * width + x] = clear;
                }
                else
                {
                    pixels[y * width + x] = rowColor;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100f,
            0, SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
    }

    /// <summary>
    /// Kenarlıklı yuvarlak köşeli panel (glassmorphism border efekti).
    /// </summary>
    public static Sprite CreateBorderedPanel(int width, int height, int radius, Color fillColor, Color borderColor, int borderWidth = 2)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distFromEdge = Mathf.Min(x, y, width - 1 - x, height - 1 - y);
                bool inCorner = false;
                float cornerDist = 0;

                if (x < radius && y < radius)
                { cornerDist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)); inCorner = true; }
                else if (x >= width - radius && y < radius)
                { cornerDist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)); inCorner = true; }
                else if (x < radius && y >= height - radius)
                { cornerDist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)); inCorner = true; }
                else if (x >= width - radius && y >= height - radius)
                { cornerDist = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)); inCorner = true; }

                if (inCorner)
                {
                    if (cornerDist > radius)
                        pixels[y * width + x] = clear;
                    else if (cornerDist > radius - borderWidth)
                        pixels[y * width + x] = borderColor;
                    else
                        pixels[y * width + x] = fillColor;
                }
                else
                {
                    if (distFromEdge < borderWidth)
                        pixels[y * width + x] = borderColor;
                    else
                        pixels[y * width + x] = fillColor;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f), 100f,
            0, SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
    }

    /// <summary>
    /// Dairesel sprite oluşturur (ikonlar, göstergeler için).
    /// </summary>
    public static Sprite CreateCircle(int diameter, Color color)
    {
        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[diameter * diameter];
        Color clear = new Color(0, 0, 0, 0);
        float center = diameter / 2f;
        float radiusF = diameter / 2f;

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radiusF - 1f)
                    pixels[y * diameter + x] = color;
                else if (dist <= radiusF)
                    pixels[y * diameter + x] = new Color(color.r, color.g, color.b, color.a * (radiusF - dist));
                else
                    pixels[y * diameter + x] = clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
    }
}
