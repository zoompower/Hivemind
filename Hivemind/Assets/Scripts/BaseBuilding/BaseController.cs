using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    public int TeamID;

    [SerializeField]
    private int[] CollisionLayers;

    private int LayerMask = 0;

    [SerializeField]
    private BaseBuildingTool currentTool = BaseBuildingTool.Wall;

    [SerializeField]
    private GameObject WallPrefab;
    [SerializeField]
    private GameObject WorkerRoomPrefab;

    void Awake()
    {
        for (int i = 0; i < CollisionLayers.Length; i++)
        {
            LayerMask += 1 << CollisionLayers[i];
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, LayerMask))
            {
                GameObject plate;
                if (hit.transform.gameObject.layer == UnityEngine.LayerMask.NameToLayer("BaseFloor"))
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

        if (Input.GetButtonDown("Fire2") && !EventSystem.current.IsPointerOverGameObject())
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
