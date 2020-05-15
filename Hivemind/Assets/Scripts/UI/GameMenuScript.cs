using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject PauseMenuPanel;

    [SerializeField]
    private GameObject SettingsMenuPanel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (PauseMenuPanel.activeSelf || SettingsMenuPanel.activeSelf)
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

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SettingsMenu()
    {
        PauseMenuPanel.SetActive(false);
        SettingsMenuPanel.SetActive(true);
    }
}
