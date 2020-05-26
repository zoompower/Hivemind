using UnityEngine;

public class InBaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null && GetComponentInParent<BaseController>().TeamID == ant.TeamID)
        {
            ant.isAtBase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null && GetComponentInParent<BaseController>().TeamID == ant.TeamID)
        {
            ant.isAtBase = false;
        }
    }
}
