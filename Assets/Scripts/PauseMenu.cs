using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject DeathMenuPanel;
    public GameObject UI;
    public GameObject winMenuPanel;
    public bool isPaused = false;
    public bool isDead = false;
    public bool isWin = false;
    string currentScene;
    public GameObject playerObject;
    public PlayerHealth health;
    GameObject[] taggedObjects;



    void Start()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");

        int count = taggedObjects.Length;

        Debug.Log($"There are {count} GameObjects with the tag 'Enemy' in the scene.");
        health = playerObject.GetComponent<PlayerHealth>();
        winMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        UI.SetActive(true);
        currentScene = SceneManager.GetActiveScene().name; // Ensure menu is hidden at start
    }

    void Update()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (taggedObjects.Length <= 1)
        {
            Debug.Log("Win");
            isWin = true;
        } else
        {
            Debug.Log($"There are {taggedObjects.Length - 1} GameObjects with the tag 'Enemy' in the scene.");
        }

        
        if(health.currentHealth<=0)
        {
            isDead = true;
        } 

        if (Input.GetKeyDown(KeyCode.Escape) && !isDead && !isWin)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (isDead)
        {
            dead();
        }
        if (isWin)
        {
            win();
        }
        
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        UI.SetActive(false);
        Time.timeScale = 0f; // Stop time
        isPaused = true;
        // Optionally, unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void dead()
    {
        DeathMenuPanel.SetActive(true);
        UI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true; 
        isDead = true;
        // Optionally, unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void win()
    {
        winMenuPanel.SetActive(true);
        UI.SetActive(false);
        Time.timeScale = 0f; // Stop time
        isPaused = true;
        isWin = true;
        // Optionally, unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        UI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        isDead = false;
        isWin = false;
        
    }

    public void RestartGame()
    {
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        UI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        isDead = false;
        isWin = false;
        SceneManager.LoadScene(currentScene);
        
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}