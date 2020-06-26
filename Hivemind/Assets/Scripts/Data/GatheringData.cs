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
        public ResourceType ExclusiveType;
        public int CarryWeight;
        public Direction PrefferedDirection;
        public bool Busy;
        public bool LeavingBase;
        public State State;
        public State NextState;
        public float ScoutDestinationX;
        public float ScoutDestinationY;
        public float ScoutDestinationZ;
        public int ScoutDeciSeconds;
        public float ReturnSeconds;
        public bool EnterBase;
        public float TeleporterExitX;
        public float TeleporterExitY;
        public float TeleporterExitZ;

        public GatheringData(Ant ant, List<string> gatheredResources, Dictionary<ResourceType, int> inventory, bool isScout, int nextHarvest, bool preparingReturn, bool scouting, ResourceNode target, ResourceType exclusiveType, int carryWeight, Direction prefferedDirection, bool busy, bool leavingBase, State state, State nextState, Vector3 scoutingDestination, int scoutDeciSeconds, float returnSeconds, bool enterbase, Vector3 teleporterExit)
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
            ExclusiveType = exclusiveType;
            CarryWeight = carryWeight;
            PrefferedDirection = prefferedDirection;
            Busy = busy;
            LeavingBase = leavingBase;
            State = state;
            NextState = nextState;
            ScoutDestinationX = scoutingDestination.x;
            ScoutDestinationY = scoutingDestination.y;
            ScoutDestinationZ = scoutingDestination.z;
            ScoutDeciSeconds = scoutDeciSeconds;
            ReturnSeconds = returnSeconds;
            EnterBase = enterbase;
            TeleporterExitX = teleporterExit.x;
            TeleporterExitY = teleporterExit.y;
            TeleporterExitZ = teleporterExit.z;
        }
    }
}