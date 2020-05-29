using System;
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
    internal GameObject WallPrefab;
    [SerializeField]
    internal GameObject WorkerRoomPrefab;

    private BaseTile HighlightedTile;

    public BuildingQueue BuildingQueue;

    [NonSerialized]
    public QueenRoom QueenRoom;
    [SerializeField]
    public Transform TeleporterExit;
    [SerializeField]
    public Transform TeleporterEntrance;

    void Awake()
    {
        for (int i = 0; i < CollisionLayers.Length; i++)
        {
            LayerMask += 1 << CollisionLayers[i];
        }

        InvokeRepeating("VerifyBuildingTasks", 1.0f, 5.0f);
    }

    private void Start()
    {
        BuildingQueue = new BuildingQueue(this);
    }

    void Update()
    {
        if (true) // TODO: if you are the owner of the base
        {
            Highlight();
        }

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

    private void OnDestroy()
    {
        CancelInvoke("VerifyBuildingTasks");
    }
    
    GameObject highlight;
    private void Highlight()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100.0f, LayerMask) && !EventSystem.current.IsPointerOverGameObject())
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

            var baseTile = plate.GetComponent<BaseTile>();

            if ((HighlightedTile == null || baseTile != HighlightedTile) && highlight == null)
            {
                if (baseTile.CurrTile == null || baseTile.RoomScript.HighlightPrefab == null)
                {
                    highlight = Instantiate(baseTile.HighlightPrefab);
                }
                else
                {
                    highlight = Instantiate(baseTile.RoomScript.HighlightPrefab);
                }

                HighlightedTile = baseTile;
                highlight.transform.SetParent(baseTile.transform, false);
            }
            else if (HighlightedTile != null && baseTile != HighlightedTile || baseTile == null)
            {
                Destroy(highlight);
                HighlightedTile = null;
            }
        }
        else
        {
            if (highlight != null)
            {
                Destroy(highlight);
                HighlightedTile = null;
            }
        }
    }

    internal GameObject GetHighlightObj(BaseBuildingTool tool)
    {
        switch (tool)
        {
            case BaseBuildingTool.Default:
                return WallPrefab.GetComponent<BaseRoom>().HighlightPrefab;
            case BaseBuildingTool.Destroy:
                return WorkerRoomPrefab.GetComponent<BaseRoom>().HighlightPrefab;
            case BaseBuildingTool.Wall:
                return WallPrefab.GetComponent<BaseRoom>().HighlightPrefab;
            case BaseBuildingTool.AntRoom:
                return WorkerRoomPrefab.GetComponent<BaseRoom>().HighlightPrefab;
        }
        return null;
    }
    
    private void OnLeftClick(BaseTile tile)
    {
        BuildingQueue.AddNewJob(tile, currentTool);
    }

    public void SetTool(int tool)
    {
        currentTool = (BaseBuildingTool)tool;
    }

    private void VerifyBuildingTasks()
    {
        BuildingQueue.VerifyTasks();
    }
}
