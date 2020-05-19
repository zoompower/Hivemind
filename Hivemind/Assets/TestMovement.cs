using UnityEngine;
using UnityEngine.AI;

public class TestMovement : MonoBehaviour
{
    GameObject target;

    private NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        target = Instantiate(Resources.Load("TestTarget") as GameObject);
        target.transform.position = transform.position;
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.transform.position);
    }
}
