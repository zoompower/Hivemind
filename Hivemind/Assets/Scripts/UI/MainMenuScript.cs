using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject settingsMenu;

    [SerializeField]
    private GameObject loadMenu;

    [SerializeField]
    private AudioSource mainMusic;

    void Start()
    {
        settingsMenu.SetActive(true);
        settingsMenu.SetActive(false);
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
        SceneManager.LoadScene("Level_1");
    }

    public void LoadGame()
    {
        mainMenu.SetActive(false);
        loadMenu.GetComponent<LoadMenuScript>().Refresh();
        loadMenu.SetActive(true);
    }

    public void SettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
