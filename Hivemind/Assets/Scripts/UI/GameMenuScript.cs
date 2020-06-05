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
        TimeController.Instance.ResumeGame();
    }

    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        TimeController.Instance.PauseGame();
    }

    public void ReturnToMenu()
    {
        TimeController.Instance.ResumeGame(1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void SettingsMenu()
    {
        PauseMenuPanel.SetActive(false);
        SettingsMenuPanel.SetActive(true);
    }
}
