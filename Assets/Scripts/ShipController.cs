using Unity.Mathematics;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 7f;
    public float deceleration = 5f;
    public float stoppingThreshold = 0.05f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    private float2 velocity = float2.zero;
    private float2 input = float2.zero;

    void Update()
    {
        // Capture input in Update for real-time responsiveness
        input = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Handle rotation towards the mouse in Update for smooth rendering
        RotateTowardsMouse();
    }

    void FixedUpdate()
    {
        // Process movement in FixedUpdate for consistent physics handling
        if (math.lengthsq(input) > 0.01f)
        {
            // Normalize input and apply acceleration
            float2 normalizedInput = math.normalize(input);
            velocity += normalizedInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            // Decelerate the ship when no input is provided
            float speed = math.length(velocity);
            speed = math.max(0, speed - deceleration * Time.fixedDeltaTime);
            velocity = math.length(velocity) > 0 ? math.normalize(velocity) * speed : float2.zero;
        }

        // Clamp velocity to max speed
        if (math.length(velocity) > maxSpeed)
        {
            velocity = math.normalize(velocity) * maxSpeed;
        }

        // Stop if velocity is below the threshold
        if (math.length(velocity) < stoppingThreshold)
        {
            velocity = float2.zero;
        }

        // Apply movement to the ship
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.fixedDeltaTime;
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate the direction to the mouse
        Vector3 direction = mousePosition - transform.position;

        // Smoothly rotate the ship towards the mouse
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
