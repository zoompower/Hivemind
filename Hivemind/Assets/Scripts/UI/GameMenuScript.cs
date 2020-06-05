using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject PauseMenuPanel;

    [SerializeField]
    private GameObject SettingsMenuPanel;

    [SerializeField]
    private GameObject LoadMenuPanel;

    [SerializeField]
    private GameObject SaveMenuPanel;

    [SerializeField]
    private UiController UIgameobject;

    private bool paused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            paused = !paused;
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
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

    public void SaveGame()
    {
        PauseMenuPanel.SetActive(false);
        SaveMenuPanel.SetActive(true);
        SaveMenuPanel.GetComponent<SaveMenuScript>().Refresh();
    }

    public void LoadGame()
    {
        PauseMenuPanel.SetActive(false);
        LoadMenuPanel.SetActive(true);
        LoadMenuPanel.GetComponent<LoadMenuScript>().Refresh();
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
