using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileButtonScript : MonoBehaviour
{
    [HideInInspector]
    public string SaveFile;

    [SerializeField]
    private Text NameText;

    [SerializeField]
    private Text DateText;

    public void SetText(string savefilepath)
    {
        SaveFile = Path.GetFileNameWithoutExtension(savefilepath);
        NameText.text = Path.GetFileNameWithoutExtension(savefilepath);
        DateText.text = File.GetLastWriteTime(savefilepath).ToString();
    }

    public void SetSelectedSaveFile()
    {
        if (GetComponentInParent<LoadMenuScript>() != null)
        {
            GetComponentInParent<LoadMenuScript>().SetSelectedSaveFile(this);
            transform.GetComponent<Button>().interactable = false;
        }
        else if (GetComponentInParent<SaveMenuScript>() != null)
        {
            GetComponentInParent<SaveMenuScript>().SetSelectedSaveFile(this);
            transform.GetComponent<Button>().interactable = false;
        }
    }
}