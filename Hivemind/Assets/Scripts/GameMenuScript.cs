using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject GameMenu;

    [SerializeField]
    private GameObject GameSettingsMenu;

    [SerializeField]
    private AudioSource audioSrc;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            GameMenu.SetActive(true);
            Time.timeScale = 0;

        }
    }
    public void ResumeGame()
    {
        GameMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SettingsMenu()
    {
        GameMenu.SetActive(false);
        GameSettingsMenu.SetActive(true);
    }
}
