using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Busy;
        public bool LeavingBase;
        public State State;
        public State NextState;
        public float ScoutDestinationX;
        public float ScoutDestinationY;
        public float ScoutDestinationZ;
        public float ScoutSeconds;
        public float ReturnSeconds;

        public GatheringData(Ant ant, List<string> gatheredResources, Dictionary<ResourceType, int> inventory, bool isScout, int nextHarvest, bool preparingReturn, bool scouting, ResourceNode target, ResourceType prefferedType, int carryWeight, Direction prefferedDirection, bool busy, bool leavingBase, State state, State nextState, Vector3 scoutingDestination, float scoutSeconds, float returnSeconds)
        {
            if (ant != null)
            {
                AntGuid = ant.myGuid.ToString();
            }
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
            Busy = busy;
            LeavingBase = leavingBase;
            State = state;
            NextState = nextState;
            ScoutDestinationX = scoutingDestination.x;
            ScoutDestinationY = scoutingDestination.y;
            ScoutDestinationZ = scoutingDestination.z;
            ScoutSeconds = scoutSeconds;
            ReturnSeconds = returnSeconds;
        }
    }
}