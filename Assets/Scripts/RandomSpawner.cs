using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{

    public GameObject prefab;
    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 2f;
    public float spawnRadius = 5f;
    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    public float spawnHeight = 10f;


    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Randomize the spawn rate
            float spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            yield return new WaitForSeconds(spawnRate);

            // Randomize the spawn position within the radius
            Vector2 spawnPosition = new Vector2(
                Random.Range(-spawnRadius, spawnRadius),
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
