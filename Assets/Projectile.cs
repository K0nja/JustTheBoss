using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool isPlayerProjectile = true;

    private bool hasCustomVelocity = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        // Check if a Rigidbody2D already set velocity (from boss)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            hasCustomVelocity = true;
        }
    }

    private void Update()
    {
        // Only move if no custom velocity was set
        if (!hasCustomVelocity)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerProjectile && collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!isPlayerProjectile && collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    // Call this for boss projectiles
    public void SetAsEnemyProjectile()
    {
        isPlayerProjectile = false;
    }
}