using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
        FileInfo[] info = new DirectoryInfo(GameWorld.GetSavePath()).GetFiles().OrderByDescending(p => p.LastWriteTime).ToArray();
        foreach (FileInfo savefilepath in info)
        {
            if (savefilepath.Extension == ".txt")
            {
                GameObject game = (GameObject)GameObject.Instantiate(Resources.Load($"Prefabs/UI/SaveFileButton"));
                game.GetComponentInChildren<SaveFileButtonScript>().SetText(savefilepath.FullName);
                game.transform.SetParent(contentBox.transform, false);
            }
        }
    }

    public void DeleteSelectedSaveFile()
    {
        if (selectedSave != "" && selectedSave != null)
        {
            GameWorld.DeleteFile(selectedSave);
            Refresh();
            transform.Find("LoadButton").GetComponent<Button>().interactable = false;
            transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
            selectedSave = null;
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
        transform.Find("DeleteButton").GetComponent<Button>().interactable = true;
    }

    public void Load()
    {
        if(selectedSave != "" && selectedSave != null)
        {
            GameWorld.Load(selectedSave);
            Refresh();
            transform.Find("LoadButton").GetComponent<Button>().interactable = false;
            transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
            selectedSave = null;
        }
    }

    public void Back()
    {
        selectedSave = null;
        transform.Find("LoadButton").GetComponent<Button>().interactable = false;
        transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
        gameObject.SetActive(false);
        if (prevPanel != null)
        {
            prevPanel.SetActive(true);
        }
    }
}
