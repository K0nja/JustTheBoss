using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 500;
    private int currentHealth;

    [Header("Attack Patterns")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private float projectileSpeed = 5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveRangeX = new Vector2(-6f, 6f);
    [SerializeField] private Vector2 moveRangeY = new Vector2(2f, 4f);

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    private Transform player;
    private float attackTimer;
    private int currentPhase = 1;
    private Vector2 targetPosition;
    private float moveTimer;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChooseNewPosition();
    }

    private void Update()
    {
        // Move towards target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            ChooseNewPosition();
        }

        // Attack timer
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = timeBetweenAttacks;
        }

        // Check phase transition
        float healthPercent = (float)currentHealth / maxHealth;
        if (healthPercent <= 0.5f && currentPhase == 1)
        {
            EnterPhase2();
        }
    }

    private void ChooseNewPosition()
    {
        targetPosition = new Vector2(
            Random.Range(moveRangeX.x, moveRangeX.y),
            Random.Range(moveRangeY.x, moveRangeY.y)
        );
        moveTimer = Random.Range(2f, 4f);
    }

    private void PerformAttack()
    {
        if (currentPhase == 1)
        {
            // Phase 1: Random attack pattern
            int attackChoice = Random.Range(0, 2);
            if (attackChoice == 0)
                Attack_Spread();
            else
                Attack_AimAtPlayer();
        }
        else
        {
            // Phase 2: More aggressive
            int attackChoice = Random.Range(0, 3);
            if (attackChoice == 0)
                Attack_Spread();
            else if (attackChoice == 1)
                Attack_AimAtPlayer();
            else
                Attack_Circle();
        }
    }

    // Attack Pattern 1: Spread shot
    private void Attack_Spread()
    {
        int bulletCount = currentPhase == 1 ? 5 : 8;
        float angleStep = 60f / bulletCount;
        float startAngle = -30f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            ShootProjectile(angle);
        }
        Debug.Log("Boss: Spread attack!");
    }

    // Attack Pattern 2: Aim at player
    private void Attack_AimAtPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            
            // Shoot 3 bullets in player direction with slight spread
            ShootProjectile(angle - 10f);
            ShootProjectile(angle);
            ShootProjectile(angle + 10f);
        }
        Debug.Log("Boss: Aimed attack!");
    }

    // Attack Pattern 3: Circle (Phase 2 only)
    private void Attack_Circle()
    {
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = angleStep * i;
            ShootProjectile(angle);
        }
        Debug.Log("Boss: Circle attack!");
    }

    private void ShootProjectile(float angleInDegrees)
    {
        if (projectilePrefab == null) return;

        Transform spawnPoint = firePoints != null && firePoints.Length > 0 
            ? firePoints[0] 
            : transform;

        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        
        // Set projectile direction
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
        
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Set as boss projectile
        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.SetAsEnemyProjectile();
        }
    }

    private void EnterPhase2()
    {
        currentPhase = 2;
        timeBetweenAttacks *= 0.7f; // Attack faster
        moveSpeed *= 1.3f; // Move faster
        Debug.Log("Boss entered Phase 2!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Boss Health: {currentHealth}/{maxHealth}");

        StartCoroutine(FlashEffect());

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");

        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    private IEnumerator FlashEffect()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color original = sprite.color; 
            sprite.color = Color.white;  
            yield return new WaitForSeconds(0.1f);  
            sprite.color = original;  
        }
    }
}