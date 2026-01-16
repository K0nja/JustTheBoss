using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private bool isPlayerProjectile = true;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move upward (or in specified direction)
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerProjectile && collision.CompareTag("Boss"))
        {
            // BossController boss = collision.GetComponent<BossController>();
            // if (boss != null)
            // {
            //     boss.TakeDamage(damage);
            // }
            // Destroy(gameObject);
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
}