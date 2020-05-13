﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class SaveObject
    {
        //super ugly way to serialize a dictionary because of Unity's stupid serialization
        public List<ResourceType> ResourceAmountsKeys;
        public List<int> ResourceAmountsValues;

        public List<ResourceNode> UnknownResources;

        public List<ResourceNode> KnownResources;

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
