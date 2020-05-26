using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public MindGroupList MindGroupList { get; private set; }

    private UiController uiController;

    public event EventHandler<GroupIdChangedEventArgs> OnGroupIdChange;

    private void Awake()
    {
        uiController = FindObjectOfType<UiController>();
        MindGroupList = new MindGroupList(uiController.UnitGroupObjects);
    }

    public Guid CreateUnitGroup()
    {
        return MindGroupList.CreateUnitGroup(uiController.unitIconBase);
    }

    public void SetCurrentUnits(Guid unitGroupId, int amount)
    {
        var group = MindGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.SetCurrentUnits(amount);
        }
    }

    public void SetMaxUnits(Guid unitGroupId, int amount)
    {
        var group = MindGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.SetMaxUnits(amount);
        }
    }

    public void MergeGroupIntoGroup(Guid mergeGroup, Guid intoGroup)
    {
        OnGroupIdChange.Invoke(null, MindGroupList.MergeGroupIntoGroup(mergeGroup, intoGroup));
    }

    public void SplitUnitGroups(BaseUnitRoom invokingRoom, List<BaseUnitRoom> roomList)
    {
        var oldId = roomList[0].GroupId;

        Dictionary<Guid, int> IdAndRoomSize = new Dictionary<Guid, int>();

        foreach (var room in roomList)
        {
            var newId = CreateUnitGroup();
            var count = Astar.ConvertRoom(room, newId, invokingRoom);
            IdAndRoomSize.Add(newId, count);
        }

        var antContainer = GameObject.Find("Ants");
        List<Ant> ants = new List<Ant>();

        foreach (Transform child in antContainer.transform)
        {
            if (child.GetComponent<Ant>() != null)
            {
                if (child.GetComponent<Ant>().unitGroupID == oldId)
                    ants.Add(child.GetComponent<Ant>());
            }
            else
            {
                throw new Exception("The ant doesnt have the required script!");
            }
        }

        IdAndRoomSize.Add(oldId, 0);
        int totalCount = 0;
        foreach (var pair in IdAndRoomSize)
        {
            var uGroup = MindGroupList.GetUnitGroupFromUnitId(pair.Key);

            if (pair.Value == 0)
            {
                uGroup.SetMaxUnits(uGroup.MaxUnits - totalCount);
                uGroup.SetCurrentUnits(uGroup.MaxUnits + 1, true);
            }
            else
            {
                uGroup.SetMaxUnits(pair.Value);
                uGroup.SetCurrentUnits(pair.Value);                
            }

            for (int i = totalCount; i < totalCount + pair.Value; i++)
            {
                ants[i].unitGroupID = pair.Key;
            }

            totalCount += pair.Value;
        }
    }

    public void OnUnitDestroy(Guid unitGroupId)
    {
        var group = MindGroupList.GetUnitGroupFromUnitId(unitGroupId);

        if (group != null)
        {
            group.RemoveUnit();

            if (group.MaxUnits <= 0 && group.CurrentUnits <= 0)
            {
                MindGroupList.DeleteUnitGroup(group);
            }
        }
    }

    public MindGroup GetMindGroup(int Index)
    {
        return MindGroupList.GetMindGroupFromIndex(Index);
    }
}
