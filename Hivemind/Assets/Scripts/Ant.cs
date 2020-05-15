using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Ant : MonoBehaviour
{
    enum AntType
    {
        Worker,
        Soldier,
    }

    public int health;
    public int damage;
    public float baseSpeed;
    public float currentSpeed;
    public Gathering.State state;

    private List<IMind> minds;
    private NavMeshAgent agent;
    public Ant closestEnemy { get; private set; }
    private Storage storage;
    internal Guid unitGroupID;


    void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
        minds = new List<IMind>();
        state = Gathering.State.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        storage = GameWorld.GetStorage();
    }

    // Update is called once per frame
    void Update()
    {
        if (AtBase())
        {
            List<IMind> mindGroupMind = FindObjectOfType<UnitController>().UnitGroupList.GetMindGroupFromUnitId(unitGroupID).minds;
          
            if(minds.Count < mindGroupMind.Count)
            {
                for(int i = minds.Count; i < mindGroupMind.Count; i++)
                {
                    minds.Add(mindGroupMind[i].Clone());
                }
            }
            for(int i = 0; i < minds.Count; i++)
            {
                if (!minds[i].Equals(mindGroupMind[i]))
                {
                    minds[i].Update(mindGroupMind[i]);
                    if (!minds[i].Equals(mindGroupMind[i]))
                    {
                        minds[i] = mindGroupMind[i].Clone();
                    }
                }
            }
        }

        double likeliest= 0;
        int mindIndex = 0;
        int currentIndex = 0;
        foreach(IMind mind in minds)
        {
           double current =  mind.Likelihood(this);
            if(current > likeliest)
            {
                mindIndex = currentIndex;
                likeliest = current;
            }
            currentIndex++;
        }

        minds[mindIndex].Execute(this);
    }

    public bool AtBase()
    {
        if (Vector3.Distance(transform.position, storage.GetPosition()) < 1f)
        {
            return true;
        }
        return false;
    }

    public bool InCombat()
    {
        return false;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public Storage GetStorage()
    {
        return storage;
    }

    public void SetUnitGroup(Guid ug)
    {
        unitGroupID = ug;
    }

    internal void UpdateSpeed()
    {
        agent.speed = currentSpeed;
    }
}

