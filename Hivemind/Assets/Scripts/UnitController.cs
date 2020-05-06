using System;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UnitController : MonoBehaviour
{
    public UnitGroupList UnitGroupList { get; private set; }

    [SerializeField]
    private UiController uiController;

    private void Awake()
    {
        UnitGroupList = new UnitGroupList(uiController.UnitGroupObjects);
    }

    public Guid CreateUnitGroup()
    {
        return UnitGroupList.CreateUnitGroup(uiController.unitIconBase);
    }

    public void SetCurrentUnits(Guid unitGroupId, int amount)
    {
        var group = UnitGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.SetCurrentUnits(amount);
        }
    }

    public void SetMaxUnits(Guid unitGroupId, int amount)
    {
        var group = UnitGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.SetMaxUnits(amount);
        }
    }
}