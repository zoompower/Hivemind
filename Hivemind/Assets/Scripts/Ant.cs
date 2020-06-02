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

    private List<IMind> minds = new List<IMind>();
    private Storage storage;
    internal Guid unitGroupID;
    private AudioSource audioSrc;

    public Ant closestEnemy { get; private set; }

    public bool isAtBase = true;

    public int TeamID;
    private Transform miniMapRenderer;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        currentSpeed = baseSpeed;
        audioSrc = GetComponent<AudioSource>();
        //get volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
        audioSrc.volume *= 0.15f;
        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };

        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == UnityEngine.LayerMask.NameToLayer("Minimap"))
            {
                miniMapRenderer = child;
            }
        }
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
            var mindGroupMind = FindObjectOfType<UnitController>().MindGroupList.GetMindGroupFromUnitId(unitGroupID).Minds;

            minds.Clear();
            for (var i = 0; i < mindGroupMind.Count; i++)
            {
                minds.Add(mindGroupMind[i].Clone());
                minds[i].Initiate(this);
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

    public void ChangeScale(float scaleAnt, float scaleMinimapRenderer)
    {
        transform.localScale = new Vector3(scaleAnt, scaleAnt, scaleAnt);
        miniMapRenderer.localScale = new Vector3(scaleMinimapRenderer, scaleMinimapRenderer, scaleMinimapRenderer);
    }
}