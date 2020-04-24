using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TabController : MonoBehaviour
    {
        [SerializeField]
        private GameObject UnitsTab;

        [SerializeField]
        private GameObject BuildingTab;

        public void ShowUnitTab()
        {
            UnitsTab.SetActive(true);
            BuildingTab.SetActive(false);
        }

        public void ShowBuildingTab()
        {
            UnitsTab.SetActive(false);
            BuildingTab.SetActive(true);
            UnitsTab.GetComponent<UnitController>().CloseMindBuilder();
        }

        public void TestButton()
        {
            var currentEventSystem = EventSystem.current;
            if (currentEventSystem == null) { return; }

            var currentSelectedGameObject = currentEventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject == null) { return; }

            Debug.Log(currentSelectedGameObject.name);
        }
    }
}