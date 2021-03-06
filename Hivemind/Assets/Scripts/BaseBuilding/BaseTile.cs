﻿using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    [SerializeField]
    private GameObject StartObject = null;

    internal GameObject CurrTile;
    internal BaseRoom RoomScript;
    internal bool AstarVisited = false;

    internal List<BaseTile> Neighbors = new List<BaseTile>();

    [SerializeField]
    internal bool IsIndestructable = false;

    [SerializeField]
    internal bool IsUnbuildable = false;

    [SerializeField]
    private int DefaultRotation = -1;

    [SerializeField]
    private float CollisionSize = 0.50f;

    [SerializeField]
    private bool ShowDebugInfo = false;

    [SerializeField]
    private bool ShowMeshPreview = false;

    [SerializeField]
    private Color MeshColor = new Color(53.0f / 255.0f, 124.0f / 255.0f, 44.0f / 255.0f);

    [SerializeField]
    private bool HideTileIfSubTileExists = true;

    [SerializeField]
    internal GameObject HighlightPrefab;

    internal bool Loaded = false;

    private void Awake()
    {
        FindAndAttachNeighbors();
        Astar.RegisterResetableRoom(this);
    }

    private void Start()
    {
        if (StartObject)
        {
            InitializeObject(StartObject, true);
        }
    }

    private void Update()
    {
        if (!Loaded)
            Loaded = true;
    }

    private void OnDestroy()
    {
        Astar.RemoveResetableRoom(this);
    }

    internal void InitializeObject(GameObject gObj, bool force = false)
    {
        if (!force && (CurrTile != null || IsUnbuildable)) return;

        CurrTile = Instantiate(gObj);
        CurrTile.transform.SetParent(gameObject.transform, false);
        CurrTile.transform.localRotation = Quaternion.Euler(0, (DefaultRotation < 0) ? UnityEngine.Random.Range(0, 5) * 60 : DefaultRotation, 0);

        RoomScript = CurrTile.GetComponent<BaseRoom>();

        if (HideTileIfSubTileExists)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    internal void DestroyRoom(bool forced = false)
    {
        if (CurrTile == null) return;

        if ((!IsIndestructable && RoomScript.IsDestructable()) || forced)
        {
            RoomScript.Destroy();
            CurrTile = null;
            RoomScript = null;
            if (HideTileIfSubTileExists)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void AntDoesAction(BaseBuildingTool tool)
    {
        switch (tool)
        {
            case BaseBuildingTool.DestroyRoom:
                if (!IsIndestructable)
                {
                    DestroyRoom();
                }
                break;

            case BaseBuildingTool.Wall:
                InitializeObject(GetComponentInParent<BaseController>().WallPrefab);
                break;

            case BaseBuildingTool.AntRoom:
                if (!IsUnbuildable)
                {
                    InitializeObject(GetComponentInParent<BaseController>().WorkerRoomPrefab);
                }
                break;

            default:
                if (RoomScript != null && !RoomScript.IsRoom() && !IsIndestructable)
                {
                    DestroyRoom();
                }
                break;
        }
    }

    private void FindAndAttachNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CollisionSize);
        colliders = colliders.Where(c => c.gameObject.layer == gameObject.layer && c.gameObject != gameObject).ToArray();

        foreach (Collider hit in colliders)
        {
            Neighbors.Add(hit.GetComponent<BaseTile>());
        }
    }

    private void OnDrawGizmos()
    {
        if (StartObject != null && StartObject.GetComponent<MeshFilter>() != null && ShowMeshPreview)
        {
            Gizmos.color = MeshColor;
            Gizmos.DrawWireMesh(StartObject.GetComponent<MeshFilter>().sharedMesh, transform.position, Quaternion.Euler(0, (DefaultRotation < 0) ? 0 : DefaultRotation, 0), transform.localScale);
        }

        if (ShowDebugInfo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CollisionSize);
        }
    }

    public BaseTileData GetData()
    {
        return new BaseTileData(RoomScript, CurrTile, gameObject.name, RoomScript is BaseUnitRoom ? (RoomScript as BaseUnitRoom).GroupId : Guid.Empty, RoomScript is BaseUnitRoom ? (RoomScript as BaseUnitRoom).RespawnTimer : -0);
    }

    public void SetData(BaseTileData data)
    {
        if (data.RoomType != RoomType.QueenRoom)
        {
            Destroy(CurrTile);
            CurrTile = null;
            RoomScript = null;
            if (HideTileIfSubTileExists)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
        }
        switch (data.RoomType)
        {
            case RoomType.None:
                break;

            case RoomType.Wall:
                InitializeObject(GetComponentInParent<BaseController>().WallPrefab, true);
                break;

            case RoomType.WorkerRoom:
                InitializeObject(GetComponentInParent<BaseController>().WorkerRoomPrefab, true);
                (RoomScript as BaseUnitRoom).GroupId = Guid.Parse(data.GroupID);
                (RoomScript as BaseUnitRoom).RespawnTimer = data.RespawnTime;
                break;

            case RoomType.ScalingWorkerRoom:
                InitializeObject(GetComponentInParent<BaseController>().ScalingWorkerRoomPrefab, true);
                (RoomScript as BaseUnitRoom).GroupId = Guid.Parse(data.GroupID);
                (RoomScript as BaseUnitRoom).RespawnTimer = data.RespawnTime;
                break;
        }
        if (CurrTile != null)
        {
            CurrTile.transform.localEulerAngles = new Vector3(0, data.RotationY, 0);
        }
    }
}