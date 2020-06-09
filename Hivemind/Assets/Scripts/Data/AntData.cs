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
    public List<MindData> MindData = new List<MindData>();

    public string UnitGroupID;
    public string ClosestEnemy;
    public bool IsAtBase;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float RotationX;
    public float RotationY;
    public float RotationZ;
    public string Prefab;
    public int TeamID;

    public AntData(Guid myGuid, float baseSpeed, float currentSpeed, int damage, int health, List<IMind> minds, Guid unitGroupID, Ant closestEnemy, bool isAtBase, string prefab, int teamID, Vector3 position, Vector3 rotation)
    {
        MyGuid = myGuid.ToString();
        BaseSpeed = baseSpeed;
        CurrentSpeed = currentSpeed;
        Damage = damage;
        Health = health;
        Minds = minds;
        foreach (IMind mind in minds)
        {
            MindData.Add(mind.GetData());
        }
        UnitGroupID = unitGroupID.ToString();
        if (closestEnemy != null)
        {
            ClosestEnemy = closestEnemy.ToString();
        }
        IsAtBase = isAtBase;
        PositionX = position.x;
        PositionY = position.y;
        PositionZ = position.z;
        RotationX = (int)rotation.x;
        RotationY = rotation.y;
        RotationZ = (int)rotation.z;
        Prefab = prefab;
        TeamID = teamID;
    }
}