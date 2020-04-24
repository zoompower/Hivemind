using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UnitController : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private GameObject mindBuilderPanel;

        [SerializeField]
        public GameObject unitIconBase { get; private set; }

        [SerializeField]
        private GameObject[] unitGroupObjects;

        public UnitGroupList UnitGroupList { get; private set; }

        private Vector2 lastMousePosition;
        private UnitGroup unitObj;

        private void Awake()
        {
            UnitGroupList = new UnitGroupList(unitGroupObjects);
        }

        public void OpenGroup(int i)
        {
            OpenMindBuilder();

            UnitGroupList.CreateUnitGroup(unitIconBase);
        }

        private void OpenMindBuilder()
        {
            mindBuilderPanel.SetActive(true);
        }

        public void CloseMindBuilder()
        {
            mindBuilderPanel.SetActive(false);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            var result = results.Find(res => res.gameObject.CompareTag("UI-Unit"));

            if (result.isValid)
            {
                var UiU = UnitGroupList.GetUIUnit(result.gameObject);
                if (UiU != null) unitObj = UiU;
            }

            lastMousePosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (unitObj == null) return;

            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;
            RectTransform rect = unitObj.gameObject.GetComponent<RectTransform>();

            Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, 0);
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
            if (unitObj == null) return;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            var groupResult = results.Find(result => result.gameObject.CompareTag("UI-UnitGroup"));

            if (groupResult.isValid)
            {
                UnitGroupList.MoveUnit(unitObj, groupResult.gameObject);
            }
            else
            {
                UnitGroupList.UpdateLayout(unitObj);
            }

            unitObj = null;
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
    }
}