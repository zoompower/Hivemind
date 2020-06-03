using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public BaseTileData(BaseRoom room, GameObject currTile, string name, Guid groupID)
        {
            RoomType = room != null ? room.GetRoomType() : RoomType.None;
            RotationY = currTile != null ? currTile.transform.localEulerAngles.y : 0;
            Name = name;
            if (groupID != Guid.Empty)
            {
                GroupID = groupID.ToString();
            }
        }
    }
}
