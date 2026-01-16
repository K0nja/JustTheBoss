using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private float invincibilityTime = 1f;
    private float lastDamageTime;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float nextFireTime;
    private bool isDashing;
    private float dashTimer;
    private float lastDashTime;
    private Vector2 dashDirection;
    private bool isShooting;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isDashing)
        {
            // Shooting
            if (isShooting && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log($"Move Input: {moveInput}"); // Add this line
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartDash();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }

    private void StartDash()
    {
        if (moveInput != Vector2.zero)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashDirection = moveInput;
            lastDashTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastDamageTime < invincibilityTime)
            return;

        currentHealth -= damage;
        lastDamageTime = Time.time;

        Debug.Log($"Player Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}