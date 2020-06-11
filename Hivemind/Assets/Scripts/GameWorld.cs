using Assets.Scripts;
using Assets.Scripts.Data;
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

    public List<ResourceNode> ResourceList = new List<ResourceNode>();
    public List<Ant> AntList = new List<Ant>();
    public UiController UiController;
    public List<UnitController> UnitControllerList = new List<UnitController>();
    public List<BaseController> BaseControllerList = new List<BaseController>();

    public int LocalTeamId = 0;

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

    private void Start()
    {
        UiController = FindObjectOfType<UiController>();
    }

    public ResourceNode FindNearestUnknownResource(Vector3 antPosition, int teamID)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if ((resource.TeamIsKnown & (1 << teamID)) == 0)
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Load();
        }
    }

    public ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType, int teamID)
    {
        ResourceNode closest = null;
        float minDistance = float.MaxValue;
        foreach (ResourceNode resource in ResourceList)
        {
            if ((resource.TeamIsKnown & (1 << teamID)) > 0 && (prefType == ResourceType.Unknown || resource.resourceType == prefType))
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

    public void AddUnitController(UnitController unitController)
    {
        UnitControllerList.Add(unitController);
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
            UiController.UpdateEventText("QuickSaving...");
        }
        else
        {
            UiController.UpdateEventText("Saving...");
        }
        SaveObject saveObject = new SaveObject
        {
            LevelName = SceneManager.GetActiveScene().name,
            Resources = ResourceList,
            Ants = AntList,
            BaseControllers = BaseControllerList
        };

        foreach (var unitController in UnitControllerList)
        {
            saveObject.TeamMindGroups.Add(new TeamMindGroup(unitController.TeamId, unitController.MindGroupList.GetMindGroupList()));
        }

        string json = saveObject.ToJson();
        File.WriteAllText(GetSavePath() + $"/{name}.txt", json);
        if (name == "QuickSave")
        {
            UiController.UpdateEventText("QuickSave Complete!");
        }
        else
        {
            UiController.UpdateEventText("Save Complete!");
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
                UiController.UpdateEventText("Deleting save file failed!", Color.red);
            }
        }
        else
        {
            UiController.UpdateEventText("Save file not found!", Color.red);
        }
    }

    public void Load(string name = "QuickSave")
    {
        Instance.StartCoroutine(LoadEnumerator(name));
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
                UiController.UpdateEventText("QuickLoading...");
            }
            else
            {
                UiController.UpdateEventText("Loading...");
            }
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

            foreach (UnitController controller in UnitControllerList)
            {
                controller.SetData(saveObject.TeamMindGroupData.FirstOrDefault(data => data.TeamId == controller.TeamId).MindGroupDataList);
            }

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
                UiController.UpdateEventText("QuickLoad Complete!");
            }
            else
            {
                UiController.UpdateEventText("Load Complete!");
            }
        }
        else
        {
            UiController.UpdateEventText("Save file not found!", Color.red);
        }
    }
}
