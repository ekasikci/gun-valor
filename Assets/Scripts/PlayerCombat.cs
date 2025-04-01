using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    private InputActionReference fireRef;
    private Rigidbody2D body;
    public float health = 1000;
    public float armor = 1f;
    private int FORCE_MULTIPLIER = 100;
    private int playerNumber;
    public GameObject weapon;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
    }

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

    private void OnEnable()
    {
        fireRef.action.started += Fire;
    }

    private void OnDisable()
    {
        fireRef.action.started -= Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        weapon.GetComponent<Weapon>().Shoot();
    }

}
