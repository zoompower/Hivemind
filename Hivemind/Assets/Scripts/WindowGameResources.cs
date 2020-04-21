using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGameResources : MonoBehaviour
{
    private void Awake()
    {
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e)
        {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }
    private void UpdateResourceTextObject()
    {
        transform.Find("resourceAmount").GetComponent<Text>().text = "Resources: " + GameResources.GetResourceAmount();
    }
}
