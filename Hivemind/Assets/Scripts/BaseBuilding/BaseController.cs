using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    private int[] CollisionLayers;

    private int LayerMask = 0;

    [SerializeField]
    private BaseBuildingTool currentTool = BaseBuildingTool.Wall;

    [SerializeField]
    private GameObject WallPrefab;
    [SerializeField]
    private GameObject WorkerRoomPrefab;

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
                OnLeftClick(plate.GetComponent<BaseTile>());
            }
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            SetTool(0);
        }
    }

    private void OnLeftClick(BaseTile tile)
    {
        switch (currentTool)
        {
            case BaseBuildingTool.Destroy:
                tile.DestroyRoom();
                break;
            case BaseBuildingTool.Wall:
                tile.InitializeObject(WallPrefab);
                break;
            case BaseBuildingTool.AntRoom:
                tile.InitializeObject(WorkerRoomPrefab);
                break;

            case BaseBuildingTool.Default:
            default:
                if (tile.RoomScript != null && !tile.RoomScript.IsRoom())
                    tile.DestroyRoom();
                break;
        }
    }

    public void SetTool(int tool)
    {
        currentTool = (BaseBuildingTool)tool;
    }
}
