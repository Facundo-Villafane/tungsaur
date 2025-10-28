using UnityEngine;

/// <summary>
/// Herramienta simple para testear el sistema de UI
/// Adjuntar al mismo GameObject que tiene el Player script
/// </summary>
public class UITester : MonoBehaviour
{
    private Player player;
    
    void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("⚠️ UITester necesita estar en el mismo GameObject que Player!");
        }
        else
        {
            Debug.Log("✓ UITester iniciado correctamente");
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Atajos de teclado
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("=== Aplicando 20 de daño ===");
            player.TakeDamage(20f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("=== Curando 20 HP ===");
            player.Heal(20f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("=== Ganando 33 de energía ===");
            player.GainEnergy(33f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("=== Health Pickup ===");
            player.GetHealthPickUp(25f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("=== Energy Pickup ===");
            player.GetEnergyPickUp(33f);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("=== RESET ===");
            player.CurrentHP = player.MaximumHP;
            player.CurrentENERGY = 0f;
            // Forzar actualización manual
            player.Heal(0f);
            player.GainEnergy(0f);
        }
    }
    
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.padding = new RectOffset(10, 10, 5, 5);
        
        string text = "CONTROLES:\n" +
                     "1 - Daño 20\n" +
                     "2 - Curar 20\n" +
                     "3 - +33 Energía\n" +
                     "4 - Health Pickup\n" +
                     "5 - Energy Pickup\n" +
                     "R - Reset\n\n";
        
        if (player != null)
        {
            text += $"HP: {player.CurrentHP:F0}/{player.MaximumHP:F0}\n";
            text += $"Energy: {player.CurrentENERGY:F0}/{player.MaximumENERGY:F0}";
        }
        
        GUI.Label(new Rect(10, 10, 200, 250), text, style);
    }
}
