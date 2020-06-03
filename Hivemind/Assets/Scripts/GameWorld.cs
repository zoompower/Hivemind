using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GameWorld
{
    public static List<ResourceNode> ResourceList = new List<ResourceNode>();
    public static List<Ant> AntList = new List<Ant>();
    public static UnitController MyUnitController;
    public static List<BaseController> BaseControllerList = new List<BaseController>();
    private static Storage storage = null;

    public static ResourceNode FindNearestUnknownResource(Vector3 antPosition)
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

    public static ResourceNode FindNearestKnownResource(Vector3 antPosition, ResourceType prefType)
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

    public static Storage GetStorage()
    {
        return storage;
    }

    public static void RemoveResource(ResourceNode resource)
    {
        ResourceList.Remove(resource);
    }

    public static void SetStorage(Storage Storage)
    {
        storage = Storage;
    }

    public static void AddNewResource(ResourceNode resource)
    {
        ResourceList.Add(resource);
    }

    public static void AddNewAnt(Ant ant)
    {
        AntList.Add(ant);
    }

    public static void SetUnitController(UnitController unitController)
    {
        MyUnitController = unitController;
    }

    public static void AddBaseController(BaseController baseController)
    {
        BaseControllerList.Add(baseController);
    }

    public static void RemoveAnt(Ant ant)
    {
        AntList.Remove(ant);
    }

    public static BaseController FindBaseController(int ID)
    {
        return BaseControllerList.Find(myteam => myteam.TeamID == ID);
    }

    public static Ant FindAnt(Guid guid)
    {
        return AntList.FirstOrDefault(myAnt => myAnt.myGuid == guid);
    }

    public static ResourceNode FindResourceNode(Guid guid)
    {
        return ResourceList.FirstOrDefault(resource => resource.myGuid == guid);
    }

    public static void Save()
    {
        MyUnitController.UpdateEventText("QuickSaving...");
        SaveObject saveObject = new SaveObject
        {
            ResourceAmountsKeys = GameResources.GetResourceAmounts().Keys.ToList(),
            ResourceAmountsValues = GameResources.GetResourceAmounts().Values.ToList(),
            Resources = ResourceList,
            Ants = AntList,
            MindGroups = MyUnitController.MindGroupList.GetMindGroupList(),
            BaseControllers = BaseControllerList
        };
        string json = saveObject.ToJson();
        File.WriteAllText(Application.dataPath + "/save.txt", json);
        MyUnitController.UpdateEventText("QuickSave Complete!");
    }

    public static void Load()
    {
        MyUnitController.UpdateEventText("QuickLoading...");
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/save.txt");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
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
            MyUnitController.UpdateEventText("QuickLoad Complete!");
        }
        else
        {
            MyUnitController.UpdateEventText("Save file not found!", Color.red);
            Debug.LogError($"Save file could not be found at {Application.dataPath}/save.txt");
        }
    }
}