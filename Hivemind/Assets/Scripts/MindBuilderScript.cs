using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MindBuilderScript : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;
    private IMind mindTypeObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        //List<RaycastResult> results = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(eventData, results);

        //var result = results.Find(rayRes => rayRes.gameObject.CompareTag("UI-MindType"));

        //if (result.isValid)
        //{
        //    mindTypeObj = 
        //}

        //lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (mindTypeObj == null) return;

        //Vector2 currentMousePosition = eventData.position;
        //Vector2 diff = currentMousePosition - lastMousePosition;
        //RectTransform rect = mindTypeObj.Ui_IconObj.GetComponent<RectTransform>();

        //Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
        //Vector3 oldPos = rect.position;
        //rect.position = newPosition;
        //if (!IsRectTransformInsideSreen(rect))
        //{
        //    rect.position = oldPos;
        //}
        //lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (mindTypeObj == null) return;

        //List<RaycastResult> results = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(eventData, results);

        //var mindresult = results.Find(result => result.gameObject.CompareTag("Ui-Mind"));

        //if (mindresult.isValid)
        //{
        //    unitController.UnitGroupList.MoveUnit(unitGroupObj, groupResult.gameObject);
        //}
        //else
        //{
        //    unitController.UnitGroupList.UpdateLayout(unitGroupObj);
        //}

        //mindTypeObj = null;
    }
}
