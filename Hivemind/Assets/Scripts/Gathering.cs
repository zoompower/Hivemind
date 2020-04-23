using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using static ResourceNode;

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
    private List<ResourceType> resources;
    private ResourceNode target;
    private Storage storage;
    private NavMeshAgent agent;
    private float baseSpeed;

    private void Awake()
    {
        resources = new List<ResourceType>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
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
                if (Vector3.Distance(transform.position, target.GetPosition()) < 1f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                resources.Add(target.resourceType);
                target.GrabResource();
                agent.speed *= 0.9f;
                if (resources.Count >= CarryAmount)
                {
                    state = State.MovingTostorage;
                }
                else
                {
                    state = State.MovingToResource;
                }
                break;
            case State.MovingTostorage:
                if (storage != null)
                {
                    agent.SetDestination(storage.GetPosition());
                    if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f)
                    {
                        GameResources.AddResources(resources);
                        resources.Clear();
                        agent.speed = baseSpeed;
                        state = State.Idle;
                    }
                }
                else
                {
                    storage = GameWorld.GetStorage();
                }
                break;
        }
    }
}
