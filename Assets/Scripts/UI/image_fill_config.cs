using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Herramienta para configurar automáticamente las Images como Filled
/// Ejecutar una vez para configurar todas las barras de vida y energía
/// </summary>
public class ImageFillConfigurator : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Activar para ver logs detallados")]
    [SerializeField] private bool verboseLogging = true;
    
    [ContextMenu("⚡ Configurar TODAS las Barras")]
    void ConfigureAllBars()
    {
        Debug.Log("========== CONFIGURANDO BARRAS ==========");
        ConfigureHealthBars();
        ConfigureEnergyBars();
        Debug.Log("✅ ¡CONFIGURACIÓN COMPLETA!");
        Debug.Log("=========================================");
    }
    
    [ContextMenu("❤️ Configurar Barras de Vida")]
    void ConfigureHealthBars()
    {
        Debug.Log("--- Configurando Barras de Vida ---");
        ConfigureImageAsFilled("HPBarRedIndicator", 1f, "Barra Roja");
        ConfigureImageAsFilled("HPBarYellowIndicator", 1f, "Barra Amarilla");
        Debug.Log("✓ Barras de vida configuradas");
    }
    
    [ContextMenu("⚡ Configurar Barras de Energía")]
    void ConfigureEnergyBars()
    {
        Debug.Log("--- Configurando Barras de Energía ---");
        ConfigureImageAsFilled("MPBarBlueIndicator1", 0f, "Barra Energía 1");
        ConfigureImageAsFilled("MPBarBlueIndicator2", 0f, "Barra Energía 2");
        ConfigureImageAsFilled("MPBarBlueIndicator3", 0f, "Barra Energía 3");
        Debug.Log("✓ Barras de energía configuradas");
    }
    
    [ContextMenu("🔍 Verificar Configuración")]
    void VerifyConfiguration()
    {
        Debug.Log("========== VERIFICANDO CONFIGURACIÓN ==========");
        
        VerifyBar("HPBarRedIndicator", "Vida Roja", 1f);
        VerifyBar("HPBarYellowIndicator", "Vida Amarilla", 1f);
        VerifyBar("MPBarBlueIndicator1", "Energía 1", 0f);
        VerifyBar("MPBarBlueIndicator2", "Energía 2", 0f);
        VerifyBar("MPBarBlueIndicator3", "Energía 3", 0f);
        
        Debug.Log("===============================================");
    }
    
    void ConfigureImageAsFilled(string objectName, float initialFill, string displayName = "")
    {
        GameObject obj = GameObject.Find(objectName);
        
        if (obj == null)
        {
            Debug.LogWarning($"⚠️ NO encontrado: {objectName}");
            Debug.LogWarning($"   → Verifica que el nombre sea exacto en Hierarchy");
            return;
        }
        
        Image img = obj.GetComponent<Image>();
        
        if (img == null)
        {
            Debug.LogWarning($"⚠️ {objectName} NO tiene componente Image");
            Debug.LogWarning($"   → Add Component → UI → Image");
            return;
        }
        
        // Configurar como Filled
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = (int)Image.OriginHorizontal.Left;
        img.fillAmount = initialFill;
        img.fillClockwise = true;
        
        // Asegurar que tenga un sprite (usar el por defecto si no tiene)
        if (img.sprite == null)
        {
            img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            if (verboseLogging)
                Debug.Log($"   ℹ️ Sprite por defecto asignado a {objectName}");
        }
        
        string name = string.IsNullOrEmpty(displayName) ? objectName : displayName;
        Debug.Log($"✅ {name} configurado: Filled | Amount: {initialFill}");
    }
    
    void VerifyBar(string objectName, string displayName, float expectedFill)
    {
        GameObject obj = GameObject.Find(objectName);
        
        if (obj == null)
        {
            Debug.LogError($"❌ {displayName}: NO ENCONTRADO");
            return;
        }
        
        Image img = obj.GetComponent<Image>();
        
        if (img == null)
        {
            Debug.LogError($"❌ {displayName}: Sin componente Image");
            return;
        }
        
        bool isCorrect = img.type == Image.Type.Filled &&
                        img.fillMethod == Image.FillMethod.Horizontal &&
                        img.fillOrigin == (int)Image.OriginHorizontal.Left &&
                        Mathf.Approximately(img.fillAmount, expectedFill);
        
        if (isCorrect)
        {
            Debug.Log($"✅ {displayName}: Correcto (Fill: {img.fillAmount})");
        }
        else
        {
            Debug.LogWarning($"⚠️ {displayName}: Configuración incorrecta");
            Debug.LogWarning($"   Type: {img.type} (debe ser Filled)");
            Debug.LogWarning($"   FillMethod: {img.fillMethod} (debe ser Horizontal)");
            Debug.LogWarning($"   FillAmount: {img.fillAmount} (esperado: {expectedFill})");
        }
    }
    
    [ContextMenu("🔧 Configurar Barra Específica (Manual)")]
    void ConfigureSpecificBar()
    {
        Debug.Log("==========================================");
        Debug.Log("Para configurar una barra específica:");
        Debug.Log("1. Asigna el GameObject a 'specificBarObject'");
        Debug.Log("2. Usa 'Configurar GameObject Asignado'");
        Debug.Log("==========================================");
    }
    
    [Header("Configuración Manual")]
    [Tooltip("Arrastra aquí el GameObject que quieres configurar")]
    public GameObject specificBarObject;
    
    [Range(0f, 1f)]
    [Tooltip("Fill Amount inicial (0 = vacío, 1 = lleno)")]
    public float specificInitialFill = 1f;
    
    [ContextMenu("✨ Configurar GameObject Asignado")]
    void ConfigureSpecificGameObject()
    {
        if (specificBarObject == null)
        {
            Debug.LogError("⚠️ Asigna un GameObject en 'specificBarObject' primero!");
            return;
        }
        
        Image img = specificBarObject.GetComponent<Image>();
        
        if (img == null)
        {
            Debug.LogError($"⚠️ {specificBarObject.name} no tiene componente Image!");
            return;
        }
        
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = (int)Image.OriginHorizontal.Left;
        img.fillAmount = specificInitialFill;
        img.fillClockwise = true;
        
        if (img.sprite == null)
        {
            img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        }
        
        Debug.Log($"✅ {specificBarObject.name} configurado exitosamente!");
        Debug.Log($"   Fill Amount: {specificInitialFill}");
    }
    
    [ContextMenu("📊 Listar TODAS las Images")]
    void ListAllImages()
    {
        Debug.Log("========== LISTADO DE IMAGES ==========");
        Image[] allImages = FindObjectsOfType<Image>();
        
        foreach (Image img in allImages)
        {
            string fillInfo = img.type == Image.Type.Filled ? $"Filled ({img.fillAmount})" : img.type.ToString();
            Debug.Log($"• {img.name} | Type: {fillInfo} | Sprite: {(img.sprite ? img.sprite.name : "NULL")}");
        }
        
        Debug.Log($"Total: {allImages.Length} Images");
        Debug.Log("=======================================");
    }
}
