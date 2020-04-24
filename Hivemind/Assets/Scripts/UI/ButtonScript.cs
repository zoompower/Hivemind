using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject settingsMenu;

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
