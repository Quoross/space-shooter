using UnityEngine;

public class ShipShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Assign the Bullet prefab here
    public Transform barrelEnd; // Assign the BarrelEnd marker here
    public float bulletSpeed = 10f; // Speed of the bullets
    public float fireRate = 0.2f; // Time between shots
    public int poolSize = 20; // Number of bullets in the pool
    public float bulletLifetime = 5f; // Time after which bullets are deactivated

    private float nextFireTime = 0f;
    private GameObject[] bulletPool;
    private float[] bulletTimers; // Tracks how long each bullet has been active
    private int currentBulletIndex = 0;

    void Start()
    {
        // Initialize the bullet pool
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned!");
            return;
        }

        bulletPool = new GameObject[poolSize];
        bulletTimers = new float[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            bulletPool[i] = Instantiate(bulletPrefab);
            bulletPool[i].SetActive(false); // Deactivate initially
            bulletTimers[i] = 0f;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Handle bullet lifetimes
        for (int i = 0; i < poolSize; i++)
        {
            if (bulletPool[i].activeSelf)
            {
                bulletTimers[i] += Time.deltaTime;
                if (bulletTimers[i] >= bulletLifetime)
                {
                    bulletPool[i].SetActive(false); // Deactivate bullet
                    bulletTimers[i] = 0f; // Reset its timer
                }
            }
        }
    }

    void Shoot()
    {
        if (barrelEnd == null)
        {
            Debug.LogWarning("BarrelEnd is not assigned!");
            return;
        }

        // Retrieve the next bullet from the pool
        GameObject bullet = bulletPool[currentBulletIndex];
        currentBulletIndex = (currentBulletIndex + 1) % poolSize;

        // Reset the bullet's timer
        bulletTimers[currentBulletIndex] = 0f;

        // Reset the bullet's state before reuse
        ResetBullet(bullet);

        // Set the bullet's position and rotation
        bullet.transform.position = barrelEnd.position;
        bullet.transform.rotation = barrelEnd.rotation;

        // Activate the bullet
        bullet.SetActive(true);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = barrelEnd.up * bulletSpeed;
        }

        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D playerCollider = GetComponent<Collider2D>();

        if (bulletCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, playerCollider);
        }
    }

    private void ResetBullet(GameObject bullet)
    {
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f; // Reset angular velocity
        }

        Collider2D collider = bullet.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true; // Ensure the collider is re-enabled
        }
    }
}
