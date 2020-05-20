using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ant : MonoBehaviour
{
    private NavMeshAgent agent;
    public float baseSpeed;
    public float currentSpeed;
    public int damage;

    public int health;

    private List<IMind> minds;
    public Gathering.State state;
    private Storage storage;
    internal Guid unitGroupID;
    private AudioSource audioSrc;
    public Ant closestEnemy { get; private set; }


    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
        minds = new List<IMind>();
        state = Gathering.State.Idle;
        audioSrc = GetComponent<AudioSource>();
        //get volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
        audioSrc.volume *=  0.05f;
        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
    }

    // Start is called before the first frame update
    private void Start()
    {
        storage = GameWorld.GetStorage();
    }

    public void OnDestroy()
    {
        SettingsScript.OnVolumeChanged -= delegate { UpdateVolume(); };
    }

    // Update is called once per frame
    private void Update()
    {
        if (AtBase())
        {
            var mindGroupMind = FindObjectOfType<UnitController>().UnitGroupList.GetMindGroupFromUnitId(unitGroupID)
                .Minds;

            if (minds.Count < mindGroupMind.Count)
                for (var i = minds.Count; i < mindGroupMind.Count; i++)
                    minds.Add(mindGroupMind[i].Clone());
            for (var i = 0; i < minds.Count; i++)
                if (!minds[i].Equals(mindGroupMind[i]))
                {
                    minds[i].Update(mindGroupMind[i]);
                    if (!minds[i].Equals(mindGroupMind[i])) minds[i] = mindGroupMind[i].Clone();
                }
        }

        double likeliest = 0;
        var mindIndex = 0;
        var currentIndex = 0;
        foreach (var mind in minds)
        {
            var current = mind.Likelihood(this);
            if (current > likeliest)
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
        if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f) return true;
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

    internal void SetStorage(Storage storage)
    {
        this.storage = storage;
    }

    public void UpdateVolume()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
            audioSrc.volume *= 0.05f;
        }
    }

    private enum AntType
    {
        Worker,
        Soldier
    }

    public void PlaySoundDiscovery()
    {
        audioSrc.Play();
    }
}