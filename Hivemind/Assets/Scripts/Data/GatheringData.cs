using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Gathering;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GatheringData : MindData
    {
        public string AntGuid;
        public List<string> GatheredResources;
        public List<ResourceType> InventoryKeys;
        public List<int> InventoryValues;
        public bool IsScout;
        public int NextHarvest;
        public bool PreparingReturn;
        public bool Scouting;
        public string TargetGuid;
        public ResourceType PrefferedType;
        public int CarryWeight;
        public Direction PrefferedDirection;
        public float ScoutDestinationX;
        public float ScoutDestinationY;
        public float ScoutDestinationZ;
        public float ScoutSeconds;
        public float ReturnSeconds;

        public GatheringData(Ant ant, List<string> gatheredResources, Dictionary<ResourceType, int> inventory, bool isScout, int nextHarvest, bool preparingReturn, bool scouting, ResourceNode target, ResourceType prefferedType, int carryWeight, Direction prefferedDirection, Vector3 scoutingDestination, float scoutSeconds, float returnSeconds)
        {
            AntGuid = ant.myGuid.ToString();
            GatheredResources = gatheredResources;
            InventoryKeys = inventory.Keys.ToList();
            InventoryValues = inventory.Values.ToList();
            IsScout = isScout;
            NextHarvest = nextHarvest;
            PreparingReturn = preparingReturn;
            Scouting = scouting;
            if (target != null)
            {
                TargetGuid = target.myGuid.ToString();
            }
            PrefferedType = prefferedType;
            CarryWeight = carryWeight;
            PrefferedDirection = prefferedDirection;
            ScoutDestinationX = scoutingDestination.x;
            ScoutDestinationY = scoutingDestination.y;
            ScoutDestinationZ = scoutingDestination.z;
            ScoutSeconds = scoutSeconds;
            ReturnSeconds = returnSeconds;
        }
    }
}
