using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    private int[] CollisionLayers;

    private int LayerMask = 0;

    [SerializeField]
    private BaseBuildingTool currentTool = BaseBuildingTool.Wall;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < CollisionLayers.Length; i++)
        {
            LayerMask += (1 << CollisionLayers[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, LayerMask))
            {
                GameObject plate;
                if (hit.transform.childCount > 0 || hit.transform.parent.childCount > 1)
                {
                    plate = hit.transform.gameObject;
                }
                else
                {
                    plate = hit.transform.parent.gameObject;
                }
                plate.GetComponent<BaseTile>().OnLeftClick(currentTool);
            }
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, LayerMask))
            {
                GameObject plate;
                if (hit.transform.childCount > 0 || hit.transform.parent.childCount > 1)
                {
                    plate = hit.transform.gameObject;
                }
                else
                {
                    plate = hit.transform.parent.gameObject;
                }
                plate.GetComponent<BaseTile>().OnRightClick(currentTool);
            }
        }
    }

    public void SetTool(int tool)
    {
        currentTool = (BaseBuildingTool)tool;
    }
}
