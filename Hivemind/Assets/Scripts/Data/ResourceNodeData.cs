using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class ResourceNodeData
    {
        public string MyGuid;
        public int TeamIsKnown;
        public bool RespawningResources;
        public int BaseResourceAmount;
        public ResourceType ResourceType;
        public bool CanRespawn;
        public int TimeToRespawn;
        public bool DestroyWhenEmpty;
        public int ResourceAmount;
        public int FutureResourceAmount;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public string Prefab;
        public float RespawnSeconds;
        public bool Enabled;

        public ResourceNodeData(Guid myGuid, int teamIsKnown, bool respawningResources, int baseResourceAmount, ResourceType resourceType, bool canRespawn, int timeToRespawn, bool destroyWhenEmpty, int resourceAmount, int futureResourceAmount, Vector3 position, Vector3 rotation, string prefab, float respawnSeconds, bool enabled)
        {
            MyGuid = myGuid.ToString();
            TeamIsKnown = teamIsKnown;
            RespawningResources = respawningResources;
            BaseResourceAmount = baseResourceAmount;
            ResourceType = resourceType;
            CanRespawn = canRespawn;
            TimeToRespawn = timeToRespawn;
            DestroyWhenEmpty = destroyWhenEmpty;
            ResourceAmount = resourceAmount;
            FutureResourceAmount = futureResourceAmount;
            PositionX = position.x;
            PositionY = position.y;
            PositionZ = position.z;
            RotationX = rotation.x;
            RotationY = rotation.y;
            RotationZ = rotation.z;
            Prefab = prefab;
            RespawnSeconds = respawnSeconds;
            Enabled = enabled;
        }
    }
}
