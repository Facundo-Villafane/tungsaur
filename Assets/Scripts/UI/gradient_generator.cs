using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

public class GradientGenerator : EditorWindow
{
    [MenuItem("Tools/UI Gradient Generator")]
    static void ShowWindow()
    {
        GetWindow<GradientGenerator>("Gradient Generator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Generador de Gradientes y Sprites para UI", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("BARRAS DE VIDA", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generar Health Gradient (Rojo)", GUILayout.Height(30)))
        {
            GenerateLinearGradient("#D62828", "#F94144", "HealthGradient", 256, 1);
        }
        
        if (GUILayout.Button("Generar Yellow Bar (Amarillo Sólido)", GUILayout.Height(30)))
        {
            GenerateSolidColor("#FFFF00", "YellowBar", 256, 1);
        }
        
        GUILayout.Space(10);
        GUILayout.Label("BARRAS DE ENERGÍA", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generar Energy Gradient (Azul)", GUILayout.Height(30)))
        {
            GenerateLinearGradient("#1D4ED8", "#60A5FA", "EnergyGradient", 256, 1);
        }
        
        GUILayout.Space(10);
        GUILayout.Label("INDICADORES RADIALES", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generar Health Pickup Indicator", GUILayout.Height(30)))
        {
            GenerateRadialGradient("#FFFFFF", "#0A604E", "HealthPickupIndicator", 256);
        }
        
        if (GUILayout.Button("Generar Energy Pickup Indicator", GUILayout.Height(30)))
        {
            GenerateRadialGradient("#FFFFFF", "#456BC4", "EnergyPickupIndicator", 256);
        }
        
        if (GUILayout.Button("Generar Danger Indicator", GUILayout.Height(30)))
        {
            GenerateRadialGradient("#FFFFFF", "#D62828", "DangerIndicator", 256);
        }
        
        GUILayout.Space(10);
        GUILayout.Label("SPRITES ADICIONALES", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generar Sprite Blanco Universal", GUILayout.Height(30)))
        {
            GenerateSolidColor("#FFFFFF", "WhiteSprite", 32, 32);
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("⚡ GENERAR TODO (Recomendado)", GUILayout.Height(40)))
        {
            GenerateAll();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Los archivos se guardarán en Assets/Textures/UI/", EditorStyles.helpBox);
    }
    
    static void GenerateAll()
    {
        Debug.Log("========== GENERANDO TODAS LAS TEXTURAS ==========");
        
        // Barras de vida
        GenerateLinearGradient("#D62828", "#F94144", "HealthGradient", 256, 1);
        GenerateSolidColor("#FFFF00", "YellowBar", 256, 1);
        
        // Barras de energía
        GenerateLinearGradient("#1D4ED8", "#60A5FA", "EnergyGradient", 256, 1);
        
        // Indicadores
        GenerateRadialGradient("#FFFFFF", "#0A604E", "HealthPickupIndicator", 256);
        GenerateRadialGradient("#FFFFFF", "#456BC4", "EnergyPickupIndicator", 256);
        GenerateRadialGradient("#FFFFFF", "#D62828", "DangerIndicator", 256);
        
        // Sprite universal
        GenerateSolidColor("#FFFFFF", "WhiteSprite", 32, 32);
        
        Debug.Log("✅ ¡TODAS LAS TEXTURAS GENERADAS!");
        Debug.Log("Ahora puedes asignar las texturas a tus barras:");
        Debug.Log("- HPBarRedIndicator → HealthGradient");
        Debug.Log("- HPBarYellowIndicator → YellowBar");
        Debug.Log("- MPBarBlueIndicator1/2/3 → EnergyGradient");
        Debug.Log("==================================================");
    }
    
    static void GenerateSolidColor(string hexColor, string fileName, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        Color color = HexToColor(hexColor);
        
        // Llenar toda la textura con el color sólido
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        SaveTextureAsSprite(texture, fileName);
        
        Debug.Log($"✓ Color sólido '{fileName}' generado ({hexColor})");
    }
    
    static void GenerateLinearGradient(string hexStart, string hexEnd, string fileName, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        Color startColor = HexToColor(hexStart);
        Color endColor = HexToColor(hexEnd);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float t = (float)x / (width - 1);
                Color color = Color.Lerp(startColor, endColor, t);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        SaveTextureAsSprite(texture, fileName);
        
        Debug.Log($"✓ Gradiente lineal '{fileName}' generado ({hexStart} → {hexEnd})");
    }
    
    static void GenerateRadialGradient(string hexCenter, string hexOuter, string fileName, int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        Color centerColor = HexToColor(hexCenter);
        Color outerColor = HexToColor(hexOuter);
        
        // Centro transparente, exterior opaco
        centerColor.a = 0f;
        outerColor.a = 1f;
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDistance = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                float t = Mathf.Clamp01(distance / maxDistance);
                
                // Suavizar el gradiente con una curva
                t = Mathf.SmoothStep(0f, 1f, t);
                
                Color color = Color.Lerp(centerColor, outerColor, t);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        SaveTextureAsSprite(texture, fileName);
        
        Debug.Log($"✓ Gradiente radial '{fileName}' generado (centro: {hexCenter}, borde: {hexOuter})");
    }
    
    static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }
    
    static void SaveTextureAsSprite(Texture2D texture, string fileName)
    {
        string directory = "Assets/Textures/UI";
        
        // Crear directorio si no existe
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        string path = $"{directory}/{fileName}.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        AssetDatabase.Refresh();
        
        // Configurar import settings para Sprite
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 100;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        
        Debug.Log($"   → Guardado en: {path}");
    }
}

#endif

// ============================================
// RUNTIME GRADIENT GENERATOR (Optional)
// Para generar gradientes en tiempo de ejecución
// ============================================

public class RuntimeGradientGenerator : MonoBehaviour
{
    public static Texture2D CreateSolidColor(Color color, int width = 32, int height = 32)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    public static Texture2D CreateLinearGradient(Color start, Color end, int width = 256, int height = 1)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float t = (float)x / (width - 1);
                Color color = Color.Lerp(start, end, t);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    public static Texture2D CreateRadialGradient(Color center, Color outer, int size = 256)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        Vector2 centerPos = new Vector2(size / 2f, size / 2f);
        float maxDistance = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, centerPos);
                float t = Mathf.Clamp01(distance / maxDistance);
                t = Mathf.SmoothStep(0f, 1f, t);
                
                Color color = Color.Lerp(center, outer, t);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
}
