using System;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UnitController : MonoBehaviour
{
    public UnitGroupList UnitGroupList { get; private set; }

    private UiController uiController;

    public event EventHandler<GroupIdChangedEventArgs> OnGroupIdChange;

    private void Awake()
    {
        uiController = FindObjectOfType<UiController>();
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

    public void MergeGroupIntoGroup(Guid mergeGroup, Guid intoGroup)
    {
        OnGroupIdChange.Invoke(null, UnitGroupList.MergeGroupIntoGroup(mergeGroup, intoGroup));
    }

    public void OnUnitDestroy(Guid unitGroupId)
    {
        var group = UnitGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.RemoveUnit();

            if (group.MaxUnits <= 0 && group.CurrentUnits <= 0)
            {
                UnitGroupList.DeleteUnitGroup(group);
            }
        }
    }

    public MindGroup GetMindGroup(int Index)
    {
        return UnitGroupList.GetMindGroupFromIndex(Index);
    }
}
