﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    public int TeamID;

    [SerializeField]
    private int[] CollisionLayers;

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

    [NonSerialized]
    public QueenRoom QueenRoom;
    [SerializeField]
    public Transform TeleporterExit;
    [SerializeField]
    public Transform TeleporterEntrance;

    private GameObject highlightObj;

    public event EventHandler<ToolChangedEventArgs> OnToolChanged;

    void Awake()
    {
        for (int i = 0; i < CollisionLayers.Length; i++)
        {
            LayerMask += 1 << CollisionLayers[i];
        }

        InvokeRepeating("VerifyBuildingTasks", 1.0f, 5.0f);

        GameWorld.AddNewTeam(TeamID);
    }

    private void Start()
    {
        BuildingQueue = new BuildingQueue(this);

        SetTool((int)BaseBuildingTool.Default);
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

            if ((HighlightedTile == null || baseTile != HighlightedTile) && highlightObj == null)
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
        if  (OnToolChanged != null)
        {
            OnToolChanged.Invoke(this, new ToolChangedEventArgs(currentTool, (BaseBuildingTool)tool));
        }

        currentTool = (BaseBuildingTool)tool;
    }

    private void VerifyBuildingTasks()
    {
        BuildingQueue.VerifyTasks();
    }
}
