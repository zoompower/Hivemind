﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileButtonScript : MonoBehaviour
{
    [NonSerialized]
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
        GetComponentInParent<LoadMenuScript>().SetSelectedSaveFile(this);
        transform.GetComponent<Button>().interactable = false;
    }
}
