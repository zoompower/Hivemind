using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


 [Serializable]
public class MindGroup
{
    private protected GameObject UIUnitGroup;

    private readonly List<UnitGroup> unitGroupList;

    public MindGroup(GameObject UiObject)
    {
        unitGroupList = new List<UnitGroup>();
        Minds = new List<IMind>();
        Minds.Add(new Gathering());
        Minds.Add(new CombatMind());
        UIUnitGroup = UiObject;
    }

    public int Count { get; private set; }

    public List<IMind> Minds { get; }

    public int MindPoints { get; set; }

    public bool Equals(MindGroup other)
    {
        return UIUnitGroup.Equals(other.UIUnitGroup);
    }

    public bool Equals(GameObject groupObject)
    {
        return UIUnitGroup.Equals(groupObject);
    }

    internal Guid AddUnit(UnitGroup unit)
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
}