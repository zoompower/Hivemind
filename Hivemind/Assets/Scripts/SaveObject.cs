using System;
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

        public List<ResourceNode> Resources;
        public List<ResourceNodeData> ResourceData = new List<ResourceNodeData>();
        public List<Ant> Ants;
        public List<AntData> AntData = new List<AntData>();

        public string ToJson()
        {
            foreach (ResourceNode node in Resources)
            {
                ResourceData.Add(node.GetData());
            }
            foreach (Ant ant in Ants)
            {
                AntData.Add(ant.GetData());
            }
            return JsonUtility.ToJson(this, true);
        }
    }
}
