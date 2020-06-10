using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BaseControllerData
    {
        public int TeamID;
        public BaseBuildingTool CurrentTool;
        public float TeleporterExitX;
        public float TeleporterExitY;
        public float TeleporterExitZ;
        public float TeleporterEntranceX;
        public float TeleporterEntranceY;
        public float TeleporterEntranceZ;
        public BuildingQueueData queueData;
        public List<BaseTileData> BaseTileData = new List<BaseTileData>();
        public ResourceDictionaryData GameResources;

        public BaseControllerData(int teamID, BaseBuildingTool currentTool, Vector3 teleporterExit, Vector3 teleporterEntrance, BuildingQueue buildingQueue, Transform transform, GameResources gameResources)
        {
            TeamID = teamID;
            CurrentTool = currentTool;
            TeleporterExitX = teleporterExit.x;
            TeleporterExitY = teleporterExit.y;
            TeleporterExitZ = teleporterExit.z;
            TeleporterEntranceX = teleporterEntrance.x;
            TeleporterEntranceY = teleporterEntrance.y;
            TeleporterEntranceZ = teleporterEntrance.z;
            queueData = buildingQueue.GetData();
            foreach (Transform myTransform in transform)
            {
                if (myTransform.GetComponent<BaseTile>() != null)
                {
                    BaseTileData.Add(myTransform.GetComponent<BaseTile>().GetData());
                }
            }
            GameResources = gameResources.GetData();
        }
    }
}