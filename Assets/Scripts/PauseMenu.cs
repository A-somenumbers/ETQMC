using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public GameObject DeathMenuPanel;
    public bool isPaused = false;
    public bool isDead = false;
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
        
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        isPaused = false;
        isDead= false;
        currentScene = SceneManager.GetActiveScene().name; // Ensure menu is hidden at start
    }

    void Update()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (taggedObjects.Length <= 1)
        {
            Debug.Log("Win");
        } else
        {
            Debug.Log($"There are {taggedObjects.Length - 1} GameObjects with the tag 'Enemy' in the scene.");
        }

        
        if(health.currentHealth<=0)
        {
            isDead = true;
            dead();
        } 

        if (Input.GetKeyDown(KeyCode.Escape) && !isDead)
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
        
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Stop time
        isPaused = true;
        // Optionally, unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void dead()
    {
        DeathMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Stop time
        isDead = true;
        // Optionally, unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        isDead = false;
        
    }

    public void RestartGame()
    {
        pauseMenuPanel.SetActive(false);
        DeathMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        isDead = false;
        SceneManager.LoadSceneAsync(currentScene);
        
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