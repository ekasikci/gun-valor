using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float jumpForce = 3;
    [SerializeField] private int playerNumber = 0;
    private string[] playerInput = new string[4];
    private Rigidbody2D body;
    private Animator animator;
    public GameObject weapon;
    private bool isGrounded;
    private bool isFacingRight = true;
    private int jumpCount = 0;
    private int maxJumps = 2;
    private int FORCE_MULTIPLIER = 100;

    public float health = 1000;
    public float armor = 1f;
    private bool flip = false;



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
        Vector2 recoil = bulletForce.normalized * recoilForce;
        recoil *= FORCE_MULTIPLIER;

        // Start knockback coroutine
        StartCoroutine(ApplyKnockbackOverTime(recoil, 0.3f)); // 0.3s duration
    }

    IEnumerator ApplyKnockbackOverTime(Vector2 force, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            body.AddForce(force * Time.deltaTime, ForceMode2D.Force); // Apply force gradually
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if(playerNumber % 2 != 0)
        {
            flip = true;
        }

        if (playerNumber == 0)
        {
            playerInput[0] = "Horizontal";
            playerInput[1] = "Vertical";
            playerInput[2] = "Jump";
            playerInput[3] = "Fire";
        }
        else
        {
            playerInput[0] = "Horizontal" + playerNumber;
            playerInput[1] = "Vertical" + playerNumber;
            playerInput[2] = "Jump" + playerNumber;
            playerInput[3] = "Fire" + playerNumber;
        }
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis(playerInput[0]);
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (!flip)
        {
            if (horizontalInput > 0 && !isFacingRight)
                Flip();
            else if (horizontalInput < 0 && isFacingRight)
                Flip();
        }
        else
        {
            if (horizontalInput > 0 && isFacingRight)
                Flip();
            else if (horizontalInput < 0 && !isFacingRight)
                Flip();
        }




        if (Input.GetButtonDown(playerInput[2]) && jumpCount < maxJumps)
            Jump();

        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isGrounded", isGrounded);

        if(Input.GetButtonDown(playerInput[3]))
        {
            weapon.GetComponent<Weapon>().Shoot();
        }
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
