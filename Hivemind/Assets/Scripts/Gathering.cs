using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gathering : MonoBehaviour
{
    private enum State
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingTostorage,
    }

    public float Speed = 0.05f;
    public int CarryAmount = 3;

    private State state;
    private int inventoryAmount;
    private ResourceNode target;
    private Transform storage;
    private float speed;

    private void Awake()
    {
        speed = Speed;
        GameWorld.CreateNewResource(10);
        storage = GameWorld.GetStorage();
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                target = GameWorld.FindNearestResource(transform.position);
                if (target != null)
                {
                    state = State.MovingToResource;
                }
                break;
            case State.MovingToResource:
                if(!target.HasResources())
                {
                    target = GameWorld.FindNearestResource(transform.position);
                    if (target == null)
                    {
                        state = State.MovingTostorage;
                    }
                    break;
                }
                transform.LookAt(target.GetPosition());
                transform.position = Vector3.MoveTowards(transform.position, target.GetPosition(), speed);
                if (Vector3.Distance(transform.position, target.GetPosition()) < 0.1f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                target.GrabResource();
                inventoryAmount++;
                speed = speed * 0.8f;
                if (speed < 0.02f)
                {
                    speed = 0.02f;
                }
                if (inventoryAmount >= CarryAmount)
                {
                    state = State.MovingTostorage;
                }
                else
                {
                    state = State.MovingToResource;
                }
                break;
            case State.MovingTostorage:
                transform.LookAt(storage);
                transform.position = Vector3.MoveTowards(transform.position, storage.position, speed);
                if (Vector3.Distance(transform.position, storage.position) < 1f)
                {
                    GameResources.AddResourceAmount(inventoryAmount);
                    inventoryAmount = 0;
                    speed = Speed;
                    state = State.Idle;
                }
                break;
        }
    }
}
