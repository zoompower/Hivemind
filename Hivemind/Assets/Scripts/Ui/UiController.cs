using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UiController : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject unitIconBase;

    public GameObject[] UnitGroupObjects; // The unit group UI GameObjects

    [SerializeField]
    private TextMeshProUGUI resourceTextBox;

    [SerializeField]
    private UnitController unitController;
    [SerializeField]
    private GameObject mindBuilderPanel;

    private Vector2 lastMousePosition; // Used in calculating screen drag of icons
    private UnitGroup unitGroupObj; // The currently being dragged UnitGroup object

    private void Awake()
    {
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e)
        {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }

    public void UI_OpenMindBuilder(int i)
    {
        mindBuilderPanel.SetActive(true);
        // Hook the mindbuilder onto here
    }

    public void UI_CloseMindBuilder()
    {
        mindBuilderPanel.SetActive(false);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-Unit"));

        if (result.isValid)
        {
            var group = unitController.UnitGroupList.GetUnitGroupFromUIObject(result.gameObject);
            if (group != null) unitGroupObj = group;
        }

        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (unitGroupObj == null) return;

        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = unitGroupObj.Ui_IconObj.GetComponent<RectTransform>();

        Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
        Vector3 oldPos = rect.position;
        rect.position = newPosition;
        if (!IsRectTransformInsideScreen(rect))
        {
            rect.position = oldPos;
        }
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (unitGroupObj == null) return;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var groupResult = results.Find(result => result.gameObject.CompareTag("UI-UnitGroup"));

        if (groupResult.isValid)
        {
            unitController.UnitGroupList.MoveUnit(unitGroupObj, groupResult.gameObject);
        }
        else
        {
            unitController.UnitGroupList.UpdateLayout(unitGroupObj);
        }

        unitGroupObj = null;
    }

    private bool IsRectTransformInsideScreen(RectTransform rectTransform)
    {
        bool isInside = false;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCorners = 0;
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        foreach (Vector3 corner in corners)
        {
            if (rect.Contains(corner))
            {
                visibleCorners++;
            }
        }

        if (visibleCorners == 4)
        {
            isInside = true;
        }

        return isInside;
    }

    private void UpdateResourceTextObject()
    {
        StringBuilder sb = new StringBuilder();

        foreach (ResourceType resourceType in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
        {
            if (resourceType != ResourceType.Unknown)
            {
                sb.Append($" {resourceType}: {GameResources.GetResourceAmount(resourceType)}");
            }
        }

        resourceTextBox.text = sb.ToString();
    }

    private string FormatResource(string spriteName, int val)
    {
        return $" <sprite={spriteName}> ({val}/999)";
    }
}