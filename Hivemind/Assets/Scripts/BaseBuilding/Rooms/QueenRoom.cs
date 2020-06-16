using System;
using UnityEngine;

public class QueenRoom : BaseRoom
{
    public int health;

    void Update()
    {
        if(health < 1)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Queen DIED!");
        GameWorld.Instance.QueenDied(transform.parent.GetComponentInParent<BaseController>().TeamID);
    }

    public override RoomType GetRoomType()
    {
        return RoomType.QueenRoom;
    }

    public override bool IsRoom()
    {
        return true;
    }

    public override bool IsDestructable()
    {
        return false;
    }

    private void Start()
    {
        health = 100;
        foreach(var baseTile in GetComponentInParent<BaseTile>().Neighbors)
        {
            baseTile.IsIndestructable = true;
            baseTile.IsUnbuildable = true;
            baseTile.DestroyRoom(true);
        }
        transform.parent.GetComponentInParent<BaseController>().SetQueenRoom(this);
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public BaseTile GetBaseTile()
    {
        return GetComponentInParent<BaseTile>();
    }
}
