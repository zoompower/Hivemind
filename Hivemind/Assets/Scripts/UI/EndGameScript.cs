using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TimeController.Instance.PauseGame();
    }

    public void RestartGame()
    {
        TimeController.Instance.ResumeGame(1f);
        SceneManager.LoadScene("Level_1");
    }

    public void ReturnToMenu()
    {
        TimeController.Instance.ResumeGame(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
