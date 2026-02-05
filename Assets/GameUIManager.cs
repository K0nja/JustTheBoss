using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Health Bars")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider bossHealthBar;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private BossController boss;

    private void Start()
    {
        // Hide end game panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Find references if not assigned
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (boss == null) boss = FindFirstObjectByType<BossController>();

        // Set max values for health bars
        if (player != null && playerHealthBar != null)
        {
            playerHealthBar.maxValue = player.GetMaxHealth();
            playerHealthBar.value = player.GetMaxHealth();
        }

        if (boss != null && bossHealthBar != null)
        {
            bossHealthBar.maxValue = boss.GetMaxHealth();
            bossHealthBar.value = boss.GetMaxHealth();
        }
    }

    private void Update()
    {
        UpdateHealthBars();
        CheckGameState();
    }

    private void UpdateHealthBars()
    {
        if (player != null && playerHealthBar != null)
        {
            playerHealthBar.value = player.GetCurrentHealth();
        }

        if (boss != null && bossHealthBar != null)
        {
            bossHealthBar.value = boss.GetCurrentHealth();
        }
    }

    private void CheckGameState()
    {
        // Check if player died
        if (player != null && player.GetCurrentHealth() <= 0 && gameOverPanel != null && !gameOverPanel.activeInHierarchy)
        {
            ShowGameOver();
        }

        // Check if boss died
        if (boss == null && victoryPanel != null && !victoryPanel.activeInHierarchy)
        {
            ShowVictory();
        }
    }
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    private void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause FIRST
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!"); // For editor testing
    }
}