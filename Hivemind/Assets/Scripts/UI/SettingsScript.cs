using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField]
    private GameObject prevPanel;

    private AudioSource audioSrc;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private AudioSource mainAudioSource;

    [SerializeField]
    private Toggle fullscreenToggle;

    public Dropdown dropdownMenu;

    public static float currentVolume = 1f;

    private int width = 1920;

    public static int globalWidth = 1920;

    private int height = 1080;

    public static int globalHeight = 1080;

    private bool Fullscreen = true;

    public static bool globalFullscreen = true;

    Resolution[] resolutions;
    public static event EventHandler OnVolumeChanged;



    private void Awake()
    {
        audioSrc = gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        dropdownMenu.ClearOptions();
        List<Dropdown.OptionData> dropdownElements = new List<Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            if(i != 0)
                if (CompareRes(resolutions[i], resolutions[i - 1]))
                {
                    continue;
                }
            string resolutionString = ResToString(resolutions[i]);
            dropdownElements.Add(new Dropdown.OptionData(resolutionString));
        }

        dropdownMenu.AddOptions(dropdownElements);

        if (PlayerPrefs.HasKey("Volume"))
        {
            currentVolume = PlayerPrefs.GetFloat("Volume");
        }

        volumeSlider.value = currentVolume;
        audioSrc.volume = currentVolume;
        audioSrc.Stop();

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            globalFullscreen = (PlayerPrefs.GetInt("Fullscreen") == 1); 
            fullscreenToggle.isOn = globalFullscreen;
            Fullscreen = globalFullscreen;
        }
        fullscreenToggle.isOn = globalFullscreen;
        if (PlayerPrefs.HasKey("Width"))
        {
            globalWidth = PlayerPrefs.GetInt("Width");
        }

        if (PlayerPrefs.HasKey("Height"))
        {
            globalHeight = PlayerPrefs.GetInt("Height");
        }
        ChangeResolution(globalWidth, globalHeight, globalFullscreen);
    }

    public void ChangeVolume(float volume)
    {
        if (!audioSrc) return;

        audioSrc.volume = volume;
        audioSrc.Play();
    }

    public void ChangeResolutionDropDown(int index)
    {
        width = resolutions[index].width;
        height = resolutions[index].height;
    }
    public void ChangeResolution(int width, int height, bool fullscreen = true)
    {
        globalHeight = height;
        globalWidth = width;
        Screen.SetResolution(width, height, fullscreen);
    }

    public void ChangeFullscreen()
    {
        Fullscreen = fullscreenToggle.isOn;
    }

    private string ResToString(Resolution resolution)
    {
        return resolution.width + " x " + resolution.height;
    }

    private bool CompareRes(Resolution resolution1, Resolution resolution2)
    {
        return 
            (resolution1.width == resolution2.width 
            && resolution1.height == resolution2.height) ;
    }
    public void Save()
    {
        globalWidth = width;
        globalHeight = height;
        globalFullscreen = Fullscreen;
        ChangeResolution(globalWidth, globalHeight, globalFullscreen);
        PlayerPrefs.SetInt("Fullscreen", Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("Width", globalWidth);
        PlayerPrefs.SetInt("Height", globalHeight);
        PlayerPrefs.SetFloat("Volume", audioSrc.volume);
        currentVolume = audioSrc.volume;
        mainAudioSource.volume = audioSrc.volume;
        PlayerPrefs.Save();

        if(OnVolumeChanged != null)
        {
            OnVolumeChanged.Invoke(null, EventArgs.Empty);
        }
    }
    public void Back()
    {
        volumeSlider.value = currentVolume;
        fullscreenToggle.isOn = globalFullscreen;
        gameObject.SetActive(false);
        if (prevPanel != null)
        {
            prevPanel.SetActive(true);
        }
    }
}
