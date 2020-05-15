using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MindBuilderScript : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;

    //currently dragged mind object.
    private IMindUI mindTypeObj;

    public MindGroup mindGroup;

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-MindType"));

        if (result.isValid)
        {
            mindTypeObj = result.gameObject.GetComponent<IMindUI>();
        }

        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mindTypeObj == null) return;

        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = mindTypeObj.Ui_IconObj.GetComponent<RectTransform>();

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
        if (mindTypeObj == null) return;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var mindresult = results.Find(result => result.gameObject.CompareTag("Ui-Mind"));

        if (mindresult.isValid)
        {
            mindGroup.AddMind(mindTypeObj.MakeNewMind(), 0);
        }
        mindTypeObj.UpdateMindLayout();
        mindTypeObj = null;
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

    internal void ClearMind()
    {

    }



    internal void GenerateMind()
    {
        for (int i = 0; i < mindGroup.Count; i++)
        {
            mindGroup.Minds[i].GenerateUI();
        }
    }
}
