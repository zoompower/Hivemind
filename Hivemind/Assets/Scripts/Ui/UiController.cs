using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiController : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private MindGroup currentOpenMindGroup;

    private Vector2 lastMousePosition; // Used in calculating screen drag of icons

    [SerializeField] private GameObject mindBuilderPanel;

    private GameObject mainCamera;

    [SerializeField] private List<BoxCollider> BoundingBoxes;

    [SerializeField] private TextMeshProUGUI resourceTextBox;

    private UnitController unitController;

    private UnitGroup unitGroupObj; // The currently being dragged UnitGroup object

    public GameObject[] UnitGroupObjects; // The unit group UI GameObjects
    public GameObject unitIconBase;

    private List<RectTransform> miniMaps;
    private Vector2 referenceResolution;

    private ColorBlock UnselectedToolButtonColor;
    [SerializeField]
    private ColorBlock SelectedToolButtonColor;

    [SerializeField]
    private Button DefaultToolButton;
    [SerializeField]
    private Button AntRoomToolButton;
    [SerializeField]
    private Button DestroyToolButton;


    private void Awake()
    {
        unitController = FindObjectOfType<UnitController>();
        GameResources.OnResourceAmountChanged += delegate { UpdateResourceTextObject(); };
        UpdateResourceTextObject();

        BaseController[] controllers = UnityEngine.Object.FindObjectsOfType<BaseController>();

        foreach (BaseController controller in controllers)
        {
            if (controller.TeamID == 0)
            {
                controller.OnToolChanged += OnToolChanged;
                break;
            }
        }
    }

    private void Start()
    {
        mainCamera = FindObjectOfType<CameraController>().gameObject;
        miniMaps = GetComponentsInChildren<RectTransform>().Where(x => x.CompareTag("UI-MiniMap")).ToList();

        //get the default resolution
        CanvasScaler canvasScaler = miniMaps[0].GetComponentInParent<CanvasScaler>();
        referenceResolution = canvasScaler.referenceResolution;
    }

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
        Vector2 mousePosition = eventData.position;
        (bool, int, float, float) clicked = InMiniMapClick(mousePosition);
        if (clicked.Item1)
        {
            //get the corrosponding bounding box of the minimap
            BoxCollider boundingBox = BoundingBoxes[clicked.Item2];

            //calculate camera position in the minimap from the starting point of the bounding box
            float cameraX = (boundingBox.bounds.size.x) * clicked.Item3;
            float cameraZ = (boundingBox.bounds.size.z) * clicked.Item4;

            //add everything together and make the new position
            mainCamera.transform.position = new Vector3(boundingBox.bounds.min.x + cameraX, mainCamera.transform.position.y, boundingBox.bounds.max.z - cameraZ);
        }
    }

    private (bool, int, float, float) InMiniMapClick(Vector2 mousePosition)
    {
        //calculate how small compared to default resolution it is
        Vector2 currentResolution = miniMaps[0].GetComponentInParent<Canvas>().pixelRect.size;
        float scalingMultiplierX = currentResolution.x / referenceResolution.x;
        float scalingMultiplierY = currentResolution.y / referenceResolution.y;

        //check all minimaps
        for (int i = 0; i < miniMaps.Count; i++)
        {
            //get the local position relative to the minimap
            Vector2 globalPos = new Vector2(miniMaps[i].transform.position.x, miniMaps[i].transform.position.y);
            Vector2 localPosition = mousePosition - (globalPos - (new Vector2(-miniMaps[i].rect.xMin, 0) * scalingMultiplierX));

            //get the scale from 0-1 where in the minimap UI the mouse was 
            //if bigger than 1 or smaller than 0 it means it was outside of the minimap element
            float scaledX = localPosition.x / (miniMaps[i].rect.width * scalingMultiplierX);
            float scaledY = 1 - (localPosition.y / (miniMaps[i].rect.height * scalingMultiplierY));

            //check if the mouse was inside the minimap UI
            if (scaledX < 1 && scaledX > 0 && scaledY < 1 && scaledY > 0)
            {
                //return the number of the minimap and also the scale of where the mouse is so it does not have to be calculated again
                return (true, i, scaledX, scaledY);
            }
        }
        //return a false tuple
        return (false, -1, -1f, -1f);
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

        foreach (var resourceType in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
            if (resourceType != ResourceType.Unknown)
                sb.Append($" {resourceType}: {GameResources.GetResourceAmount(resourceType)}");

        resourceTextBox.text = sb.ToString();
    }

    private void OnToolChanged(object sender, ToolChangedEventArgs args)
    {
        
        if (args.newTool != args.oldTool)
        {
            switch (args.oldTool)
            {
                case BaseBuildingTool.Default:
                    DefaultToolButton.colors = UnselectedToolButtonColor;
                    break;
                case BaseBuildingTool.DestroyRoom:
                    DestroyToolButton.colors = UnselectedToolButtonColor;
                    break;
                case BaseBuildingTool.Wall:
                    // TODO: when building walls is gonna be a thing
                    break;
                case BaseBuildingTool.AntRoom:
                    AntRoomToolButton.colors = UnselectedToolButtonColor;
                    break;
            }
        }

        if (UnselectedToolButtonColor == default || args.newTool != args.oldTool)
        {
            switch (args.newTool)
            {
                case BaseBuildingTool.Default:
                    UnselectedToolButtonColor = DefaultToolButton.colors;
                    DefaultToolButton.colors = SelectedToolButtonColor;
                    break;
                case BaseBuildingTool.DestroyRoom:
                    UnselectedToolButtonColor = DestroyToolButton.colors;
                    DestroyToolButton.colors = SelectedToolButtonColor;
                    break;
                case BaseBuildingTool.Wall:
                    // TODO: when building walls is gonna be a thing
                    break;
                case BaseBuildingTool.AntRoom:
                    UnselectedToolButtonColor = AntRoomToolButton.colors;
                    AntRoomToolButton.colors = SelectedToolButtonColor;
                    break;
            }
        }
    }

    private string FormatResource(string spriteName, int val)
    {
        return $" <sprite={spriteName}> ({val}/999)";
    }
}