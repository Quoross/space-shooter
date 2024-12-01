using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100; // Maximum health
    [SerializeField] private bool enableDebugLogs = true; // Toggle debug logs

    private int currentHealth;

    [Header("Events")]
    [SerializeField] private UnityEvent onDeath; // Event triggered on death
    [SerializeField] private UnityEvent<int> onHealthChanged; // Event triggered when health changes

    [Header("Death Handling")]
    [SerializeField] private bool destroyOnDeath = true; // Toggle object destruction on death

    private EnemySpawner spawner; // Reference to the spawner or pooling system

    void Awake()
    {
        InitializeHealth(); // Initialize health during Awake for predictable setup

        // Find the spawner if pooling is used
        spawner = FindObjectOfType<EnemySpawner>();
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        LogDebug($"{gameObject.name} initialized with health: {currentHealth}/{maxHealth}");
        onHealthChanged?.Invoke(currentHealth); // Notify listeners of initial health
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            LogDebug("Invalid damage value ignored.");
            return; // Ignore invalid damage values
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        LogDebug($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            LogDebug("Invalid heal value ignored.");
            return; // Ignore invalid heal values
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        LogDebug($"{gameObject.name} healed by {amount}. Health: {currentHealth}/{maxHealth}");
        onHealthChanged?.Invoke(currentHealth);
    }

    public int GetCurrentHealth() => currentHealth;

    public float GetHealthPercentage() => (float)currentHealth / maxHealth;

    private void Die()
    {
        LogDebug($"{gameObject.name} has died.");
        onDeath?.Invoke();

        if (destroyOnDeath)
        {
            if (spawner != null)
            {
                // Return to pool if spawner exists
                spawner.ReturnToPool(gameObject);
            }
            else
            {
                // Fallback to destroying the object if no pool is available
                Destroy(gameObject);
            }
        }
        else
        {
            // Deactivate instead of destroying
            gameObject.SetActive(false);
        }
    }

    public void ResetHealth()
    {
        // Reset health and ensure the component is ready for reuse
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
        LogDebug($"{gameObject.name} health reset to {currentHealth}/{maxHealth}");
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs && !string.IsNullOrEmpty(message))
        {
            Debug.Log(message);
        }
    }
}
