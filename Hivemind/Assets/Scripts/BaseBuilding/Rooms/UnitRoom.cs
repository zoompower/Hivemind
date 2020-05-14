using System;
using UnityEngine;

public abstract class UnitRoom : BaseRoom
{
    internal UnitGroup unitGroup;
    
    internal Guid GroupId = Guid.Empty;

    internal bool Destructable = true;

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
            Debug.Log($"{transform.parent.name}, {e.oldGuid}, {e.newGuid}");
            GroupId = e.newGuid;
            unitGroup = unitController.UnitGroupList.GetUnitGroupFromUnitId(GroupId);
        }
    }

    private void Start()
    {
        Destructable = true;

        unitController = FindObjectOfType<UnitController>();
        if (unitController == null)
        {
            throw new Exception("There is no unit controller present, please fix this issue.");
        }

        AddEventListeners();

        bool createNew = true;

        foreach (var neighbor in transform.GetComponentInParent<BaseTile>().Neighbors)
        {
            if (neighbor.GetComponent<BaseTile>().RoomScript is UnitRoom)
            {
                var other = neighbor.GetComponent<BaseTile>().RoomScript as UnitRoom;
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
            Debug.Log($"{transform.parent.name} is spawning a unit"); // TODO: actually spawn unit in room. // warning: The "Ants" container was not found please add a container to save the Hierarchy of clutter.
        }
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }
}