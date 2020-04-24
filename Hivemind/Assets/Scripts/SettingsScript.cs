using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField]
    private GameObject prevPanal;

    [SerializeField]
    private GameObject settingsMenu;

    [SerializeField]
    private AudioSource audioSrc;

    [SerializeField]
    private Slider volumeSlider;

    public Dropdown dropdownMenu;

    public static float currentVolume = 1;

    public static int globalWidth = 1920;

    public static int globalHeight = 1080;

    

    Resolution[] resolutions;
    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        dropdownMenu.ClearOptions();
        List<Dropdown.OptionData> dropdownElements = new List<Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = ResToString(resolutions[i]);          
                dropdownElements.Add( new Dropdown.OptionData(resolutionString));
        }
        dropdownMenu.AddOptions(dropdownElements);

        if (PlayerPrefs.HasKey("Volume"))
        {
            currentVolume = PlayerPrefs.GetFloat("Volume");
            
        }

        volumeSlider.value = currentVolume;
        audioSrc.volume = currentVolume;
        audioSrc.Stop();

        if (PlayerPrefs.HasKey("Width"))
        {
            globalWidth = PlayerPrefs.GetInt("Width");
        }

        if (PlayerPrefs.HasKey("Height"))
        {
            globalHeight = PlayerPrefs.GetInt("Height");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVolume(float volume)
    {
        audioSrc.volume = volume;
        audioSrc.Play();
    }

    public void ChangeResolutionDropDown(System.Int32 index)
    {
        
        ChangeResolution(resolutions[index].width, resolutions[index].height, false);
    }
    public void ChangeResolution(int width, int height, bool fullscreen = true)
    {
        globalHeight = height;
        globalWidth = width;
        Screen.SetResolution(width, height, fullscreen);
    }

    private string ResToString(Resolution resolution)
    {
        return resolution.width + " X " + resolution.height;
    }
    public void Save()
    {

        PlayerPrefs.SetInt("Width", globalWidth);
        PlayerPrefs.SetInt("Height", globalHeight);
        PlayerPrefs.SetFloat("Volume", audioSrc.volume);
        PlayerPrefs.Save();
    }
    public void Back()
    {
        volumeSlider.value = currentVolume;
        settingsMenu.SetActive(false);
        if(prevPanal != null)
        {
            prevPanal.SetActive(true);
        }
    }
}
