using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class UnitGroupList
{
    private List<MindGroup> MindGroupList;

    private int MaxGroupCount = 6;

    public UnitGroupList(GameObject[] unitGroupObjects)
    {
        MindGroupList = new List<MindGroup>();

        foreach (var obj in unitGroupObjects)
        {
            MindGroupList.Add(new MindGroup(obj));
        }
    }

    public MindGroup GetMindGroupFromUnitId(Guid unitId)
    {
        foreach (var group in MindGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return group;
        }

        return null;
    }

    public UnitGroup GetUnitGroupFromUnitId(Guid unitId)
    {
        foreach (var group in MindGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return unit;
        }

        return null;
    }

    internal Guid CreateUnitGroup(GameObject unitIconBase)
    {
        for (int i = 0; i < MindGroupList.Count; i++)
        {
            if (MindGroupList[i].Count < MaxGroupCount)
            {
                return MindGroupList[i].AddUnit(new UnitGroup(unitIconBase));
            }
        }

        return Guid.Empty;
    }

    internal GroupIdChangedEventArgs MergeGroupIntoGroup(Guid group, Guid intoGroup)
    {
        var oldUnitGroup = GetUnitGroupFromUnitId(group);
        var newUnitGroup = GetUnitGroupFromUnitId(intoGroup);

        newUnitGroup.MergeGroupIntoThis(oldUnitGroup);

        DeleteUnitGroup(oldUnitGroup);

        return new GroupIdChangedEventArgs(group, newUnitGroup.UnitGroupId);
    }

    internal UnitGroup GetUnitGroupFromUIObject(GameObject gameObject)
    {
        foreach (MindGroup group in MindGroupList)
        {
            var u = group.FindUnit(gameObject);
            if (u != null)
            {
                return u;
            }
        }
        return null;
    }

    internal void MoveUnitIntoGroup(UnitGroup unit, GameObject groupGameObject)
    {
        MindGroup oldGroup = null;
        MindGroup newGroup = null;
        foreach (var group in MindGroupList)
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
        foreach (var group in MindGroupList)
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
}