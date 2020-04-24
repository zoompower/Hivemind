using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MindGroup
    {
        private protected GameObject UIUnitGroup;

        private List<UnitGroup> unitList;

        internal int Count { get; private set; }

        internal MindGroup(GameObject UiObject)
        {
            unitList = new List<UnitGroup>();

            UIUnitGroup = UiObject;
        }

        internal bool Equals(MindGroup other)
        {
            return UIUnitGroup.Equals(other.UIUnitGroup);
        }

        internal bool Equals(GameObject groupObject)
        {
            return UIUnitGroup.Equals(groupObject);
        }

        internal System.Guid AddUnit(UnitGroup unit)
        {
            if (!unitList.Contains(unit))
            {
                unitList.Add(unit);
                Count++;

                unit.gameObject.transform.SetParent(UIUnitGroup.transform, false);

                UpdateLayout();

                return unit.ID;
            }

            return new System.Guid();
        }

        internal bool RemoveUnit(UnitGroup unit)
        {
            if (unitList.Contains(unit))
            {
                unitList.Remove(unit);
                Count--;

                unit.gameObject.transform.SetParent(null, false);

                UpdateLayout();

                return true;
            }

            return false;
        }

        internal UnitGroup FindUnit(GameObject gameObject)
        {
            return unitList.Find(u => u.gameObject.Equals(gameObject));
        }

        internal UnitGroup FindUnit(System.Guid unitId)
        {
            return unitList.Find(u => u.ID.Equals(unitId));
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
}
