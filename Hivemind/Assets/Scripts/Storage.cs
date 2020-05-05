using UnityEngine;

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
