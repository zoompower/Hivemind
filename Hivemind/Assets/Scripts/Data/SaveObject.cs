using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class SaveObject
    {
        public string LevelName;

        public List<ResourceNode> Resources;
        public List<ResourceNodeData> ResourceData = new List<ResourceNodeData>();
        public List<Ant> Ants;
        public List<AntData> AntData = new List<AntData>();
        public List<TeamMindGroup> TeamMindGroups = new List<TeamMindGroup>();
        public List<TeamMindGroupData> TeamMindGroupData = new List<TeamMindGroupData>();
        public List<BaseController> BaseControllers = new List<BaseController>();
        public List<BaseControllerData> BaseControllerData = new List<BaseControllerData>();
        public BasicAIData BasicAIData;

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
            foreach (TeamMindGroup teamMindGroup in TeamMindGroups)
            {
                TeamMindGroupData.Add(teamMindGroup.GetData());
            }
            foreach (BaseController baseController in BaseControllers)
            {
                BaseControllerData.Add(baseController.GetData());
            }
            return JsonUtility.ToJson(this);
        }
    }
}