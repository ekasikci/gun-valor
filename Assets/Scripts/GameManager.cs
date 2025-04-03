using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int numberOfPlayers = 1;
    public GameObject playerPrefab;
    [HideInInspector] public Transform[] spawnPoints;
    public TextMeshProUGUI scoreTable;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private static int team1Score = 0;
    private static int team2Score = 0;
    public static bool isTeam1 = true;
    private const int maxScore = 1;
    public bool isGameOn = true;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;


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
        scoreTable.gameObject.SetActive(true);
    }

    public void updateScoreTable(int teamDied)
    {
        if(teamDied == 1)
            team2Score++;
        else
            team1Score++;

        scoreTable.text = $"{team1Score} - {team2Score}";

        if (team1Score >= maxScore || team2Score >= maxScore)
        {
            isGameOn = false;
            gameOverText.gameObject.SetActive(true);
            scoreTable.text = team1Score > team2Score ? scoreTable.text = "Team 1 Wins!" : scoreTable.text = "Team 2 Wins!";
            restartButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);

        }

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
            playerScript.teamNumber = isTeam1 ? 1 : 2; // Assign team number

            Debug.Log($"Player {playerScript.playerNumber} spawned at {spawnPoints[i].position}");
        }
    }

    public void LoadMap(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("GameScene");
        isGameOn = true;
    }

    public void GoToMenu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }
}