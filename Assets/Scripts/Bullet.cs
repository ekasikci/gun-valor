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
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            Vector2 bulletForce = rb.linearVelocity.normalized;
            player.TakeDamage(damage, bulletForce);
        }
        Destroy(gameObject);
    }
}
