using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MindBuilderScript : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;

    public MindGroup mindGroup;

    //currently dragged mind object.
    private IMindUI mindTypeObj;

    public void OnDrag(PointerEventData eventData)
    {
        if (mindTypeObj == null) return;

        var currentMousePosition = eventData.position;
        var diff = currentMousePosition - lastMousePosition;
        var rect = mindTypeObj.Ui_IconObj.GetComponent<RectTransform>();

        var newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
        var oldPos = rect.position;
        rect.position = newPosition;
        if (!IsRectTransformInsideScreen(rect)) rect.position = oldPos;
        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (mindTypeObj == null) return;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var mindresult = results.Find(result => result.gameObject.CompareTag("Ui-Mind"));

        if (mindresult.isValid) mindGroup.AddMind(mindTypeObj.MakeNewMind(), 0);
        mindTypeObj.UpdateMindLayout();
        mindTypeObj = null;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-MindType"));

        if (result.isValid) mindTypeObj = result.gameObject.GetComponent<IMindUI>();

        lastMousePosition = eventData.position;
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

    internal void ClearMind()
    {
    }


    internal void GenerateMind()
    {
        for (var i = 0; i < mindGroup.Count; i++) mindGroup.Minds[i].GenerateUI();
    }
}