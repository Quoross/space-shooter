using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private Health healthComponent; // Reference to the Health component
    [SerializeField] private Vector2 barPosition = new Vector2(10, 10); // Top-left corner
    [SerializeField] private Vector2 barSize = new Vector2(200, 20); // Width and height
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    private Texture2D healthBarTexture;
    private Texture2D backgroundTexture;

    void Awake()
    {
        // Initialize textures once to avoid creating them every frame
        healthBarTexture = new Texture2D(1, 1);
        healthBarTexture.SetPixel(0, 0, Color.white);
        healthBarTexture.Apply();

        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, Color.black);
        backgroundTexture.Apply();

        if (healthComponent == null)
        {
            Debug.LogWarning("Health component is not assigned!");
        }
    }

    void OnGUI()
    {
        if (healthComponent == null)
        {
            return; // No health component to display
        }

        // Calculate the health percentage
        float healthPercentage = healthComponent.GetHealthPercentage();

        // Draw the background bar
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(barPosition.x, barPosition.y, barSize.x, barSize.y), backgroundTexture);

        // Calculate the current health bar width
        float healthBarWidth = barSize.x * healthPercentage;

        // Interpolate color based on health percentage
        GUI.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);

        // Draw the health bar
        GUI.DrawTexture(new Rect(barPosition.x, barPosition.y, healthBarWidth, barSize.y), healthBarTexture);

        // Reset GUI color to avoid affecting other GUI elements
        GUI.color = Color.white;
    }

    public void SetHealthComponent(Health newHealthComponent)
    {
        healthComponent = newHealthComponent; // Allow dynamic assignment of the Health component
    }
}
