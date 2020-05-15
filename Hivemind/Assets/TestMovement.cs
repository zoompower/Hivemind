using UnityEngine;
using UnityEngine.AI;

public class TestMovement : MonoBehaviour
{
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = Instantiate(Resources.Load("TestTarget") as GameObject);
        target.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(target.transform.position);
    }
}
