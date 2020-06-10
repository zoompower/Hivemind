using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MindGroupList
{
    public List<MindGroup> mindGroupList;

    private int MaxGroupCount = 6;

    public MindGroupList(GameObject[] unitGroupObjects)
    {
        mindGroupList = new List<MindGroup>();

        foreach (var obj in unitGroupObjects)
        {
            mindGroupList.Add(new MindGroup(obj));
            if (mindGroupList.Count == 1)
            {
                mindGroupList[0].Minds.Clear();
                mindGroupList[0].Minds.Add(new BaseGroupMind());
            }
        }
    }

    public MindGroup GetMindGroupFromUnitId(Guid unitId)
    {
        foreach (var group in mindGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return group;
        }

        return null;
    }

    public UnitGroup GetUnitGroupFromUnitId(Guid unitId)
    {
        foreach (var group in mindGroupList)
        {
            var unit = group.FindUnit(unitId);
            if (unit != null) return unit;
        }

        return null;
    }

    internal Guid CreateUnitGroup(GameObject unitIconBase)
    {
        for (var i = 0; i < mindGroupList.Count; i++)
            if (mindGroupList[i].Count < MaxGroupCount)
                return mindGroupList[i].AddUnit(new UnitGroup(unitIconBase));

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
        foreach (MindGroup group in mindGroupList)
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
        foreach (var group in mindGroupList)
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
        foreach (var group in mindGroupList)
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
        return mindGroupList[Index];
    }

    public List<MindGroup> GetMindGroupList()
    {
        return mindGroupList;
    }

    public void SetData(List<MindGroupData> mindGroupDatas, GameObject[] mindGroupIcons, GameObject unitIconBase)
    {
        foreach (MindGroup mindGroup in mindGroupList)
        {
            while (mindGroup.unitGroupList.Count > 0)
            {
                DeleteUnitGroup(mindGroup.unitGroupList.FirstOrDefault());
            }
        }
        mindGroupList = new List<MindGroup>();
        for (int i = 0; i < mindGroupDatas.Count; i++)
        {
            mindGroupList.Add(new MindGroup(mindGroupIcons[i]));
            mindGroupList[i].SetData(mindGroupDatas[i], mindGroupIcons[i], unitIconBase);
        }
    }
}