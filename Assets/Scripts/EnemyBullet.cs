using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 10; // Damage dealt to the player
    public float lifetime = 5f; // Lifetime before deactivation

    private float spawnTime; // Tracks when the bullet was spawned

    void OnEnable()
    {
        // Initialize spawn time for lifetime tracking
        spawnTime = Time.time;
    }

    void Update()
    {
        // Deactivate the bullet after its lifetime expires
        if (Time.time >= spawnTime + lifetime)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check for collision with the player
        ShipController player = other.GetComponent<ShipController>();
        if (player != null)
        {
            Debug.Log("Enemy's bullet hit the player.");

            // Apply damage to the player's health
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Deactivate the bullet instead of destroying it
            gameObject.SetActive(false);
        }
    }

    public void ResetBullet()
    {
        // Reset bullet-specific state (e.g., velocity, angular velocity) for pooling
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}