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

    private BaseTile Parent;

    private int TeamId;

    internal Dictionary<ResourceType, int> RespawnCost;
    private bool singleFree = false;

    internal int RespawnTimer;
    internal int DefaultRespawnTime;

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
        if(TeamId == GameWorld.Instance.LocalTeamId)
        {
        unitController.OnGroupIdChange -= ChangeGroupID;
        }
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
        unitController = transform.parent.GetComponentInParent<UnitController>();
        baseController = transform.parent.GetComponentInParent<BaseController>();
        if (unitController == null)
        {
            throw new Exception("There is no unit controller present on one of the base containers, please fix this issue.");
        }

        AddEventListeners();
        Parent = GetComponentInParent<BaseTile>();
        TeamId = Parent.GetComponentInParent<BaseController>().TeamID;

        if (GroupId == Guid.Empty)
        {
            bool createNew = true;

            foreach (var neighbor in Parent.Neighbors)
            {
                if (neighbor.RoomScript is BaseUnitRoom)
                {
                    var other = neighbor.RoomScript as BaseUnitRoom;
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
        }

        AttachUnitGroup();

        InvokeRepeating("CheckSpawnable", 1.0f, 1.0f);

        if (!Parent.Loaded)
        {
            //singleFree = true;
        }
        baseController.GetGameResources().AddResources(RespawnCost);
    }

    internal void AttachUnitGroup()
    {
        unitGroup = unitController.MindGroupList.GetUnitGroupFromUnitId(GroupId);
    }

    private void CheckSpawnable()
    {
        if (RespawnTimer <= 0 || singleFree)
        {
            if (singleFree || GameResources.EnoughResources(RespawnCost, baseController.GetGameResources()))
            {
                SpawnUnit();
            }
        }
        else
        {
            RespawnTimer--;
        }
    }

    private void SpawnUnit()
    {
        if (UnitResource != null)
        {
            if (unitGroup.AddUnit())
            {
                RespawnTimer = DefaultRespawnTime;
                if (!singleFree)
                {
                    baseController.GetGameResources().SubtractResources(RespawnCost);
                }
                else
                {
                    singleFree = false;
                }

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
                ant.GetComponent<Ant>().TeamID = TeamId;

                ant.GetComponent<NavMeshAgent>().Warp(transform.position);
                ant.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
        else
        {
            throw new Exception($"The {GetType()} has no UnitResource set!");
        }
    }

    private void SplitRoom()
    {
        List<BaseTile> sameNeighborList = new List<BaseTile>();

        foreach (var neighbor in Parent.Neighbors)
        {
            if (neighbor.RoomScript != null && neighbor.RoomScript.IsRoom() && neighbor.RoomScript.GetType() == GetType())
            {
                sameNeighborList.Add(neighbor);
            }
        }

        if (sameNeighborList.Count > 1 && sameNeighborList.Count < 5)
        {
            List<BaseTile> checkList = new List<BaseTile>(sameNeighborList);
            List<BaseTile> roomList = new List<BaseTile>();

            while (checkList.Count > 1)
            {
                roomList.Add(checkList[0]);

                List<BaseTile> swapList = new List<BaseTile>();
                for (int i = 1; i < checkList.Count; i++)
                {
                    if (!Astar.CanFindOwnNeighbor(checkList[0], checkList[i], new List<BaseTile>() { Parent }))
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
                unitController.SplitUnitGroups(Parent, roomList);
            }
        }
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
        CancelInvoke("CheckSpawnable");
    }

    public override void Destroy()
    {
        unitGroup?.RemoveMax();
        SplitRoom();
        Destroy(gameObject);
    }
}