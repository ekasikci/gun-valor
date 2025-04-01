using UnityEngine;

public class SpawnedBox : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // Array to hold weapon prefabs

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();
        if (player != null)
        {
            Destroy(gameObject);

            Vector3 weaponPosition = Vector3.zero;
            Quaternion weaponRotation = Quaternion.identity;

            // Store the position and rotation of the old weapon if it exists
            if (player.weapon != null)
            {
                weaponPosition = player.weapon.transform.position;
                weaponRotation = player.weapon.transform.rotation;
                Destroy(player.weapon);
            }

            // Instantiate a random weapon prefab at the stored position and rotation
            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            GameObject weaponObject = Instantiate(weaponPrefabs[randomIndex], weaponPosition, weaponRotation, player.transform);

            // Initialize the new weapon
            Weapon weapon = weaponObject.GetComponent<Weapon>();
            if (weapon != null)
            {
                switch (randomIndex)
                {
                    case 0:
                        weapon.Initialize("Pistol", 5, 10, 3f);
                        break;
                    case 1:
                        weapon.Initialize("Shotgun", 50, 5, 1f);
                        break;
                    case 2:
                        weapon.Initialize("Machine Gun", 10, 20, 10f);
                        break;
                }
                player.weapon = weaponObject;
            }
        }
    }
}