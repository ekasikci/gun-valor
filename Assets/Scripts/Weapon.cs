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

    void Update()
    {
        if (Input.GetButton("Fire1") && canShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;

        GameObject bulletObject = Instantiate(bulletPrefab, weapon.position, weapon.rotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(damage, speed);
        }

        yield return new WaitForSeconds(1f / fireRate);

        canShoot = true;
    }
}