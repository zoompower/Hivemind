using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject prevPanel;

    [SerializeField]
    private GameObject contentBox;

    [SerializeField]
    private InputField newSaveInput;

    private string selectedSave;

    public void Refresh()
    {
        foreach (Transform child in contentBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        FileInfo[] info = new DirectoryInfo(GameWorld.Instance.GetSavePath()).GetFiles().OrderByDescending(p => p.LastWriteTime).ToArray();
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
            GameWorld.Instance.DeleteFile(selectedSave);
            Refresh();
            transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
            transform.Find("SaveButton").GetComponent<Button>().interactable = false;
            selectedSave = null;
        }
    }

    public void SetSelectedNewSaveFile()
    {
        if (newSaveInput.text != "")
        {
            foreach (Transform child in contentBox.transform)
            {
                child.gameObject.GetComponent<Button>().interactable = true;
            }
            selectedSave = newSaveInput.text;
            transform.Find("SaveButton").GetComponent<Button>().interactable = true;
            transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
        }
    }

    public void SetSelectedSaveFile(SaveFileButtonScript buttonScript)
    {
        foreach (Transform child in contentBox.transform)
        {
            child.gameObject.GetComponent<Button>().interactable = true;
        }
        selectedSave = buttonScript.SaveFile;
        transform.Find("SaveButton").GetComponent<Button>().interactable = true;
        transform.Find("DeleteButton").GetComponent<Button>().interactable = true;
    }

    public void Save()
    {
        if (selectedSave != "" && selectedSave != null)
        {
            GameWorld.Instance.Save(selectedSave);
            Refresh();
            transform.Find("SaveButton").GetComponent<Button>().interactable = false;
            transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
            selectedSave = null;
        }
    }

    public void Back()
    {
        newSaveInput.text = null;
        selectedSave = null;
        transform.Find("SaveButton").GetComponent<Button>().interactable = false;
        transform.Find("DeleteButton").GetComponent<Button>().interactable = false;
        gameObject.SetActive(false);
        if (prevPanel != null)
        {
            prevPanel.SetActive(true);
        }
    }
}