using System;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UiController : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    private MindGroup currentOpenMindGroup;

    private Vector2 lastMousePosition; // Used in calculating screen drag of icons

    [SerializeField] private GameObject mindBuilderPanel;

    [SerializeField] private TextMeshProUGUI resourceTextBox;

    [SerializeField] private UnitController unitController;

    private UnitGroup unitGroupObj; // The currently being dragged UnitGroup object

    public GameObject[] UnitGroupObjects; // The unit group UI GameObjects
    public GameObject unitIconBase;

    public AudioSource audioSrc;

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
            unitController.UnitGroupList.MoveUnit(unitGroupObj, groupResult.gameObject);
        else
            unitController.UnitGroupList.UpdateLayout(unitGroupObj);

        unitGroupObj = null;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-Unit"));

        if (result.isValid)
        {
            var group = unitController.UnitGroupList.GetUnitGroupFromUIObject(result.gameObject);
            if (group != null) unitGroupObj = group;
        }

        lastMousePosition = eventData.position;
    }

    private void Awake()
    {
        GameResources.OnResourceAmountChanged += delegate { UpdateResourceTextObject(); };
        UpdateResourceTextObject();
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
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
        // Hook the mindbuilder onto here
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

    public void UpdateVolume()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
    }
}