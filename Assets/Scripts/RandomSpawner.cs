using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{

    public GameObject prefab;
    private float spawnRateMin = 1f;
    private float spawnRateMax = 5f;
    private float spawnRadius = 2f;
    private float minSpeed = 1f;
    private float maxSpeed = 5f;
    private float spawnHeight = 10f;


    private IEnumerator SpawnObjects()
    {
        while (GameManager.Instance.isGameOn)
        {
            // Randomize the spawn rate
            float spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            yield return new WaitForSeconds(spawnRate);


            // Randomize the spawn position within the radius
            Vector2 spawnPosition = new Vector2(
                Random.Range(transform.position.x - spawnRadius, transform.position.x + spawnRadius),
                spawnHeight
            );

            // Instantiate the prefab at the random position
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Randomize the speed
            float speed = Random.Range(minSpeed, maxSpeed);
            Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, -speed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            // You can add any additional logic here if needed
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
