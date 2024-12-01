using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 10; // Damage dealt by this bullet
    public float lifetime = 5f; // Lifetime before the bullet is deactivated

    private Coroutine lifetimeCoroutine;

    void OnEnable()
    {
        // Start the coroutine for lifetime management
        lifetimeCoroutine = StartCoroutine(DeactivateAfterLifetime());
        
#if UNITY_EDITOR
        Debug.Log("PlayerBullet spawned.");
#endif
    }

    void OnDisable()
    {
        // Stop all coroutines if the bullet is deactivated early (e.g., on collision)
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }
    }

    private System.Collections.IEnumerator DeactivateAfterLifetime()
    {
        // Wait for the bullet's lifetime to expire
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the player or objects tagged as "Player"
        if (other.CompareTag("Player"))
        {
#if UNITY_EDITOR
            Debug.Log("PlayerBullet ignored collision with Player.");
#endif
            return;
        }

        // Check for a Health component on the collided object
        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null && targetHealth.isActiveAndEnabled)
        {
            targetHealth.TakeDamage(damage);

#if UNITY_EDITOR
            Debug.Log($"PlayerBullet hit {other.gameObject.name}. Applied {damage} damage.");
#endif

            // Deactivate bullet on impact
            gameObject.SetActive(false);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"PlayerBullet hit {other.gameObject.name}, but it has no Health component.");
#endif
        }
    }

    public void ResetBullet()
    {
        // Reset physics and any other state when the bullet is reused from the pool
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Reset velocity
            rb.angularVelocity = 0f;   // Reset angular velocity
        }
    }

    void Awake()
    {
        // Preconfigure Rigidbody2D for consistent behavior
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Bullets are not affected by physics forces
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }
}
