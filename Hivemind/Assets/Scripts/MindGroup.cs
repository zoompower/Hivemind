using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MindGroup
{
    private protected GameObject UIUnitGroup;

    public List<UnitGroup> unitGroupList;

    public MindGroup(GameObject UiObject)
    {
        unitGroupList = new List<UnitGroup>();
        Minds = new List<IMind>();
        Minds.Add(new Gathering());
        Minds.Add(new CombatMind());
        UIUnitGroup = UiObject;
    }

    public int Count { get; private set; }

    public List<IMind> Minds;

    public int MindPoints { get; set; }

    public bool Equals(MindGroup other)
    {
        return UIUnitGroup.Equals(other.UIUnitGroup);
    }

    public bool Equals(GameObject groupObject)
    {
        if (UIUnitGroup.Equals(groupObject))
        {
            return true;
        }
        else
        {
            return UIUnitGroup.Equals(groupObject.transform.GetChild(0).GetChild(0).gameObject);
        }
    }

    public Guid AddUnit(UnitGroup unit)
    {
        if (!unitGroupList.Contains(unit))
        {
            unitGroupList.Add(unit);

            Count++;

            unit.Ui_IconObj.transform.SetParent(UIUnitGroup.transform, false);

            UpdateLayout();

            return unit.UnitGroupId;
        }

        return Guid.Empty;
    }

    internal bool RemoveUnit(UnitGroup unit)
    {
        if (unitGroupList.Contains(unit))
        {
            unitGroupList.Remove(unit);
            Count--;

            unit.Ui_IconObj.transform.SetParent(null, false);

            UpdateLayout();

            return true;
        }

        return false;
    }

    internal UnitGroup FindUnit(GameObject gameObject)
    {
        return unitGroupList.Find(unitGroup => unitGroup.Ui_IconObj.Equals(gameObject));
    }

    internal UnitGroup FindUnit(Guid unitId)
    {
        return unitGroupList.Find(unitGroup => unitGroup.UnitGroupId.Equals(unitId));
    }

    internal bool UnitExists(UnitGroup unit)
    {
        return unitGroupList.Contains(unit);
    }

    internal void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(UIUnitGroup.GetComponent<RectTransform>());
    }

    public void AddMind(IMind mind, int Position)
    {
        Minds[Position] = mind;
    }

    public MindGroupData GetData()
    {
        return new MindGroupData(unitGroupList, Count, Minds, MindPoints);
    }

    public void SetData(MindGroupData data, GameObject parent, GameObject UiObject)
    {
        Minds = data.Minds;
        Count = data.Count;
        MindPoints = data.MindPoints;
        for (int i = 0; i < Minds.Count; i++)
        {
            Minds[i].SetData(data.MindData[i]);
        }
        unitGroupList = new List<UnitGroup>();
        foreach (UnitGroupData unitGroupData in data.UnitGroupDataList)
        {
            UnitGroup newUnitGroup = new UnitGroup(UiObject);
            newUnitGroup.Ui_IconObj.transform.SetParent(parent.transform, false);
            newUnitGroup.SetData(unitGroupData);
            unitGroupList.Add(newUnitGroup);
            UpdateLayout();
        }
    }
}