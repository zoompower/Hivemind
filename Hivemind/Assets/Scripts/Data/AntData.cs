using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AntData
{
    public string MyGuid;
    public float BaseSpeed;
    public float CurrentSpeed;
    public int Damage;
    public int Health;

    [SerializeReference]
    public List<IMind> Minds;
    [SerializeReference]
    public List<MindData> MindData;

    public Gathering.State State;
    public Storage Storage;
    public string UnitGroupID;
    public string ClosestEnemy;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public string Prefab;
    public Transform Parent;

    public AntData(Guid myGuid, float baseSpeed, float currentSpeed, int damage, int health, List<IMind> minds, Gathering.State state, Storage storage, Guid unitGroupID, Ant closestEnemy, string prefab, Vector3 position, Vector3 rotation, Transform parent)
    {
        MyGuid = myGuid.ToString();
        BaseSpeed = baseSpeed;
        CurrentSpeed = currentSpeed;
        Damage = damage;
        Health = health;
        MindData = new List<MindData>();
        Minds = minds;
        foreach (IMind mind in minds)
        {
            MindData.Add(mind.GetData());
        }
        State = state;
        Storage = storage;
        UnitGroupID = unitGroupID.ToString();
        if(closestEnemy != null)
        {
            ClosestEnemy = closestEnemy.ToString();
        }
        PositionX = position.x;
        PositionY = position.y;
        PositionZ = position.z;
        RotationX = rotation.x;
        RotationY = rotation.y;
        RotationZ = rotation.z;
        Prefab = prefab;
        Parent = parent;
    }
}