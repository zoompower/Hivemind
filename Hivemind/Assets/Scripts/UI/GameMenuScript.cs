using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject PauseMenuPanel;

    [SerializeField]
    private GameObject SettingsMenuPanel;

    [SerializeField]
    private UiController UIgameobject;

    private bool paused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
            paused = !paused;
        }
    }
    public void ResumeGame()
    {
        SettingsMenuPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void SettingsMenu()
    {
        PauseMenuPanel.SetActive(false);
        SettingsMenuPanel.SetActive(true);
    }
}
