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

    

    public void Start()
    {
       

      
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
        if (mainMenu.activeSelf == true)
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
        }
    }

   
    public void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
