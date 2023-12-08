using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    // Reference to game over UI elements
    public GameObject gameWinUI;
    public GameObject gameLoseUI;

    // Flag to track whether the game is over
    bool gameIsOver;

    // Subscribe to events when the GameUI is initialized
    void Start()
    {
        // Subscribe to the Player's event for reaching the end of the level
        FindObjectOfType<PlayerMovement>().OnReachedEndOfLevel += ShowGameWinUI;

        // Subscribe to the new event for handling player hit by a bullet
        FindObjectOfType<PlayerMovement>().OnPlayerHitByBullet += ShowGameLoseUI;
    }

    // Check for input to restart the game after it's over
    void Update()
    {
        if (gameIsOver)
        {
            // If the game is over, listen for Space key press to restart the game
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Reload the scene (restart the game)
                SceneManager.LoadScene(0);
            }
        }
    }

    // Show game win UI and handle game over logic
    void ShowGameWinUI()
    {
        // Call the common method for game over UI
        OnGameOver(gameWinUI);
    }

    void ShowGameLoseUI()
    {
        // Call the common method for game over UI
        OnGameOver(gameLoseUI);
    }

    // Common method to handle game over UI elements
    void OnGameOver(GameObject gameOverUI)
    {
        // Activate the specified game over UI
        gameOverUI.SetActive(true);

        // Set the game over flag to true
        gameIsOver = true;

        // Unsubscribe from events to avoid memory leaks
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.OnReachedEndOfLevel -= ShowGameWinUI;
            playerMovement.OnPlayerHitByBullet -= ShowGameLoseUI;
        }
    }

}
