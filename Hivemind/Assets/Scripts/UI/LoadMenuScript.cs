using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject prevPanel;
    [SerializeField]
    private GameObject contentBox;
    private string selectedSave;

    public void Refresh()
    {
        foreach (Transform child in contentBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (string savefilepath in Directory.GetFiles(GameWorld.GetSavePath()))
        {
            if (Path.GetExtension(savefilepath) == ".txt")
            {
                GameObject game = (GameObject)GameObject.Instantiate(Resources.Load($"Prefabs/UI/SaveFileButton"));
                game.GetComponentInChildren<SaveFileButtonScript>().SetText(savefilepath);
                game.transform.SetParent(contentBox.transform, false);
            }
        }
    }

    public void SetSelectedSaveFile(SaveFileButtonScript buttonScript)
    {
        foreach (Transform child in contentBox.transform)
        {
            child.gameObject.GetComponent<Button>().interactable = true;
        }
        selectedSave = buttonScript.SaveFile;
        transform.Find("LoadButton").GetComponent<Button>().interactable = true;
    }

    public void Load()
    {
        if(selectedSave != "" && selectedSave != null)
        {
            GameWorld.Load(selectedSave);
            Refresh();
            transform.Find("LoadButton").GetComponent<Button>().interactable = false;
            selectedSave = null;
        }
    }

    public void Back()
    {
        selectedSave = null;
        transform.Find("LoadButton").GetComponent<Button>().interactable = false;
        gameObject.SetActive(false);
        if (prevPanel != null)
        {
            prevPanel.SetActive(true);
        }
    }
}
