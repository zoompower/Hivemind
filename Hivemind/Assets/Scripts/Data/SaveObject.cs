using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class SaveObject
    {
        //super ugly way to serialize a dictionary because of Unity's stupid serialization
        public List<ResourceType> ResourceAmountsKeys;

        public List<int> ResourceAmountsValues;

        public List<ResourceNode> Resources;
        public List<ResourceNodeData> ResourceData = new List<ResourceNodeData>();
        public List<Ant> Ants;
        public List<AntData> AntData = new List<AntData>();
        public List<MindGroup> MindGroups = new List<MindGroup>();
        public List<MindGroupData> MindGroupData = new List<MindGroupData>();
        public List<BaseController> BaseControllers = new List<BaseController>();
        public List<BaseControllerData> BaseControllerData = new List<BaseControllerData>();

        public string ToJson()
        {
            foreach (ResourceNode node in Resources)
            {
                ResourceData.Add(node.GetData());
            }
            foreach (Ant ant in Ants)
            {
                AntData.Add(ant.GetData());
            }
            foreach (MindGroup MindGroup in MindGroups)
            {
                MindGroupData.Add(MindGroup.GetData());
            }
            foreach (BaseController baseController in BaseControllers)
            {
                BaseControllerData.Add(baseController.GetData());
            }
            return JsonUtility.ToJson(this, true);
        }
    }
}