using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    private Transform weapon;
    public GameObject bulletPrefab;
    private string weaponName;
    [SerializeField]private int damage;
    private float speed;
    private float fireRate;
    private bool canShoot = true;
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    public void Initialize(string weaponName, int damage, float speed, float fireRate)
    {
        this.weaponName = weaponName;
        this.damage = damage;
        this.speed = speed;
        this.fireRate = fireRate;
    }

    void Awake()
    {
        weapon = GetComponent<Transform>();
    }

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void Shoot()
    {
        if (canShoot)
            StartCoroutine(shootRoutine());
    }

    void Update()
    {

    }

    private IEnumerator shootRoutine()
    {
        canShoot = false;

        GameObject bulletObject = Instantiate(bulletPrefab, weapon.position, weapon.rotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();

        Collider2D playerCollider = GetComponentInParent<Collider2D>();
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();

        if (bullet != null)
        {
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
            bullet.Initialize(damage, speed);
            audioSource.PlayOneShot(shootSound);
        }

        yield return new WaitForSeconds(1f / fireRate);

        canShoot = true;
    }
}