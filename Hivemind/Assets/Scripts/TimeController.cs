using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    private float currentTimeScale = 1;

    public bool Paused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void SetTimeScale(float scale)
    {
        currentTimeScale = scale;
        if (!Paused)
            Time.timeScale = currentTimeScale;
    }

    public void PauseGame()
    {
        Paused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Paused = false;
        Time.timeScale = currentTimeScale;
    }

    public void ResumeGame(float scale)
    {
        SetTimeScale(scale);
        ResumeGame();
    }
}