using System;
using UnityEngine;

[Serializable]
public class UnitController : MonoBehaviour
{
    public UnitGroupList UnitGroupList { get; private set; }

    [SerializeField]
    private UiController uiController;

    private void Awake()
    {
        UnitGroupList = new UnitGroupList(uiController.UnitGroupObjects);
        GameWorld.SetUnitController(this);
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

    public MindGroup GetMindGroup(int Index)
    {
        return UnitGroupList.GetMindGroupFromIndex(Index);
    }

    public void LoadData(UnitController data)
    {
        UnitGroupList = data.UnitGroupList;
    }
}