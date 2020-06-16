using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class BaseTileData
    {
        public RoomType RoomType;
        public float RotationY;
        public string Name;
        public string GroupID;
        public int RespawnTime;

        public BaseTileData(BaseRoom room, GameObject currTile, string name, Guid groupID, int respawnTime)
        {
            RoomType = room != null ? room.GetRoomType() : RoomType.None;
            RotationY = currTile != null ? currTile.transform.localEulerAngles.y : 0;
            Name = name;
            if (groupID != Guid.Empty)
            {
                GroupID = groupID.ToString();
            }
            RespawnTime = respawnTime;
        }
    }
}