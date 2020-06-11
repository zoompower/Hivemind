using Assets.Scripts.Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    public int TeamID;

    [SerializeField]
    private LayerMask[] ColisionLayers;
    private int LayerMask = 0;

    private BaseBuildingTool currentTool;

    [SerializeField]
    internal GameObject WallPrefab;
    [SerializeField]
    internal GameObject WorkerRoomPrefab;
    [SerializeField]
    internal GameObject UnbuildablePrefab;
    [SerializeField]
    internal GameObject IndestructablePrefab;

    private BaseTile HighlightedTile;

    public BuildingQueue BuildingQueue;

    public QueenRoom QueenRoom { get; private set; }
    [SerializeField]
    public Transform TeleporterExitTransform;
    [SerializeField]
    public Transform TeleporterEntranceTransform;

    [NonSerialized]
    public Vector3 TeleporterExit;
    [NonSerialized]
    public Vector3 TeleporterEntrance;

    private GameObject highlightObj;

    public event EventHandler<ToolChangedEventArgs> OnToolChanged;

    private GameResources gameResources = new GameResources();

    [SerializeField]
    public Color TeamColor;

    void Awake()
    {
        if (TeleporterEntranceTransform != null && TeleporterExitTransform != null)
        {
            TeleporterExit = TeleporterExitTransform.position;
            TeleporterEntrance = TeleporterEntranceTransform.position;
        }
        for (int i = 0; i < ColisionLayers.Length; i++)
        {
            LayerMask += ColisionLayers[i].value;
        }

        InvokeRepeating("VerifyBuildingTasks", 1.0f, 5.0f);
        GameWorld.Instance.AddBaseController(this);
    }

    private void Start()
    {
        BuildingQueue = new BuildingQueue(this);

        SetTool((int)BaseBuildingTool.Default);
    }

    void Update()
    {
        if (TeamID != GameWorld.Instance.LocalTeamId) return;

        Highlight();

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

                if (plate.transform.GetComponentInParent<BaseController>().TeamID == GameWorld.Instance.LocalTeamId)
                {
                    OnLeftClick(plate.GetComponent<BaseTile>());
                }
            }
        }

        if (Input.GetButtonDown("Fire2") && !EventSystem.current.IsPointerOverGameObject())
        {
            SetTool(0);
        }
    }

    private void OnDestroy()
    {
        GameWorld.Instance.RemoveBaseController(this);
        CancelInvoke("VerifyBuildingTasks");
    }

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

            if (baseTile.transform.GetComponentInParent<BaseController>().TeamID == GameWorld.Instance.LocalTeamId && (HighlightedTile == null || baseTile != HighlightedTile) && highlightObj == null)
            {
                GameObject highlight = GetHightLightWithTool(baseTile);
                if (highlight)
                {
                    highlightObj = Instantiate(highlight);
                    HighlightedTile = baseTile;
                    highlightObj.transform.SetParent(baseTile.transform, false);
                }
            }
            else if (HighlightedTile != null && baseTile != HighlightedTile)
            {
                Destroy(highlightObj);
                HighlightedTile = null;
            }
        }
        else
        {
            if (highlightObj != null)
            {
                Destroy(highlightObj);
                HighlightedTile = null;
            }
        }
    }

    private GameObject GetHightLightWithTool(BaseTile tile)
    {
        if ((HighlightedTile == null || tile != HighlightedTile) && highlightObj == null && (tile.IsUnbuildable || tile.IsIndestructable))
        {
            if (tile.IsIndestructable && tile.RoomScript != null && !tile.RoomScript.IsRoom())
            {
                return IndestructablePrefab;
            }
            else if (tile.IsUnbuildable)
            {
                return UnbuildablePrefab;
            }
        }

        switch (currentTool)
        {
            case BaseBuildingTool.Default:
                if (tile.RoomScript != null && !tile.RoomScript.IsRoom())
                    return tile.RoomScript.HighlightPrefab;
                break;
            case BaseBuildingTool.DestroyRoom:
                if (tile.RoomScript != null && tile.RoomScript.IsRoom())
                    return tile.RoomScript.HighlightPrefab;
                break;
            case BaseBuildingTool.Wall:
                // TODO: when building walls is gonna be a thing
                break;
            case BaseBuildingTool.AntRoom:
                if (tile.RoomScript == null)
                    return GetHighlightObj(BaseBuildingTool.AntRoom);
                break;
        }

        return null;
    }

    internal GameObject GetHighlightObj(BaseBuildingTool tool)
    {
        switch (tool)
        {
            case BaseBuildingTool.Default:
                return WallPrefab.GetComponent<BaseRoom>().HighlightPrefab;
            case BaseBuildingTool.DestroyRoom:
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
        if (OnToolChanged != null)
        {
            OnToolChanged.Invoke(this, new ToolChangedEventArgs(currentTool, (BaseBuildingTool)tool));
        }

        currentTool = (BaseBuildingTool)tool;
    }

    private void VerifyBuildingTasks()
    {
        BuildingQueue.VerifyTasks();
    }

    public void SetQueenRoom(QueenRoom queenRoom)
    {
        QueenRoom = queenRoom;
    }

    public Vector3 GetPosition()
    {
        return QueenRoom.GetPosition();
    }

    public GameResources GetGameResources()
    {
        return gameResources;
    }

    public BaseControllerData GetData()
    {
        return new BaseControllerData(TeamID, currentTool, TeleporterExit, TeleporterEntrance, BuildingQueue, gameObject.transform, gameResources);
    }

    public void SetData(BaseControllerData data)
    {
        TeamID = data.TeamID;
        SetTool((int)data.CurrentTool);
        TeleporterExit = new Vector3(data.TeleporterExitX, data.TeleporterExitY, data.TeleporterExitZ);
        TeleporterEntrance = new Vector3(data.TeleporterEntranceX, data.TeleporterEntranceY, data.TeleporterEntranceZ);
        BuildingQueue.SetData(data.queueData);

        foreach (BaseTileData baseTileData in data.BaseTileData)
        {
            gameObject.transform.Find(baseTileData.Name).GetComponent<BaseTile>().SetData(baseTileData);
        }

        gameResources.SetData(data.GameResources);
    }
}
