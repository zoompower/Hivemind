using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public MindGroupList MindGroupList { get; private set; }

    [HideInInspector]
    public UiController uiController;

    public event EventHandler<GroupIdChangedEventArgs> OnGroupIdChange;

    [HideInInspector]
    public int TeamId;

    // Property for overriding the default minds via the inspector menu
    public DataEditor[] MindDatas;

    private void Awake()
    {
        MindGroupList = new MindGroupList(FindObjectOfType<UiController>().UnitGroupObjects);
        MindGroupList.OverrideMinds(MindDatas);

        TeamId = GetComponent<BaseController>().TeamID;
    }

    private void Start()
    {
        if (TeamId == GameWorld.Instance.LocalTeamId)
        {
            uiController = FindObjectOfType<UiController>();
            uiController.RegisterUnitController(this);
        }
        GameWorld.Instance.AddUnitController(this);
    }

    public Guid CreateUnitGroup()
    {
        return MindGroupList.CreateUnitGroup(uiController?.unitIconBase);
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

    public void SplitUnitGroups(BaseTile invokingRoom, List<BaseTile> roomList)
    {
        var oldId = (roomList[0].RoomScript as BaseUnitRoom).GroupId;

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
                uGroup.SetCurrentUnits(uGroup.CurrentUnits - totalCount, true);
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

    public void SetData(List<MindGroupData> data)
    {
        MindGroupList.SetData(data, FindObjectOfType<UiController>().UnitGroupObjects, TeamId == GameWorld.Instance.LocalTeamId ? FindObjectOfType<UiController>().unitIconBase : null);
    }
}