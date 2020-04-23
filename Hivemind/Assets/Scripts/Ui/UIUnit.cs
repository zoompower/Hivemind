using UnityEngine;

public class UIUnit
{
    public int MaxUnits = 99;
    public int CurrentUnits = 50;

    public GameObject Object;

    public int Group;

    public UIUnit(GameObject unitIconBase, int unitGroup)
    {
        var icon = UnityEngine.Object.Instantiate(unitIconBase);

        UnitController controller = UnityEngine.Object.FindObjectOfType<UnitController>();

        Group = unitGroup;

        icon.transform.SetParent(controller.GetUnitGroup(Group).transform, false);

        Object = icon;
    }
}
