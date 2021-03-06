﻿using System.Collections.Generic;
using UnityEngine;

internal class ScalingWorkerRoom : WorkerRoom
{
    public override RoomType GetRoomType()
    {
        return RoomType.ScalingWorkerRoom;
    }

    private void Start()
    {
        InvokeRepeating("CheckAndScale", 1f, 1f);
    }

    private void CheckAndScale()
    {
        if (GameResources.EnoughResources(GetScaledUpgradeCost(), baseController.GetGameResources()))
        {
            baseController.GetGameResources().SubtractResources(GetScaledUpgradeCost());
            unitGroup.AddMax();
        }
    }

    private Dictionary<ResourceType, int> GetScaledUpgradeCost()
    {
        return new Dictionary<ResourceType, int>() { { ResourceType.Rock, Mathf.Clamp(3 + 2 * unitGroup.MaxUnits, 5, 15) } }; ;
    }
}