using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gathering : MonoBehaviour
{
    private enum State
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingTostorage,
    }
    public int CarryAmount = 3;

    private State state;
    private int inventoryAmount;
    private ResourceNode target;
    private Transform storage;
    private NavMeshAgent agent;
    private float baseSpeed;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
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
                agent.SetDestination(target.GetPosition());
                if (Vector3.Distance(transform.position, target.GetPosition()) < 2f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                target.GrabResource();
                StartCoroutine(target.respawnResource());
                inventoryAmount++;
                agent.speed *= 0.9f;
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
                agent.SetDestination(storage.position);
                if (Vector3.Distance(transform.position, storage.position) < 8f)
                {
                    GameResources.AddResourceAmount(inventoryAmount);
                    inventoryAmount = 0;
                    agent.speed = baseSpeed;
                    state = State.Idle;
                }
                break;
        }
    }
}
