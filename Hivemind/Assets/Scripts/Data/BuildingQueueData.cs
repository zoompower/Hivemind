using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BuildingQueueData
    {
        public List<BuildingTaskData> Queue = new List<BuildingTaskData>();
        public List<BuildingTaskData> WaitQueue = new List<BuildingTaskData>();
        public int ControllerID;

        public BuildingQueueData(List<BuildingTask> queue, List<BuildingTask> waitQueue, int controllerID)
        {
            foreach (BuildingTask buildingTask in queue)
            {
                Queue.Add(buildingTask.GetData());
            }
            foreach (BuildingTask buildingTask in waitQueue)
            {
                WaitQueue.Add(buildingTask.GetData());
            }
            ControllerID = controllerID;
        }
    }
}