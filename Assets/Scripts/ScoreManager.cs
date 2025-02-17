using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton instance

    public TMP_Text scoreText; // Reference to the TextMeshPro text
    public GameObject winPanel; // Reference to the Win Panel
    public GameObject losePanel; // Reference to the Lose Panel
    public Button restartButton; // Reference to the Restart Button

    private int successfulOrders = 0; // Counter for successful orders
    private int failedOrders = 0; // Counter for failed orders
    private bool isGameFrozen = false; // Flag to check if the game is frozen

    void Awake()
    {
        // Ensure only one instance of ScoreManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize panels and buttons
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Assign the restart functionality to the button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateScoreText();
    }

    void Update()
    {
        // Check for win condition
        if (successfulOrders >= 10 && !isGameFrozen)
        {
            ActivateWinPanel();
        }

        // Check for lose condition
        if (failedOrders >= 10 && !isGameFrozen)
        {
            ActivateLosePanel();
        }
    }

    // Method to add a successful order
    public void AddSuccessfulOrder()
    {
        successfulOrders++;
        UpdateScoreText();
    }

    // Method to add a failed order
    public void AddFailedOrder()
    {
        failedOrders++;
        UpdateScoreText();
    }

    // Update the score text in the UI
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Successful Orders: {successfulOrders}\nFailed Orders: {failedOrders}";
        }
        else
        {
            Debug.LogError("Score Text (TextMeshPro) is not assigned!");
        }
    }

    private void ActivateWinPanel()
    {
        Debug.Log("You Win!");
        if (winPanel != null) winPanel.SetActive(true);
        FreezeGame();
    }

    private void ActivateLosePanel()
    {
        Debug.Log("You Lose!");
        if (losePanel != null) losePanel.SetActive(true);
        FreezeGame();
    }

    private void FreezeGame()
    {
        // Freeze the game by setting time scale to 0
        Time.timeScale = 0f;
        isGameFrozen = true;
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");

        // Reset scores
        successfulOrders = 0;
        failedOrders = 0;
        UpdateScoreText();

        // Deactivate panels
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Unfreeze the game
        Time.timeScale = 1f;
        isGameFrozen = false;

        // Reload the current scene to reset everything
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}