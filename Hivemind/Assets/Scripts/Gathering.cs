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
        MovingToStorage,
    }

    private State state;
    private int inventoryAmount;
    public List<Transform> Resources;
    private Transform Target;
    public Transform Storage;

    private void Awake()
    {
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                if (Resources.Count > 0)
                {
                    Target = findNearestResource();
                    state = State.MovingToResource;
                }
                break;
            case State.MovingToResource:
                transform.LookAt(Target);
                transform.position = Vector3.MoveTowards(transform.position, Target.position, 0.05f);
                if (Vector3.Distance(transform.position, Target.position) < 0.001f)
                {
                    state = State.Gathering;
                }
                break;
            case State.Gathering:
                Debug.Log("Gathering...");
                Resources.Remove(Target);
                inventoryAmount++;
                state = State.MovingToStorage;
                Destroy(Target.gameObject);
                break;
            case State.MovingToStorage:
                transform.LookAt(Storage);
                transform.position = Vector3.MoveTowards(transform.position, Storage.position, 0.05f);
                if (Vector3.Distance(transform.position, Storage.position) < 0.001f)
                {
                    GameResources.AddResourceAmount(inventoryAmount);
                    Debug.Log("Depositing Resources...");
                    inventoryAmount = 0;
                    state = State.Idle;
                }
                break;
        }
    }
    private Transform findNearestResource()
    {
        Transform closest = null;
        float minDistance = 100000000000000f;
        foreach(Transform resource in Resources)
        {
            float dist = Vector3.Distance(transform.position, resource.position);
            if (dist < minDistance)
            {
                closest = resource;
                minDistance = dist;
            }
        }
        return closest;
    }
}
