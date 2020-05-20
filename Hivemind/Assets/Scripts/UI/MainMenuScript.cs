using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject settingsMenu;

    [SerializeField]
    private AudioSource mainMusic;

    void Start()
    {
        if (mainMusic != null)
        {
            if (PlayerPrefs.HasKey("Volume"))
            {
                mainMusic.volume = PlayerPrefs.GetFloat("Volume");
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Map");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
}
