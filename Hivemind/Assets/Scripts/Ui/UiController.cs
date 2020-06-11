using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private MindGroup currentOpenMindGroup;

    private Vector2 lastMousePosition; // Used in calculating screen drag of icons

    [SerializeField] private GameObject mindBuilderPanel;

    private GameObject mainCamera;

    [SerializeField] private List<BoxCollider> BoundingBoxes;

    [SerializeField] private TextMeshProUGUI resourceTextBox;

    [SerializeField] private GameObject EventDisplayer;

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
        if (FindObjectOfType<CameraController>() != null)
        {
            mainCamera = FindObjectOfType<CameraController>().gameObject;
        }
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
        //return gameobject to the mask
        unitGroupObj.Ui_IconObj.transform.parent = unitGroupObj.Ui_IconObj.transform.parent.GetChild(0).GetChild(0);
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var groupResult = results.Find(result => result.gameObject.CompareTag("UI-UnitGroup"));

        if (groupResult.isValid)
            unitController.MindGroupList.MoveUnitIntoGroup(unitGroupObj, groupResult.gameObject);
        else
            unitController.MindGroupList.UpdateLayout(unitGroupObj);

        unitGroupObj = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-Unit"));

        if (result.isValid)
        {
            var group = unitController.MindGroupList.GetUnitGroupFromUIObject(result.gameObject);
            if (group != null) unitGroupObj = group;
            //Remove the gameobject from the mask so it is visible outisde the mask
            unitGroupObj.Ui_IconObj.transform.parent = unitGroupObj.Ui_IconObj.transform.parent.parent.parent;
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
        var MBtabbed = mindBuilderPanel.GetComponent<MindBuilderTabbed>();
        MBtabbed.mindGroup = currentOpenMindGroup;
        MBtabbed.GenerateMind();
    }

    public void UI_CloseMindBuilder()
    {
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

    public IEnumerator UpdateEventText(string text, Color? color = null, float seconds = 3f)
    {
        float startSeconds = seconds;
        Text myText = EventDisplayer.GetComponent<Text>();
        myText.color = color ?? Color.black;
        myText.text = text;
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            Color newColor = myText.color;
            newColor.a = ((float)seconds * 2 / (float)startSeconds);
            myText.color = newColor;
            seconds -= 0.1f;
        }
        myText.text = "";
    }

    public void SetTime(float timeScale)
    {
        TimeController.Instance.SetTimeScale(timeScale);
    }

    private string FormatResource(string spriteName, int val)
    {
        return $" <sprite={spriteName}> ({val}/999)";
    }
}