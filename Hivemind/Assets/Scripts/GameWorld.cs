using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance;

    public static List<ResourceNode> ResourceList = new List<ResourceNode>();
    public List<Ant> AntList = new List<Ant>();
    public UnitController MyUnitController;
    public List<BaseController> BaseControllerList = new List<BaseController>();
    private Storage storage = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public ResourceNode FindNearestUnknownResource(Vector3 antPosition)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if (!resource.IsKnown)
            {
                float dist = Vector3.Distance(antPosition, resource.GetPosition());
                if (dist < minDistance)
                {
                    closest = resource;
                    minDistance = dist;
                }
            }
        }
        return closest;
    }

    public ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if (resource.IsKnown && (prefType == ResourceType.Unknown || resource.resourceType == prefType))
            {
                if (resource.GetResourcesFuture() > 0)
                {
                    float dist = Vector3.Distance(antPosition, resource.GetPosition());
                    if (dist < minDistance)
                    {
                        closest = resource;
                        minDistance = dist;
                    }
                }
            }
        }
        return closest;
    }

    public Storage GetStorage()
    {
        return storage;
    }

    public void SetStorage(Storage Storage)
    {
        storage = Storage;
    }

    public void AddResource(ResourceNode resource)
    {
        ResourceList.Add(resource);
    }

    public void RemoveResource(ResourceNode resource)
    {
        ResourceList.Remove(resource);
    }

    public void AddAnt(Ant ant)
    {
        AntList.Add(ant);
    }

    public void RemoveAnt(Ant ant)
    {
        AntList.Remove(ant);
    }

    public void SetUnitController(UnitController unitController)
    {
        MyUnitController = unitController;
    }

    public void AddBaseController(BaseController baseController)
    {
        BaseControllerList.Add(baseController);
    }

    public void RemoveBaseController(BaseController baseController)
    {
        BaseControllerList.Remove(baseController);
    }

    public BaseController FindBaseController(int ID)
    {
        return BaseControllerList.Find(myteam => myteam.TeamID == ID);
    }

    public Ant FindAnt(Guid guid)
    {
        return AntList.FirstOrDefault(myAnt => myAnt.myGuid == guid);
    }

    public ResourceNode FindResourceNode(Guid guid)
    {
        return ResourceList.FirstOrDefault(resource => resource.myGuid == guid);
    }

    public void Save(string name = "QuickSave")
    {
        if (name == "QuickSave")
        {
            MyUnitController.UpdateEventText("QuickSaving...");
        }
        else
        {
            MyUnitController.UpdateEventText("Saving...");
        }
        SaveObject saveObject = new SaveObject
        {
            LevelName = SceneManager.GetActiveScene().name,
            ResourceAmountsKeys = GameResources.GetResourceAmounts().Keys.ToList(),
            ResourceAmountsValues = GameResources.GetResourceAmounts().Values.ToList(),
            Resources = ResourceList,
            Ants = AntList,
            MindGroups = MyUnitController.MindGroupList.GetMindGroupList(),
            BaseControllers = BaseControllerList
        };
        string json = saveObject.ToJson();
        File.WriteAllText(GetSavePath() + $"/{name}.txt", json);
        if (name == "QuickSave")
        {
            MyUnitController.UpdateEventText("QuickSave Complete!");
        }
        else
        {
            MyUnitController.UpdateEventText("Save Complete!");
        }
    }

    public string GetSavePath()
    {
        string path = Application.dataPath + "/Saves";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public void DeleteFile(string name)
    {
        if (File.Exists(GetSavePath() + $"/{name}.txt"))
        {
            try
            {
                File.Delete(Application.dataPath + $"/Saves/{name}.txt");
            }
            catch
            {
                MyUnitController.UpdateEventText("Deleting save file failed!", Color.red);
            }
        }
        else
        {
            MyUnitController.UpdateEventText("Save file not found!", Color.red);
        }
    }

    public void Load(string name = "QuickSave")
    {
        Instance.StartCoroutine(LoadEnumerator(name));
    }

    public int AmountOfKnownResources()
    {
        int amount = 0;
        foreach (ResourceNode resource in ResourceList)
        {
            if (resource.IsKnown)
            {
                amount++;
            }
        }
        return amount;
    }

    private IEnumerator LoadEnumerator(string name)
    {
        if (File.Exists(GetSavePath() + $"/{name}.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + $"/Saves/{name}.txt");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            if (SceneManager.GetActiveScene().name != saveObject.LevelName)
            {
                AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(saveObject.LevelName, LoadSceneMode.Single);
                while (!asyncLoadLevel.isDone)
                {
                    Debug.Log("Loading the Scene");
                    yield return null;
                }
            }
            if (name == "QuickSave")
            {
                MyUnitController.UpdateEventText("QuickLoading...");
            }
            else
            {
                MyUnitController.UpdateEventText("Loading...");
            }
            GameResources.SetResourceAmounts(saveObject.ResourceAmountsKeys, saveObject.ResourceAmountsValues);
            for (int i = 0; i < ResourceList.Count;)
            {
                ResourceList[i].Destroy();
            }
            for (int i = 0; i < saveObject.Resources.Count; i++)
            {
                ResourceNodeData data = saveObject.ResourceData[i];
                GameObject newNode = (GameObject)GameObject.Instantiate(Resources.Load($"Prefabs/Resources/{data.Prefab}"), new Vector3(data.PositionX, data.PositionY, data.PositionZ), Quaternion.identity);
                newNode.GetComponent<ResourceNode>().SetData(data);
            }
            MyUnitController.SetData(saveObject.MindGroupData);
            for (int i = 0; i < AntList.Count;)
            {
                AntList[i].Destroy();
            }
            for (int i = 0; i < saveObject.Ants.Count; i++)
            {
                AntData data = saveObject.AntData[i];
                GameObject newAnt = (GameObject)GameObject.Instantiate(Resources.Load($"Prefabs/{data.Prefab}"), new Vector3(data.PositionX, data.PositionY, data.PositionZ), Quaternion.identity);
                newAnt.GetComponent<Ant>().SetData(data);
            }
            for (int i = 0; i < BaseControllerList.Count; i++)
            {
                BaseControllerList[i].SetData(saveObject.BaseControllerData[i]);
            }
            if (name == "QuickSave")
            {
                MyUnitController.UpdateEventText("QuickLoad Complete!");
            }
            else
            {
                MyUnitController.UpdateEventText("Load Complete!");
            }
        }
        else
        {
            MyUnitController.UpdateEventText("Save file not found!", Color.red);
        }
    }
}