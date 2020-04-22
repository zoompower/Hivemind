using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private int[] Group1;

    [SerializeField]
    private GameObject MindBuilderPanel;

    public void OpenGroup(int i)
    {
        Debug.Log($"Opening Mindbuilder for group {i}");
        OpenMindBuilder();
    }

    private void OpenMindBuilder()
    {
        MindBuilderPanel.SetActive(true);
    }

    public void CloseMindBuilder()
    {
        MindBuilderPanel.SetActive(false);
    }
}
