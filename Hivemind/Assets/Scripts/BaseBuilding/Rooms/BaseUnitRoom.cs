using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnitRoom : BaseRoom
{
    internal UnitGroup unitGroup;

    internal Guid GroupId = Guid.Empty;

    internal bool Destructable = true;

    internal string UnitResource;

    internal bool AstarVisited = false;

    public override bool IsDestructable()
    {
        return Destructable;
    }

    public override bool IsRoom()
    {
        return true;
    }

    private void AddEventListeners()
    {
        unitController.OnGroupIdChange += ChangeGroupID;
    }

    private void RemoveEventListeners()
    {
        unitController.OnGroupIdChange -= ChangeGroupID;
    }

    private void ChangeGroupID(object sender, GroupIdChangedEventArgs e)
    {
        if (GroupId == e.oldGuid)
        {
            GroupId = e.newGuid;
            unitGroup = unitController.MindGroupList.GetUnitGroupFromUnitId(GroupId);
        }
    }

    private void Start()
    {
        unitController = FindObjectOfType<UnitController>();
        if (unitController == null)
        {
            throw new Exception("There is no unit controller present, please fix this issue.");
        }

        AddEventListeners();
        Astar.RegisterResetableRoom(this);

        bool createNew = true;

        foreach (var neighbor in transform.GetComponentInParent<BaseTile>().Neighbors)
        {
            if (neighbor.GetComponent<BaseTile>().RoomScript is BaseUnitRoom)
            {
                var other = neighbor.GetComponent<BaseTile>().RoomScript as BaseUnitRoom;
                if (other.GroupId == Guid.Empty)
                {
                    continue;
                }
                else
                {
                    if (createNew)
                    {
                        createNew = false;
                        GroupId = other.GroupId;
                    }
                    else if (other.GroupId != GroupId)
                    {
                        unitController.MergeGroupIntoGroup(other.GroupId, GroupId);
                    }
                }
            }
        }

        if (createNew)
        {
            GroupId = unitController.CreateUnitGroup();
        }

        AttachUnitGroup();

        unitGroup.AddMax();

        InvokeRepeating("CheckSpawnable", 1.0f, 1.0f);
    }

    internal void AttachUnitGroup()
    {
        unitGroup = unitController.MindGroupList.GetUnitGroupFromUnitId(GroupId);
    }

    private void CheckSpawnable()
    {
        if (unitGroup.AddUnit())
        {
            if (UnitResource != null)
            {
                GameObject ant = Instantiate(Resources.Load(UnitResource) as GameObject);

                GameObject container = GameObject.Find("Ants");
                if (container == null)
                {
                    throw new Exception("The \"Ants\" container was not found. Please add a container to save the Hierarchy from clutter!");
                }
                else
                {
                    ant.transform.SetParent(container.transform);
                }

                ant.GetComponent<Ant>().unitGroupID = GroupId;

                ant.GetComponent<NavMeshAgent>().Warp(transform.position);
            }
            else
            {
                throw new Exception($"The {GetType()} has no UnitResource set!");
            }
        }
    }

    private void SplitRoom()
    {
        List<BaseUnitRoom> sameNeighborList = new List<BaseUnitRoom>();

        foreach (var neighbor in transform.GetComponentInParent<BaseTile>()?.Neighbors)
        {
            BaseTile neighborTile = neighbor.GetComponent<BaseTile>();
            if (neighborTile.RoomScript != null && neighborTile.RoomScript.IsRoom() && neighborTile.RoomScript.GetType() == GetType())
            {
                sameNeighborList.Add(neighborTile.RoomScript as BaseUnitRoom);
            }
        }

        if (sameNeighborList.Count > 1 && sameNeighborList.Count < 5)
        {
            List<BaseUnitRoom> checkList = new List<BaseUnitRoom>(sameNeighborList);
            List<BaseUnitRoom> roomList = new List<BaseUnitRoom>();

            while (checkList.Count > 1)
            {
                roomList.Add(checkList[0]);

                List<BaseUnitRoom> swapList = new List<BaseUnitRoom>();
                for (int i = 1; i < checkList.Count; i++)
                {
                    if (!Astar.CanFind(checkList[0], checkList[i], new List<BaseUnitRoom>() { this }))
                    {
                        swapList.Add(checkList[i]);
                    }
                }
                checkList = swapList;
            }

            roomList.AddRange(checkList);

            if (roomList.Count > 1)
            {
                roomList.RemoveAt(0);
                FindObjectOfType<UnitController>().SplitUnitGroups(this, roomList);
            }
        }
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
        Astar.RemoveResetableRoom(this);
        CancelInvoke("CheckSpawnable");
    }

    public override void Destroy()
    {
        unitGroup?.RemoveMax();
        SplitRoom();
        Destroy(gameObject); 
    }
}