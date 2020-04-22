using UnityEngine;

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
    }
}
