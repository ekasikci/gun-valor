using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

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
    private bool flip;

    public InputActionReference moveAction;
    public InputActionReference fireAction;

    private Vector2 moveInput;



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

        if(transform.localEulerAngles.y == 180)
            flip = true;
        else
            flip = false;
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);

        if (!flip)
        {
            if (moveInput.x > 0 && !isFacingRight)
                Flip();
            else if (moveInput.x < 0 && isFacingRight)
                Flip();
        }
        else
        {
            if (moveInput.x > 0 && isFacingRight)
                Flip();
            else if (moveInput.x < 0 && !isFacingRight)
                Flip();
        }

        if (moveInput.y > 0.5f && jumpCount < maxJumps)
            Jump();

        animator.SetBool("isRunning", moveInput.x != 0);
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

    private void OnEnable()
    {
        fireAction.action.started += Fire;
    }

    private void OnDisable()
    {
        fireAction.action.started -= Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        weapon.GetComponent<Weapon>().Shoot();
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

}
