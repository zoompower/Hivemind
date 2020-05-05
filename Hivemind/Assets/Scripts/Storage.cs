using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class Storage : MonoBehaviour
    {
        private void Awake()
        {
            GameWorld.SetStorage(this);
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }
    }
}
