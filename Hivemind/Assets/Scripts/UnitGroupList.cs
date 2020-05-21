using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UnitGroupList
{
    private List<MindGroup> unitGroupList;

    private int MaxGroupCount = 6;

    public UnitGroupList(GameObject[] unitGroupObjects)
    {
        unitGroupList = new List<MindGroup>();

        foreach (var obj in unitGroupObjects)
        {
            unitGroupList.Add(new MindGroup(obj));
        }
    }

    public MindGroup GetMindGroupFromUnitId(Guid unitId)
    {
        foreach (var group in unitGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return group;
        }

        return null;
    }

    public UnitGroup GetUnitGroupFromUnitId(Guid unitId)
    {
        foreach (var group in unitGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return unit;
        }

        return null;
    }

    internal Guid CreateUnitGroup(GameObject unitIconBase)
    {
        for (var i = 0; i < unitGroupList.Count; i++)
            if (unitGroupList[i].Count < MaxGroupCount)
                return unitGroupList[i].AddUnit(new UnitGroup(unitIconBase));

        return Guid.Empty;
    }

    internal GroupIdChangedEventArgs MergeGroupIntoGroup(Guid mergeGroup, Guid intoGroup)
    {
        var oldUnitGroup = GetUnitGroupFromUnitId(mergeGroup);
        var newUnitGroup = GetUnitGroupFromUnitId(intoGroup);

        newUnitGroup.MergeGroupIntoThis(oldUnitGroup);

        DeleteUnitGroup(oldUnitGroup);

        return new GroupIdChangedEventArgs(mergeGroup, newUnitGroup.UnitGroupId);
    }

    internal UnitGroup GetUnitGroupFromUIObject(GameObject gameObject)
    {
        foreach (MindGroup group in unitGroupList)
        {
            var u = group.FindUnit(gameObject);
            if (u != null) return u;
        }
        return null;
    }

    internal void MoveUnitIntoGroup(UnitGroup unit, GameObject groupGameObject)
    {
        MindGroup oldGroup = null;
        MindGroup newGroup = null;
        foreach (var group in unitGroupList)
        {
            if (group.UnitExists(unit))
            {
                oldGroup = group;
            }
            if (group.Equals(groupGameObject))
            {
                newGroup = group;
            }
        }

        if (oldGroup.Equals(newGroup) || newGroup.Count >= MaxGroupCount)
        {
            oldGroup.UpdateLayout();
            return;
        }

        oldGroup.RemoveUnit(unit);
        newGroup.AddUnit(unit);
    }

    internal void UpdateLayout(UnitGroup unit)
    {
        foreach (var group in unitGroupList)
        {
            if (group.UnitExists(unit))
            {
                group.UpdateLayout();
                return;
            }
        }
    }

    public void DeleteUnitGroup(UnitGroup unitGroup)
    {
        GetMindGroupFromUnitId(unitGroup.UnitGroupId)?.RemoveUnit(unitGroup);

        UnityEngine.Object.Destroy(unitGroup.Ui_IconObj);
    }
 public MindGroup GetMindGroupFromIndex(int Index)
    {
        return unitGroupList[Index];
    }
}
