using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject MindBuilderPanel;

    [SerializeField]
    private GameObject UnitIconBase;

    [SerializeField]
    private GameObject[] UnitGroups;

    private List<UIUnit> UnitIconList;

    private Vector2 lastMousePosition;
    private UIUnit iconObj;

    private void Awake()
    {
        UnitIconList = new List<UIUnit>();
    }

    public void OpenGroup(int i)
    {
        Debug.Log($"Opening Mindbuilder for group {i}");
        UnitIconList.Add(new UIUnit(UnitIconBase, i));
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

    public GameObject GetUnitGroup(int i)
    {
        if (i >= UnitGroups.Length) return null;

        return UnitGroups[i];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        results.ForEach((result) => {
            if (result.gameObject.CompareTag("UI-Unit")) {
                var UiU = GetUIUnit(result.gameObject);
                if (UiU != null) iconObj = UiU;
            }
        });

        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (iconObj == null) return;

        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = iconObj.Object.GetComponent<RectTransform>();

        Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, transform.position.z);
        Vector3 oldPos = rect.position;
        rect.position = newPosition;
        if (!IsRectTransformInsideSreen(rect))
        {
            rect.position = oldPos;
        }
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (iconObj == null) return;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        results.ForEach((result) => {
            if (result.gameObject.CompareTag("UI-UnitGroup"))
            {
                Debug.Log("group: " + result);
                
            }
            else
            {

            }
        });

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetUnitGroup(iconObj.Group).GetComponent<RectTransform>());
        iconObj = null;
    }

    private bool IsRectTransformInsideSreen(RectTransform rectTransform)
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

    private UIUnit GetUIUnit(GameObject gameObject)
    {
        return UnitIconList.Find(UiU => UiU.Object.Equals(gameObject));
    }
}
