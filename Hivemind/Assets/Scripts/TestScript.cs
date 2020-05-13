﻿using System;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class TestScript : MonoBehaviour
{
    [SerializeField]
    private GameObject WorkerPrefab;

    [SerializeField]
    private Transform SpawnPosition;

    private UnitController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<UnitController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.CreateUnitGroup();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameWorld.Save();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            GameWorld.Load();
        }
    }

    private Guid guid;

    public void CreateUnit()
    {
        guid = controller.CreateUnitGroup();
    }

    public void AddCurrentUnits()
    {
        controller.SetCurrentUnits(guid, controller.UnitGroupList.GetUnitGroupFromUnitId(guid).CurrentUnits + 1);
    }

    public void RemoveCurrentUnits()
    {
        controller.SetCurrentUnits(guid, controller.UnitGroupList.GetUnitGroupFromUnitId(guid).CurrentUnits - 1);
    }

    public void AddMaxUnits()
    {
        controller.SetMaxUnits(guid, controller.UnitGroupList.GetUnitGroupFromUnitId(guid).MaxUnits + 1);
    }

    public void RemoveMaxUnits()
    {
        controller.SetMaxUnits(guid, controller.UnitGroupList.GetUnitGroupFromUnitId(guid).MaxUnits - 1);
    }

    public void SpawnUnitForLastGroup()
    {
        if (!guid.Equals(Guid.Empty))
        {
            Instantiate(WorkerPrefab, SpawnPosition.position, Quaternion.identity, GameObject.Find("Ants").transform);
            AddMaxUnits();
            AddCurrentUnits();
        }
    }
}
