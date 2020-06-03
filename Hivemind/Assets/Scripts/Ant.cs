using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ant : MonoBehaviour
{
    public SpatialPartitioning SpatialPosition;
    private NavMeshAgent agent;
    public float baseSpeed;
    public float currentSpeed;
    public int damage;

    public int health;
    public bool alive;

    private List<IMind> minds;
    private Storage storage;
    internal Guid unitGroupID;
    private AudioSource audioSrc;

    public Ant closestEnemy { get; private set; }

    public bool isAtBase = true;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
        minds = new List<IMind>();
        audioSrc = GetComponent<AudioSource>();
        //get volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
        audioSrc.volume *= 0.15f;
        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
    }

    // Start is called before the first frame update
    private void Start()
    {
        alive = true;
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

        if (!IsBusy())
            UpdateMind();

        if (minds.Count < 1) return;
        double likeliest = 0;
        var mindIndex = 0;
        var currentIndex = 0;
        foreach (var mind in minds)
        {
            var current = mind.Likelihood();
            if (current > likeliest)
            {
                mindIndex = currentIndex;
                likeliest = current;
            }

            currentIndex++;
        }
        minds[mindIndex].Execute();
    }

    private bool IsBusy()
    {
        foreach (var mind in minds)
        {
            if (mind.IsBusy())
                return true;
        }
        return false;
    }

    public bool AtBase()
    {
        return isAtBase;
    }

    public bool InCombat()
    {
        return false;
    }

    public void SetClosestEnemy(Ant c)
    {
        closestEnemy = c;
    }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public bool CheckAlive()
    {
        if (health < 1)
        {
            alive = false;
        }
        return alive;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
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

    private void UpdateMind()
    {
        if (AtBase())
        {
            var mindGroupMind = FindObjectOfType<UnitController>().MindGroupList.GetMindGroupFromUnitId(unitGroupID)
                .Minds;

            if (minds.Count < mindGroupMind.Count)
            {
                for (var i = minds.Count; i < mindGroupMind.Count; i++)
                {
                    minds.Add(mindGroupMind[i].Clone());
                    minds[i].Initiate(this);
                }
            }
            for (var i = 0; i < minds.Count; i++)
            {
                if (!minds[i].Equals(mindGroupMind[i]))
                {
                    minds[i].Update(mindGroupMind[i]);
                    if (!minds[i].Equals(mindGroupMind[i]))
                    {
                        minds[i] = mindGroupMind[i].Clone();
                        minds[i].Initiate(this);
                    }
                }
            }
        }
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