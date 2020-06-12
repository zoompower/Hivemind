using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ant : MonoBehaviour
{
    public Guid myGuid = Guid.NewGuid();
    public int SpatialPositionId;
    private NavMeshAgent agent;
    public float baseSpeed;
    public float currentSpeed;
    public int damage;

    public int health;
    public bool alive;
    public string Prefab;

    private List<IMind> minds = new List<IMind>();
    private BaseController baseController;
    internal Guid unitGroupID;
    private AudioSource audioSrc;

    private UnitController unitController;

    public Ant closestEnemy { get; private set; }

    internal bool isAtBase = true;

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
            if (child.gameObject.layer == LayerMask.NameToLayer("Minimap"))
            {
                miniMapRenderer = child;
            }
        }
        GameWorld.Instance.AddAnt(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        var baseControllers = FindObjectsOfType<BaseController>();

        foreach (var controller in baseControllers)
        {
            if (controller.TeamID == TeamID)
            {
                baseController = controller;
                break;
            }
        }

        var unitControllers = FindObjectsOfType<UnitController>();

        foreach (var controller in unitControllers)
        {
            if (controller.TeamId == TeamID)
            {
                unitController = controller;
                break;
            }
        }

        miniMapRenderer.GetComponent<SpriteRenderer>().color = GetBaseController().TeamColor;

        alive = true;
        AddEventListeners();
    }

    public void OnDestroy()
    {
        unitController.OnUnitDestroy(unitGroupID);
        GameWorld.Instance.RemoveAnt(this);
        RemoveEventListeners();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckAlive();
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
        if (agent.isOnNavMesh)
        {
            minds[mindIndex].Execute();
        }
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
        SpatialPartition.GetSpatialFromGrid(SpatialPositionId).Remove(gameObject);
        Destroy(gameObject);
    }

    public bool CheckAlive()
    {
        if (health < 1)
        {
            alive = false;
            Die();
        }
        return alive;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public BaseController GetBaseController()
    {
        return baseController;
    }

    internal void UpdateSpeed()
    {
        agent.speed = currentSpeed;
    }

    public void SetunitGroupID(Guid id)
    {
        unitGroupID = id;
    }

    public void SetAtBase(bool atBase)
    {
        isAtBase = atBase;
    }

    private void UpdateMind()
    {
        if (AtBase())
        {
            if (TeamID != 0)
            {
                minds.Clear();
                minds = new List<IMind>() { new Gathering(ResourceType.Crystal, 1, Gathering.Direction.East, true), new CombatMind() };

                foreach (var mind in minds)
                {
                    mind.Initiate(this);
                }
                return;
            }

            var mindGroupMind = unitController.MindGroupList.GetMindGroupFromUnitId(unitGroupID)?.Minds;
            if (mindGroupMind != null)
            {
                minds.Clear();
                for (var i = 0; i < mindGroupMind.Count; i++)
                {
                    minds.Add(mindGroupMind[i].Clone());
                    minds[i].Initiate(this);
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

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public AntData GetData()
    {
        return new AntData(myGuid, baseSpeed, currentSpeed, damage, health, minds, unitGroupID, closestEnemy, isAtBase, Prefab, TeamID, gameObject.transform.position, gameObject.transform.localEulerAngles, gameObject.transform.localScale);
    }

    public void SetData(AntData data)
    {
        Debug.ClearDeveloperConsole();
        gameObject.SetActive(false);
        myGuid = Guid.Parse(data.MyGuid);
        gameObject.transform.parent = GameObject.Find("Ants").transform;
        baseSpeed = data.BaseSpeed;
        currentSpeed = data.CurrentSpeed;
        damage = data.Damage;
        health = data.Health;
        Prefab = data.Prefab;
        minds = data.Minds;
        unitGroupID = Guid.Parse(data.UnitGroupID);
        if (data.ClosestEnemy != string.Empty)
        {
            closestEnemy = GameWorld.Instance.FindAnt(Guid.Parse(data.ClosestEnemy));
        }
        isAtBase = data.IsAtBase;
        TeamID = data.TeamID;
        gameObject.SetActive(true);
        gameObject.transform.localEulerAngles = new Vector3(data.RotationX, data.RotationY, data.RotationZ);
        transform.localScale = new Vector3(data.ScaleX, data.ScaleY, data.ScaleZ);
        GetComponent<NavMeshAgent>().enabled = true;
        for (int i = 0; i < minds.Count; i++)
        {
            minds[i].SetData(data.MindData[i]);
        }
    }

    public void PlaySoundDiscovery()
    {
        audioSrc.Play();
    }

    private void AddEventListeners()
    {
        if (unitController)
            unitController.OnGroupIdChange += ChangeGroupID;

        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
    }

    private void RemoveEventListeners()
    {
        if (unitController != null)
        {
            unitController.OnGroupIdChange -= ChangeGroupID;
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

    public List<IMind> GetMinds()
    {
        return minds;
    }

    public void SetMinds(List<IMind> minds)
    {
        this.minds = minds;
    }

    public void ChangeScale(float scaleAnt, float scaleMinimapRenderer)
    {
        transform.localScale = new Vector3(scaleAnt, scaleAnt, scaleAnt);
        miniMapRenderer.localScale = new Vector3(scaleMinimapRenderer, scaleMinimapRenderer, scaleMinimapRenderer);
    }
}
