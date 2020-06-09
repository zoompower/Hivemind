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
        TimeController.Instance.ResumeGame();
    }

    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        TimeController.Instance.PauseGame();
    }

    public void SaveMenu()
    {
        PauseMenuPanel.SetActive(false);
        SaveMenuPanel.SetActive(true);
        SaveMenuPanel.GetComponent<SaveMenuScript>().Refresh();
    }

    public void LoadMenu()
    {
        PauseMenuPanel.SetActive(false);
        LoadMenuPanel.SetActive(true);
        LoadMenuPanel.GetComponent<LoadMenuScript>().Refresh();
    }

    public void SettingsMenu()
    {
        PauseMenuPanel.SetActive(false);
        SettingsMenuPanel.SetActive(true);
    }

    public void ReturnToMenu()
    {
        TimeController.Instance.ResumeGame(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
