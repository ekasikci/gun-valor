using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int numberOfPlayers = 1;
    public GameObject playerPrefab;
    [HideInInspector] public Transform[] spawnPoints;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        // Make sure there are enough spawn points for the number of players
        if (spawnPoints.Length < numberOfPlayers)
        {
            Debug.LogError("Not enough spawn points for the number of players.");
            return;
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Instantiate player at the corresponding spawn point
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            Player playerScript = player.GetComponent<Player>();
            playerScript.playerNumber = i + 1; // Assign player number
            Debug.Log($"Player {playerScript.playerNumber} spawned at {spawnPoints[i].position}");
        }
    }

    public void LoadMap(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }
}