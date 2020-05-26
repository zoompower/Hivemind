using System;
using System.Collections;
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
    public bool finishedTask;

    public Ant closestEnemy { get; private set; }

    public bool isAtBase = true;

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
        audioSrc.volume *=  0.15f;
        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
        finishedTask = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        storage = GameWorld.GetStorage();
        AddEventListeners();
    }

    public void OnDestroy()
    {
        RemoveEventListeners();
    }

    // Update is called once per frame
    private void Update()
    {
        if (finishedTask)
        {
            StartCoroutine(UpdateMind());
        }

        if (minds.Count < 1) return;
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

        if (minds.Count > 0)
            minds[mindIndex].Execute(this);
    }

    public bool AtBase()
    {
        return isAtBase;
    }

    public bool InCombat()
    {
        return false;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    private IEnumerator UpdateMind()
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
        yield return new WaitForSeconds(2);
    }

    public Storage GetStorage()
    {
        return storage;
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
            audioSrc.volume *= 0.15f;
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

    private void AddEventListeners()
    {
        FindObjectOfType<UnitController>().OnGroupIdChange += ChangeGroupID;

        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
    }

    private void RemoveEventListeners()
    {
        if (FindObjectOfType<UnitController>() != null)
        {
            FindObjectOfType<UnitController>().OnGroupIdChange -= ChangeGroupID;
        }

        SettingsScript.OnVolumeChanged -= delegate { UpdateVolume(); };
    }

    private void ChangeGroupID(object sender, GroupIdChangedEventArgs e)
    {
        if (unitGroupID == e.oldGuid)
        {
            unitGroupID = e.newGuid;
        }
    }
}