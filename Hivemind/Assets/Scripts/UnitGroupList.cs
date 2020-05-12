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
        int i = 0;
        unitGroupList = new List<MindGroup>();
        foreach (var obj in unitGroupObjects)
        {
            i++;
            if (i == 3)
                unitGroupList.Add(new MindGroup(obj, new Gathering(ResourceType.Crystal, 5, Gathering.Direction.West)));
            else
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
        for (int i = 0; i < unitGroupList.Count; i++)
        {
            if (unitGroupList[i].Count < MaxGroupCount)
            {
                return unitGroupList[i].AddUnit(new UnitGroup(unitIconBase));
            }
        }

        return Guid.Empty;
    }

    internal UnitGroup GetUnitGroupFromUIObject(GameObject gameObject)
    {
        foreach (MindGroup group in unitGroupList)
        {
            var u = group.FindUnit(gameObject);
            if (u != null)
            {
                return u;
            }
        }
        return null;
    }

    internal void MoveUnit(UnitGroup unit, GameObject groupGameObject)
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
}