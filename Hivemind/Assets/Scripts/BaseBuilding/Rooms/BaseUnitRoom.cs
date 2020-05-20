using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnitRoom : BaseRoom
{
    internal UnitGroup unitGroup;

    internal Guid GroupId = Guid.Empty;

    internal bool Destructable = true;

    internal string UnitResource;

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
            unitGroup = unitController.UnitGroupList.GetUnitGroupFromUnitId(GroupId);
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

        unitGroup = unitController.UnitGroupList.GetUnitGroupFromUnitId(GroupId);

        unitGroup.AddMax();

        InvokeRepeating("CheckSpawnable", 1.0f, 1.0f);
    }

    private void CheckSpawnable()
    {
        if (unitGroup.AddUnit())
        {
            if (UnitResource != null)
            {
                GameObject ant = Instantiate(Resources.Load("TestAnt") as GameObject); // TODO: instead of "testAnt" use unitresource property

                GameObject container = GameObject.Find("Ants");
                if (container == null)
                {
                    throw new Exception("The \"Ants\" container was not found. Please add a container to save the Hiearchy from clutter!");
                }
                else
                {
                    ant.transform.SetParent(container.transform);
                }

                ant.GetComponent<TestMovement>().unitGroup = this.GroupId;

                ant.GetComponent<NavMeshAgent>().Warp(transform.position);
            }
            else
            {
                throw new Exception("This object has no UnitResource set!");
            }
        }
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
        unitGroup.RemoveMax();
    }
}