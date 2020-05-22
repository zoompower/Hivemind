using UnityEngine;

public class InBaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null)
        {
            ant.isAtBase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null)
        {
            ant.isAtBase = false;
        }
    }
}
