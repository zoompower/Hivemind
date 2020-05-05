using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        StringBuilder sb = new StringBuilder();
        foreach(ResourceType resourceType in (ResourceType[]) Enum.GetValues(typeof(ResourceType)))
        {
            if(resourceType != ResourceType.Unknown)
            {
                sb.Append($"{resourceType}: {GameResources.GetResourceAmount(resourceType)} \n");
            }
        }
        transform.Find("resourceAmount").GetComponent<Text>().text = sb.ToString();
    }
}
