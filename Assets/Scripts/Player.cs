using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float jumpForce = 3;
    private Rigidbody2D body;
    private Animator animator;
    public GameObject weapon;
    private bool isGrounded;
    private bool isFacingRight = true;
    private int jumpCount = 0;
    private int maxJumps = 2;

    public float health = 1000;
    public float armor = 0.1f;

    

    public void TakeDamage(int damage, Vector2 bulletForce)
    {
        float damageToTake = damage / armor;
        health -= damageToTake;
        if (health <= 0)
        {
            // Die();
        }

        // The recoil force magnitude can be based on the damage scaled by the armor.
        float recoilForce = damage / armor;
        Vector2 recoil = bulletForce * recoilForce;
        body.AddForce(recoil);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if(horizontalInput > 0 && !isFacingRight)
            Flip();
        else if (horizontalInput < 0 && isFacingRight)
            Flip();


        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
            Jump();

        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isGrounded", isGrounded);

    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        isGrounded = false;
        jumpCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

}
