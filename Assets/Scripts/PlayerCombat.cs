using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerCombat : MonoBehaviour
{

    private InputActionReference fireRef;
    private Rigidbody2D body;
    public float armor = 1f;
    private int FORCE_MULTIPLIER = 800;
    private int playerNumber;
    public GameObject weapon;
    private Coroutine fireCoroutine;
    private const float FASTEST_FIRE_RATE = 0.01f;
    private const int PLAYER_HEALTH = 1000;
    public float health;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        weapon = transform.Find("Weapon").gameObject;
        if (weapon == null)
        {
            Debug.LogError("Weapon not found in children of Player.");
        }
        health = PLAYER_HEALTH;
    }

    private void Start()
    {
        playerNumber = GetComponent<Player>().playerNumber;

        // Automatically load the InputActionAsset from the Resources folder.
        var inputAsset = Resources.Load<InputActionAsset>("PlayerController");
        if (inputAsset == null)
        {
            Debug.LogError("PlayerController asset not found in Resources.");
            return;
        }

        string fireActionName = "Fire" + playerNumber;
        var fireAction = inputAsset.FindAction(fireActionName);
        if (fireAction == null)
        {
            Debug.LogError($"Action '{fireAction}' not found in the asset.");
        }
        else
        {
            fireRef = InputActionReference.Create(fireAction);
        }

        Debug.Log("FireActionName: " + fireActionName);
        Debug.Log("FireAction: " + fireAction);
        Debug.Log("FireRef: " + fireRef);

        if (fireRef != null && fireRef.action != null)
        {
            fireRef.action.performed += StartFiring;
            fireRef.action.canceled += StopFiring;
        }
        else
        {
            Debug.LogWarning("fireRef or fireRef.action is null on OnEnable.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            GameManager.Instance.updateScoreTable(playerNumber);
            reSpawn();
        }

    }

    private void reSpawn()
    {
        transform.position = GameManager.Instance.transform.position;
    }

    public void TakeDamage(int damage, Vector2 bulletForce)
    {
        float damageToTake = damage / armor;
        health -= damageToTake;

        if (health <= 0)
        {
            GameManager.Instance.updateScoreTable(playerNumber);

            if (GameManager.Instance.isGameOn)
            {
                reSpawn();
                health = PLAYER_HEALTH;
            }
        }

        // The recoil force magnitude can be based on the damage scaled by the armor.
        float recoilForce = damage / armor;
        Vector2 recoil = bulletForce.normalized * recoilForce;
        recoil *= FORCE_MULTIPLIER;

        // Start knockback coroutine
        StartCoroutine(ApplyKnockbackOverTime(recoil, 0.125f)); 
    }

    IEnumerator ApplyKnockbackOverTime(Vector2 force, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            body.AddForce(force * Time.fixedDeltaTime, ForceMode2D.Force); // Apply force gradually
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        if (fireRef != null && fireRef.action != null)
        {
            fireRef.action.performed -= StartFiring;
            fireRef.action.canceled -= StopFiring;
        }
    }

    private void StartFiring(InputAction.CallbackContext context)
    {
        if (fireCoroutine == null && GameManager.Instance.isGameOn)
        {
            fireCoroutine = StartCoroutine(FireContinuously());
        }
    }

    private void StopFiring(InputAction.CallbackContext context)
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    private IEnumerator FireContinuously()
    {
        while (true)
        {
            weapon.GetComponent<Weapon>().Shoot();
            yield return new WaitForSeconds(FASTEST_FIRE_RATE);
        }
    }

}
