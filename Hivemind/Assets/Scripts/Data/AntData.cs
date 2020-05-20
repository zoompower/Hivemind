using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AntData
{
    public float BaseSpeed;
    public float CurrentSpeed;
    public int Damage;
    public int Health;
    public List<IMind> Minds;
    public Gathering.State State;
    public Storage Storage;
    public string UnitGroupID;
    public Ant ClosestEnemy;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public string Prefab;
    public Transform Parent;

    public AntData(float baseSpeed, float currentSpeed, int damage, int health, List<IMind> minds, Gathering.State state, Storage storage, Guid unitGroupID, Ant closestEnemy, string prefab, Vector3 position, Vector3 rotation, Transform parent)
    {
        BaseSpeed = baseSpeed;
        CurrentSpeed = currentSpeed;
        Damage = damage;
        Health = health;
        Minds = minds;
        State = state;
        Storage = storage;
        UnitGroupID = unitGroupID.ToString();
        ClosestEnemy = closestEnemy;
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