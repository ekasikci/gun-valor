using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float jumpForce = 3;

    private Rigidbody2D body;
    private Animator animator;
    private bool isGrounded;
    private bool isFacingRight = true;
    private int jumpCount = 0;
    private const int maxJumps = 2;
    private bool flip;
    private int playerNumber;
    private InputActionReference movementRef;
    private Vector2 moveInput;
    private bool jumpButtonReleased = true;
    private AudioSource audioSource;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip walkSound;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerNumber = GetComponent<Player>().playerNumber;
        audioSource = GetComponent<AudioSource>();

        // Automatically load the InputActionAsset from the Resources folder.
        var inputAsset = Resources.Load<InputActionAsset>("PlayerController");
        if (inputAsset == null)
        {
            Debug.LogError("PlayerController asset not found in Resources.");
            return;
        }

        string movementActionName = "Movement" + playerNumber;

        var movementAction = inputAsset.FindAction(movementActionName);
        if (movementAction == null)
        {
            Debug.LogError($"Action '{movementActionName}' not found in the asset.");
        }
        else
        {
            movementRef = InputActionReference.Create(movementAction);
        }

        flip = transform.localEulerAngles.y == 180;
    }

    private void Update()
    {
        if (GameManager.isGameOn)
        {
            moveInput = movementRef.action.ReadValue<Vector2>();

            body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
            if (moveInput.x != 0 && isGrounded && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walkSound);
            }

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


            if (moveInput.y > 0 && jumpCount < maxJumps && jumpButtonReleased)
            {
                Jump();
                jumpButtonReleased = false; // Set the flag to false after jumping
            }

            if (moveInput.y <= 0)
            {
                jumpButtonReleased = true; // Reset the flag when the jump button is released
            }


            animator.SetBool("isRunning", moveInput.x != 0);
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    private void Jump()
    {
        Debug.Log("Jump count: " + jumpCount);
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        audioSource.PlayOneShot(jumpSound);
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
