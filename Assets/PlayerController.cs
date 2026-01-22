using UnityEngine;

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
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float nextFireTime;
    private bool isDashing;
    private float dashTimer;
    private float lastDashTime;
    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        
        inputActions.Player.Fire.performed += ctx => OnFirePressed();
        inputActions.Player.Dash.performed += ctx => OnDashPressed();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        
        inputActions.Player.Fire.performed -= ctx => OnFirePressed();
        inputActions.Player.Dash.performed -= ctx => OnDashPressed();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        if (!isDashing)
        {
            if (inputActions.Player.Fire.IsPressed() && Time.time >= nextFireTime)
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

    private void OnFirePressed()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnDashPressed()
    {
        if (Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartDash();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Debug.Log("Shot fired!");
        }
    }

    private void StartDash()
    {
        if (moveInput != Vector2.zero)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashDirection = moveInput.normalized;
            lastDashTime = Time.time;
            Debug.Log("Dashing!");
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