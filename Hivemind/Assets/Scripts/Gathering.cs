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

    private State state;
    private int inventoryAmount;
    private Transform target;
    private Transform storage;

    private void Awake()
    {
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
                if(target == null)
                {
                    target = GameWorld.FindNearestResource(transform.position);
                    if (target == null)
                    {
                        state = State.MovingTostorage;
                    }
                    break;
                }
                transform.LookAt(target);
                transform.position = Vector3.MoveTowards(transform.position, target.position, 0.1f);
                if (Vector3.Distance(transform.position, target.position) < 0.1f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                GameWorld.RemoveResource(target);
                inventoryAmount++;
                state = State.MovingTostorage;
                Destroy(target.gameObject);
                break;
            case State.MovingTostorage:
                transform.LookAt(storage);
                transform.position = Vector3.MoveTowards(transform.position, storage.position, 0.1f);
                if (Vector3.Distance(transform.position, storage.position) < 1f)
                {
                    GameResources.AddResourceAmount(inventoryAmount);
                    inventoryAmount = 0;
                    state = State.Idle;
                }
                break;
        }
    }
}
