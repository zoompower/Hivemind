using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Unknown,
    Rock,
    Crystal,
    Food
}

public class ResourceNode : MonoBehaviour
{
    public Guid myGuid = Guid.NewGuid();
    private bool respawningResources = false;

    public GameObject baseObject;
    public int BaseResourceAmount = 4;
    public ResourceType resourceType = ResourceType.Unknown;
    public bool CanRespawn = false;
    public bool Enabled = true;

    [SerializeField]
    private int TimeToRespawn = 30;
    public bool DestroyWhenEmpty = false;
    [HideInInspector]
    public int TeamIsKnown;
    public string Prefab;
    private float respawnSeconds;

    private AudioSource audioSrc;

    private int resourceAmount;

    private int futureResourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        futureResourceAmount = resourceAmount;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        TeamIsKnown = 0;
        audioSrc = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        }
        if (resourceType == ResourceType.Crystal)
        {
            audioSrc.volume *= 2.5f;
        }

        SettingsScript.OnVolumeChanged += delegate { UpdateVolume(); };
        GameWorld.Instance.AddResource(this);
    }

    private void Update()
    {
        if (CanRespawn && resourceAmount < BaseResourceAmount && !respawningResources)
        {
            StartCoroutine(respawnResource());
        }
    }

    public void Discover(int teamID)
    {
        if ((TeamIsKnown & (1 << teamID)) == 0)
        {
            TeamIsKnown += 1 << teamID;
            gameObject.GetComponent<MeshRenderer>().enabled = (TeamIsKnown & (1 << GameWorld.Instance.LocalTeamId)) > 0;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDestroy()
    {
        GameWorld.Instance.RemoveResource(this);
    }

    public void Destroy()
    {
        if (this != null)
        {
            DestroyImmediate(gameObject);
        }
        SettingsScript.OnVolumeChanged -= delegate { UpdateVolume(); };
    }

    private IEnumerator respawnResource(float timeToRespawn = -1f)
    {
        respawningResources = true;
        if (timeToRespawn < 0)
        {
            respawnSeconds = TimeToRespawn;
        }
        while (respawnSeconds > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            respawnSeconds -= 0.1f;
        }
        resourceAmount++;
        futureResourceAmount++;
        ColorResource(resourceAmount);
        respawningResources = false;
    }

    public int DecreaseFutureResources(int amount)
    {
        futureResourceAmount -= amount;
        if (futureResourceAmount > -1)
        {
            return amount;
        }
        else
        {
            int newAmount = amount + futureResourceAmount;
            futureResourceAmount = 0;
            return newAmount;
        }
    }

    internal void IncreaseResourceAmount(int nextHarvest)
    {
        futureResourceAmount += nextHarvest;
    }

    public void GrabResource()
    {
        resourceAmount--;
        if (audioSrc != null)
        {
            audioSrc.Play();
            audioSrc.SetScheduledEndTime(AudioSettings.dspTime + (1));
        }
        if (resourceAmount == 0 && DestroyWhenEmpty)
        {
            Enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
        if (resourceType == ResourceType.Rock)
        {
            ColorResource(resourceAmount);
        }
    }

    public void ColorResource(int amount)
    {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        float amountLeft = (float)amount / (float)BaseResourceAmount;
        mesh.material.SetColor("_Color", new Color(amountLeft, amountLeft, amountLeft));
    }

    public bool HasResources()
    {
        return resourceAmount > 0;
    }

    public int GetResources()
    {
        return resourceAmount;
    }

    public int GetResourcesFuture()
    {
        return futureResourceAmount;
    }

    public ResourceNodeData GetData()
    {
        ResourceNodeData data = new ResourceNodeData(myGuid, TeamIsKnown, respawningResources, BaseResourceAmount, resourceType, CanRespawn, TimeToRespawn, DestroyWhenEmpty, resourceAmount, futureResourceAmount, gameObject.transform.position, gameObject.transform.localEulerAngles, Prefab, respawnSeconds, Enabled);
        return data;
    }

    public void SetData(ResourceNodeData data)
    {
        gameObject.SetActive(false);
        myGuid = Guid.Parse(data.MyGuid);
        gameObject.transform.parent = GameObject.Find("Resources").transform.Find(resourceType.ToString() + "s").transform;
        respawningResources = data.RespawningResources;
        BaseResourceAmount = data.BaseResourceAmount;
        resourceType = data.ResourceType;
        CanRespawn = data.CanRespawn;
        TimeToRespawn = data.TimeToRespawn;
        DestroyWhenEmpty = data.DestroyWhenEmpty;
        resourceAmount = data.ResourceAmount;
        futureResourceAmount = data.FutureResourceAmount;
        ColorResource(resourceAmount);
        TeamIsKnown = data.TeamIsKnown;
        respawnSeconds = data.RespawnSeconds;
        gameObject.GetComponent<MeshRenderer>().enabled = (TeamIsKnown & (1 << GameWorld.Instance.LocalTeamId)) > 0;

        Enabled = data.Enabled;
        if (!Enabled)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        gameObject.SetActive(true);
        if (respawningResources)
        {
            StartCoroutine(respawnResource(respawnSeconds));
        }
        gameObject.transform.localEulerAngles = new Vector3(data.RotationX, data.RotationY, data.RotationZ);
    }

    public void UpdateVolume()
    {
        if (audioSrc != null)
        {
            if (PlayerPrefs.HasKey("Volume"))
            {
                audioSrc.volume = PlayerPrefs.GetFloat("Volume");
            }
            if (resourceType == ResourceType.Crystal)
            {
                audioSrc.volume *= 1.5f;
            }
        }
    }
}
