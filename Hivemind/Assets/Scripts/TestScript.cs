using System;
using UnityEngine;

/**
 * Authors:
 * René Duivenvoorden
 */
public class TestScript : MonoBehaviour
{
    private UnitController controller;

    private Guid guid;

    [SerializeField] private Transform SpawnPosition;

    [SerializeField] private GameObject WorkerPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        controller = FindObjectOfType<UnitController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) controller.CreateUnitGroup();
    }

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
            var go = Instantiate(WorkerPrefab, SpawnPosition.position, Quaternion.identity,
                GameObject.Find("Ants").transform);
            var a = go.GetComponent<Ant>();

            controller.UnitGroupList.GetUnitGroupFromUnitId(guid).AddUnits(a);
            AddMaxUnits();
            AddCurrentUnits();
        }
    }
}