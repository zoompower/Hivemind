using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Rock,
    Crystal,
    Unknown,
}

public class ResourceNode : MonoBehaviour
{
    private bool respawningResources = false;

    public GameObject baseObject;
    public int BaseResourceAmount = 4;
    public ResourceType resourceType = ResourceType.Unknown;
    public bool CanRespawn = false;
    [SerializeField]
    private int TimeToRespawn = 30;
    public bool DestroyWhenEmpty = false;

    [SerializeField]
    private AudioSource audioSrc;

    private int resourceAmount;

    private int futureResourceAmount;

    private void Awake()
    {
        resourceAmount = BaseResourceAmount;
        futureResourceAmount = resourceAmount;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameWorld.AddNewResource(this);
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
    }

    public void AddToKnownResourceList()
    {
        if (!GameWorld.KnownResources.Contains(this) && gameObject != null)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            GameWorld.AddNewKnownResource(this);
        }
    }

    private void Update()
    {
        if (CanRespawn && resourceAmount < BaseResourceAmount && !respawningResources)
        {
            StartCoroutine(respawnResource());
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void OnDestroy()
    {
        GameWorld.RemoveResource(this);
        SettingsScript.OnVolumeChanged -= delegate { UpdateVolume(); };
    }

    private IEnumerator respawnResource()
    {
        respawningResources = true;
        yield return new WaitForSeconds(TimeToRespawn);
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
            Destroy(gameObject);
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

    public int GetResourcesFuture()
    {
        return futureResourceAmount;
    }

    public bool knownResource()
    {
        var returnValue = GameWorld.KnownResources.Contains(this);
        return returnValue;
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