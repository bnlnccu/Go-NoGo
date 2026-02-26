using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor 工具：自動生成 Go/No-Go 打地鼠遊戲所需的佔位 Sprite 素材。
/// 透過選單 Tools > Go-No-Go > 生成佔位素材 執行。
/// </summary>
public static class SpriteGenerator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Go-No-Go/生成佔位素材")]
    public static void GenerateAllSprites()
    {
        string folder = "Assets/Art/GoNoGo";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets/Art", "GoNoGo");
        }

        // 生成 Go 地鼠（棕色圓形）
        GenerateCircleSprite(folder, "GoMole", new Color(0.6f, 0.4f, 0.2f), 128, true);
        // 生成 No-Go 地鼠（紅色圓形，帶 X 標記）
        GenerateCircleSprite(folder, "NoGoMole", new Color(0.85f, 0.2f, 0.2f), 128, true, true);
        // 生成洞口（深棕色橢圓）
        GenerateEllipseSprite(folder, "Hole", new Color(0.2f, 0.15f, 0.1f), 128, 64);
        // 生成背景（草地綠色）
        GenerateSolidSprite(folder, "Background", new Color(0.4f, 0.7f, 0.3f), 64, 64);
        // 生成被敲中效果（星星爆炸）
        GenerateStarSprite(folder, "HitEffect", new Color(1f, 0.9f, 0.2f), 128);
        // 生成 Go 地鼠被打中的圖（棕色扁橢圓 + 暈眩眼 ××）
        GenerateHitMoleSprite(folder, "GoMoleHit", new Color(0.6f, 0.4f, 0.2f), 128, false);
        // 生成 No-Go 地鼠被誤敲的圖（紅色 + 怒目 + ！標記）
        GenerateHitMoleSprite(folder, "NoGoMoleHit", new Color(0.85f, 0.2f, 0.2f), 128, true);

        AssetDatabase.Refresh();
        Debug.Log("Go/No-Go 佔位素材已全部生成！路徑：" + folder);
    }

    private static void GenerateCircleSprite(string folder, string name, Color color, int size, bool addFace, bool addCross = false)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        float center = size / 2f;
        float radius = size / 2f - 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= radius)
                {
                    // 簡單光影效果
                    float shade = 1f - (dist / radius) * 0.3f;
                    Color c = color * shade;
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }

        if (addFace)
        {
            // 畫眼睛（兩個白點）
            DrawCircle(tex, (int)(center - size * 0.15f), (int)(center + size * 0.1f), size / 16, Color.white);
            DrawCircle(tex, (int)(center + size * 0.15f), (int)(center + size * 0.1f), size / 16, Color.white);
            // 畫瞳孔
            DrawCircle(tex, (int)(center - size * 0.15f), (int)(center + size * 0.1f), size / 32, Color.black);
            DrawCircle(tex, (int)(center + size * 0.15f), (int)(center + size * 0.1f), size / 32, Color.black);
            // 畫鼻子
            DrawCircle(tex, (int)center, (int)(center - size * 0.02f), size / 24, new Color(0.3f, 0.2f, 0.1f));
            // 畫嘴巴（小弧線）
            DrawHorizontalLine(tex, (int)(center - size * 0.1f), (int)(center + size * 0.1f), (int)(center - size * 0.15f), Color.black, 2);
        }

        if (addCross)
        {
            // 在 No-Go 地鼠上畫 X 標記（用白色）
            Color crossColor = Color.white;
            int thickness = 3;
            for (int i = 0; i < size; i++)
            {
                for (int t = -thickness; t <= thickness; t++)
                {
                    // 對角線 1
                    int y1 = i + t;
                    if (i >= 0 && i < size && y1 >= 0 && y1 < size)
                    {
                        float dx = i - center;
                        float dy = y1 - center;
                        if (Mathf.Sqrt(dx * dx + dy * dy) <= radius - 4)
                            tex.SetPixel(i, y1, crossColor);
                    }
                    // 對角線 2
                    int y2 = size - 1 - i + t;
                    if (i >= 0 && i < size && y2 >= 0 && y2 < size)
                    {
                        float dx = i - center;
                        float dy = y2 - center;
                        if (Mathf.Sqrt(dx * dx + dy * dy) <= radius - 4)
                            tex.SetPixel(i, y2, crossColor);
                    }
                }
            }
        }

        tex.Apply();
        SaveTextureAsSprite(tex, folder, name);
    }

    private static void GenerateEllipseSprite(string folder, string name, Color color, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        float cx = width / 2f;
        float cy = height / 2f;
        float rx = width / 2f - 2;
        float ry = height / 2f - 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x - cx) / rx;
                float dy = (y - cy) / ry;
                if (dx * dx + dy * dy <= 1f)
                {
                    float shade = 0.8f + 0.2f * (1f - (dx * dx + dy * dy));
                    Color c = color * shade;
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }

        tex.Apply();
        SaveTextureAsSprite(tex, folder, name);
    }

    private static void GenerateSolidSprite(string folder, string name, Color color, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        SaveTextureAsSprite(tex, folder, name);
    }

    private static void GenerateStarSprite(string folder, string name, Color color, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        float center = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float angle = Mathf.Atan2(dy, dx);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float starRadius = center * 0.8f * (0.5f + 0.5f * Mathf.Abs(Mathf.Sin(angle * 4)));

                if (dist <= starRadius)
                {
                    float alpha = 1f - (dist / starRadius) * 0.5f;
                    Color c = color;
                    c.a = alpha;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }

        tex.Apply();
        SaveTextureAsSprite(tex, folder, name);
    }

    private static void DrawCircle(Texture2D tex, int cx, int cy, int radius, Color color)
    {
        for (int y = cy - radius; y <= cy + radius; y++)
        {
            for (int x = cx - radius; x <= cx + radius; x++)
            {
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    if (dx * dx + dy * dy <= radius * radius)
                        tex.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void DrawHorizontalLine(Texture2D tex, int x1, int x2, int y, Color color, int thickness)
    {
        for (int x = x1; x <= x2; x++)
        {
            for (int t = -thickness / 2; t <= thickness / 2; t++)
            {
                int py = y + t;
                if (x >= 0 && x < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(x, py, color);
            }
        }
    }

    /// <summary>
    /// 生成被打中的地鼠 Sprite。
    /// isAngry=false → 暈眩（被打扁：扁橢圓 + ×× 眼）
    /// isAngry=true  → 生氣（怒目 + ！標記）
    /// </summary>
    private static void GenerateHitMoleSprite(string folder, string name, Color color, int size, bool isAngry)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        float cx = size / 2f;
        float cy = size / 2f;

        if (!isAngry)
        {
            // 被打扁：畫一個扁橢圓
            float rx = size / 2f - 2;
            float ry = size / 4f; // 高度壓扁一半
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    if (dx * dx + dy * dy <= 1f)
                    {
                        float shade = 1f - Mathf.Sqrt(dx * dx + dy * dy) * 0.3f;
                        Color c = color * shade;
                        c.a = 1f;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                }
            }

            // 暈眩眼 ×× （左眼）
            DrawCross(tex, (int)(cx - size * 0.18f), (int)(cy + size * 0.05f), size / 12, Color.white, 2);
            // 暈眩眼 ×× （右眼）
            DrawCross(tex, (int)(cx + size * 0.18f), (int)(cy + size * 0.05f), size / 12, Color.white, 2);
            // 嘴巴（波浪線用短橫線代替）
            DrawHorizontalLine(tex, (int)(cx - size * 0.1f), (int)(cx + size * 0.1f), (int)(cy - size * 0.08f), Color.black, 2);
        }
        else
        {
            // 生氣：畫圓形（跟普通地鼠一樣大）
            float radius = size / 2f - 2;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius)
                    {
                        float shade = 1f - (dist / radius) * 0.3f;
                        Color c = color * shade;
                        c.a = 1f;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                }
            }

            // 怒目（V 形眉毛 + 小白眼）
            // 左眉毛（往內下斜）
            DrawLine(tex, (int)(cx - size * 0.28f), (int)(cy + size * 0.22f),
                         (int)(cx - size * 0.10f), (int)(cy + size * 0.16f), Color.black, 3);
            // 右眉毛（往內下斜）
            DrawLine(tex, (int)(cx + size * 0.28f), (int)(cy + size * 0.22f),
                         (int)(cx + size * 0.10f), (int)(cy + size * 0.16f), Color.black, 3);
            // 眼睛
            DrawCircle(tex, (int)(cx - size * 0.15f), (int)(cy + size * 0.08f), size / 18, Color.white);
            DrawCircle(tex, (int)(cx + size * 0.15f), (int)(cy + size * 0.08f), size / 18, Color.white);
            DrawCircle(tex, (int)(cx - size * 0.15f), (int)(cy + size * 0.08f), size / 36, Color.black);
            DrawCircle(tex, (int)(cx + size * 0.15f), (int)(cy + size * 0.08f), size / 36, Color.black);
            // 生氣嘴巴（倒弧線 → 用短橫線表示怒氣）
            DrawHorizontalLine(tex, (int)(cx - size * 0.12f), (int)(cx + size * 0.12f), (int)(cy - size * 0.10f), Color.black, 3);
            // ！標記（在頭頂）
            DrawCircle(tex, (int)cx, (int)(cy + size * 0.38f), size / 20, Color.yellow);
            // 驚嘆號的長條
            for (int dy = (int)(cy + size * 0.24f); dy < (int)(cy + size * 0.35f); dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int px = (int)cx + dx;
                    if (px >= 0 && px < size && dy >= 0 && dy < size)
                        tex.SetPixel(px, dy, Color.yellow);
                }
            }
        }

        tex.Apply();
        SaveTextureAsSprite(tex, folder, name);
    }

    private static void DrawCross(Texture2D tex, int cx, int cy, int halfSize, Color color, int thickness)
    {
        // 對角線 ×
        for (int i = -halfSize; i <= halfSize; i++)
        {
            for (int t = -thickness / 2; t <= thickness / 2; t++)
            {
                int x1 = cx + i;
                int y1 = cy + i + t;
                if (x1 >= 0 && x1 < tex.width && y1 >= 0 && y1 < tex.height)
                    tex.SetPixel(x1, y1, color);
                int y2 = cy - i + t;
                if (x1 >= 0 && x1 < tex.width && y2 >= 0 && y2 < tex.height)
                    tex.SetPixel(x1, y2, color);
            }
        }
    }

    private static void DrawLine(Texture2D tex, int x1, int y1, int x2, int y2, Color color, int thickness)
    {
        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);
        int steps = Mathf.Max(dx, dy);
        if (steps == 0) return;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            int px = Mathf.RoundToInt(Mathf.Lerp(x1, x2, t));
            int py = Mathf.RoundToInt(Mathf.Lerp(y1, y2, t));
            for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
            {
                for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                {
                    int fx = px + tx;
                    int fy = py + ty;
                    if (fx >= 0 && fx < tex.width && fy >= 0 && fy < tex.height)
                        tex.SetPixel(fx, fy, color);
                }
            }
        }
    }

    private static void SaveTextureAsSprite(Texture2D tex, string folder, string name)
    {
        byte[] pngData = tex.EncodeToPNG();
        string path = Path.Combine(folder, name + ".png");
        File.WriteAllBytes(path, pngData);
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }
#endif
}
