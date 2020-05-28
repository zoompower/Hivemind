﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiController : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private MindGroup currentOpenMindGroup;

    private Vector2 lastMousePosition; // Used in calculating screen drag of icons

    [SerializeField] private GameObject mindBuilderPanel;

    [SerializeField] private GameObject mainCamera;

    [SerializeField] private List<BoxCollider> BoundingBoxes;

    [SerializeField] private TextMeshProUGUI resourceTextBox;

    private UnitController unitController;

    private UnitGroup unitGroupObj; // The currently being dragged UnitGroup object

    public GameObject[] UnitGroupObjects; // The unit group UI GameObjects
    public GameObject unitIconBase;

    private List<RectTransform> miniMaps;

    public void OnDrag(PointerEventData eventData)
    {
        if (unitGroupObj == null) return;

        var currentMousePosition = eventData.position;
        var diff = currentMousePosition - lastMousePosition;
        var rect = unitGroupObj.Ui_IconObj.GetComponent<RectTransform>();

        var newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
        var oldPos = rect.position;
        rect.position = newPosition;
        if (!IsRectTransformInsideScreen(rect)) rect.position = oldPos;
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (unitGroupObj == null) return;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var groupResult = results.Find(result => result.gameObject.CompareTag("UI-UnitGroup"));

        if (groupResult.isValid)
            unitController.MindGroupList.MoveUnitIntoGroup(unitGroupObj, groupResult.gameObject);
        else
            unitController.MindGroupList.UpdateLayout(unitGroupObj);

        unitGroupObj = null;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-Unit"));

        if (result.isValid)
        {
            var group = unitController.MindGroupList.GetUnitGroupFromUIObject(result.gameObject);
            if (group != null) unitGroupObj = group;
        }

        lastMousePosition = eventData.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var mousePosition = eventData.position;
        var clicked = InMiniMapClick(mousePosition);
        if (clicked.Item1)
        {
            var boundingBox = BoundingBoxes[clicked.Item2];

            var cameraX = (boundingBox.bounds.size.x) * clicked.Item3;
            var cameraZ = (boundingBox.bounds.size.z) * clicked.Item4;

            mainCamera.transform.position = new Vector3(boundingBox.bounds.min.x + cameraX, mainCamera.transform.position.y, boundingBox.bounds.max.z - cameraZ);
        }
    }

    private (bool, int, float, float) InMiniMapClick(Vector2 mousePosition)
    {
        for (int i = 0; i < miniMaps.Count; i++)
        {
            var globalPos = new Vector2(miniMaps[i].transform.position.x, miniMaps[i].transform.position.y);
            var localPosition = mousePosition - (globalPos - (new Vector2(-miniMaps[i].rect.xMin, 0)) * 0.85f);

            var scaledX = localPosition.x / (miniMaps[i].rect.width * 0.85f);
            var scaledY = 1 - (localPosition.y / (miniMaps[i].rect.height * 0.85f));

            if (scaledX < 1 && scaledX > 0 && scaledY < 1 && scaledY > 0)
            {
                return (true, i, scaledX, scaledY);
            }
        }
        return (false, -1, -1f, -1f);
    }

    private void Awake()
    {
        unitController = FindObjectOfType<UnitController>();
        GameResources.OnResourceAmountChanged += delegate { UpdateResourceTextObject(); };
        UpdateResourceTextObject();
    }

    private void Start()
    {
        miniMaps = GetComponentsInChildren<RectTransform>().Where(x => x.CompareTag("UI-MiniMap")).ToList();
    }

    public void UI_OpenMindBuilder(int i)
    {
        mindBuilderPanel.SetActive(true);
        currentOpenMindGroup = unitController.GetMindGroup(i);
        var MBScript = mindBuilderPanel.GetComponent<MindBuilderScript>();
        if (MBScript == null)
        {
            var MBtabbed = mindBuilderPanel.GetComponent<MindBuilderTabbed>();
            MBtabbed.mindGroup = currentOpenMindGroup;
            MBtabbed.GenerateMind();
            return;
        }

        MBScript.mindGroup = currentOpenMindGroup;
        MBScript.GenerateMind();
    }

    public void UI_CloseMindBuilder()
    {
        var MBScript = mindBuilderPanel.GetComponent<MindBuilderScript>();
        if (MBScript == null)
        {
            var MBtabbed = mindBuilderPanel.GetComponent<MindBuilderTabbed>();
            mindBuilderPanel.SetActive(false);
            return;
        }

        MBScript.ClearMind();
        currentOpenMindGroup = null;
        mindBuilderPanel.SetActive(false);
    }

    private bool IsRectTransformInsideScreen(RectTransform rectTransform)
    {
        var isInside = false;
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        var visibleCorners = 0;
        var rect = new Rect(0, 0, Screen.width, Screen.height);

        foreach (var corner in corners)
            if (rect.Contains(corner))
                visibleCorners++;

        if (visibleCorners == 4) isInside = true;

        return isInside;
    }

    private void UpdateResourceTextObject()
    {
        var sb = new StringBuilder();

        foreach (var resourceType in (ResourceType[]) Enum.GetValues(typeof(ResourceType)))
            if (resourceType != ResourceType.Unknown)
                sb.Append($" {resourceType}: {GameResources.GetResourceAmount(resourceType)}");

        resourceTextBox.text = sb.ToString();
    }

    private string FormatResource(string spriteName, int val)
    {
        return $" <sprite={spriteName}> ({val}/999)";
    }
}