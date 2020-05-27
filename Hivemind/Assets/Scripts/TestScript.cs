using System;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class TestScript : MonoBehaviour
{
    private UnitController controller;

    private Guid Guid;

    [SerializeField] private Transform SpawnPosition;

    [SerializeField] private GameObject WorkerPrefab;

    // Start is called before the first frame update
    private void Start()
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
        if (Input.GetKeyDown(KeyCode.Space)) controller.CreateUnitGroup();
    }

    public void CreateUnit()
    {
        Guid = controller.CreateUnitGroup();
    }

    public void AddCurrentUnits()
    {
        controller.SetCurrentUnits(Guid, controller.UnitGroupList.GetUnitGroupFromUnitId(Guid).CurrentUnits + 1);
    }

    public void RemoveCurrentUnits()
    {
        controller.SetCurrentUnits(Guid, controller.UnitGroupList.GetUnitGroupFromUnitId(Guid).CurrentUnits - 1);
    }

    public void AddMaxUnits()
    {
        controller.SetMaxUnits(Guid, controller.UnitGroupList.GetUnitGroupFromUnitId(Guid).MaxUnits + 1);
    }

    public void RemoveMaxUnits()
    {
        controller.SetMaxUnits(Guid, controller.UnitGroupList.GetUnitGroupFromUnitId(Guid).MaxUnits - 1);
    }

    public void SpawnUnitForLastGroup()
    {
        if (!Guid.Equals(Guid.Empty))
        {
            var go = Instantiate(WorkerPrefab, SpawnPosition.position, Quaternion.identity,
                GameObject.Find("Ants").transform);
            var a = go.GetComponent<Ant>();

            controller.UnitGroupList.GetUnitGroupFromUnitId(Guid).AddUnits(a);
            AddMaxUnits();
            AddCurrentUnits();
        }
    }
}