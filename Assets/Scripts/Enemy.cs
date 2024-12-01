using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 5f;
    [SerializeField] private int maxHealth = 50;

    private Transform player;
    private float nextFireTime = 0f;
    private int currentHealth;
    private EnemySpawner spawner;
    private Collider2D enemyCollider;

    void Awake()
    {
        currentHealth = maxHealth;

        spawner = FindObjectOfType<EnemySpawner>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        enemyCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (player != null && gameObject.activeSelf)
        {
            MoveTowardPlayer();
            RotateToFacePlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void MoveTowardPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    void RotateToFacePlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or fire point not assigned!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * bulletSpeed;
        }

        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        if (bulletCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, enemyCollider);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        if (spawner != null)
        {
            spawner.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetEnemy()
    {
        // Reset health and other states
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.ResetHealth(); // Ensure health is fully restored
        }

        Debug.Log($"{gameObject.name} has been reset.");
    }

}
