using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Authors:
 * René Duivenvoorden
 */
public class MindGroup
{
    public int Count { get; private set; }

    private protected GameObject UIUnitGroup;

    private List<UnitGroup> unitList;

    public ResourceMind resMind { get; private set; }

    public CombatMind combatMind { get; private set; }

    public MindGroup(GameObject UiObject)
    {
        unitList = new List<UnitGroup>();

        UIUnitGroup = UiObject;
    }

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
        if (!unitList.Contains(unit))
        {
            unitList.Add(unit);

            unit.SetMindGroup(this);
            Count++;

            unit.Ui_IconObj.transform.SetParent(UIUnitGroup.transform, false);

            UpdateLayout();

            return unit.UnitGroupId;
        }

        return Guid.Empty;
    }

    internal bool RemoveUnit(UnitGroup unit)
    {
        if (unitList.Contains(unit))
        {
            unitList.Remove(unit);
            Count--;

            unit.Ui_IconObj.transform.SetParent(null, false);

            UpdateLayout();

            return true;
        }

        return false;
    }

    internal UnitGroup FindUnit(GameObject gameObject)
    {
        return unitList.Find(unitGroup => unitGroup.Ui_IconObj.Equals(gameObject));
    }

    internal UnitGroup FindUnit(Guid unitId)
    {
        return unitList.Find(unitGroup => unitGroup.UnitGroupId.Equals(unitId));
    }

    internal bool UnitExists(UnitGroup unit)
    {
        return unitList.Contains(unit);
    }

    internal void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(UIUnitGroup.GetComponent<RectTransform>());
    }
}