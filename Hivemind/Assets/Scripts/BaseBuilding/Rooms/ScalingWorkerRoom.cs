using System.Collections.Generic;
using UnityEngine;

class ScalingWorkerRoom : WorkerRoom
{
    private int checkTimer;

    public override RoomType GetRoomType()
    {
        return RoomType.ScalingWorkerRoom;
    }

    private void Update()
    {
        if (checkTimer <= 0)
        {
            checkTimer = 100;

            if (GameResources.EnoughResources(GetScaledUpgradeCost(), baseController.GetGameResources()))
            {
                baseController.GetGameResources().SubtractResources(GetScaledUpgradeCost());
                unitGroup.AddMax();
            }
        }
        else
        {
            checkTimer--;
        }
    }

    private Dictionary<ResourceType, int> GetScaledUpgradeCost()
    {
        return new Dictionary<ResourceType, int>() { { ResourceType.Rock, Mathf.Clamp(3 + 2 * unitGroup.MaxUnits, 5, 15) } }; ;
    }
}
