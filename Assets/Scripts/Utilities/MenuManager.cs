using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public GameObject hud;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the MenuManager between scenes if needed.
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }
    private void Start()
    {
        /*  PoolingManager.Instance.InitializePool(coinPrefab, initialCoinPoolSize);
          PoolingManager.Instance.InitializePool(enemyPrefab, initialEnemyPoolSize);*/
        // Initialize menus.
        hud.SetActive(true);
        gameOverMenu.SetActive(false);
        //pauseMenu.SetActive(false);
    }

    public void ShowGameOverMenu()
    {
        // Display the game over menu.
        hud.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        // Display the pause menu.
        hud.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game.
    }

    public void ResumeGame()
    {
        // Resume the game from the pause menu.
        hud.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Unpause the game.
    }

    public void RestartGame()
    {
        // Restart the game from the game over menu.
        hud.SetActive(true);
        gameOverMenu.SetActive(false);
        // Add logic to reset the game.
    }

    public void ExitGame()
    {
        // Quit the application (works in standalone builds).
        Application.Quit();
    }
}
