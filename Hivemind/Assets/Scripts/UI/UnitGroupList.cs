using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UnitGroupList
    {
        private List<MindGroup> unitGroupList;

        public UnitGroupList(GameObject[] unitGroupObjects)
        {
            unitGroupList = new List<MindGroup>();

            foreach (var obj in unitGroupObjects)
            {
                unitGroupList.Add(new MindGroup(obj));
            }
        }

        public System.Guid CreateUnitGroup(GameObject unitIconBase)
        {
            return unitGroupList[0].AddUnit(new UnitGroup(unitIconBase));
        }

        public MindGroup GetMindGroupFromUnitId(System.Guid unitId)
        {
            foreach (var group in unitGroupList)
            {
                var unit = group.FindUnit(unitId);
                if (unit != null) return group;
            }

            return null;
        }

        public UnitGroup GetUnitGroupFromUnitId(System.Guid unitId)
        {
            foreach (var group in unitGroupList)
            {
                var unit = group.FindUnit(unitId);
                if (unit != null) return unit;
            }

            return null;
        }

        internal UnitGroup GetUIUnit(GameObject gameObject)
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

            if (oldGroup.Equals(newGroup) || newGroup.Count >= 6)
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
}