using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private int damage;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        rb.linearVelocity = transform.right * speed;
    }

    public void Initialize(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCombat player = collision.GetComponent<PlayerCombat>();
        if (player != null)
        {
            Vector2 bulletForce = rb.linearVelocity.normalized;
            Debug.Log("Bullet force: " + bulletForce);
            player.TakeDamage(damage, bulletForce);
        }
        Destroy(gameObject);
    }
}
