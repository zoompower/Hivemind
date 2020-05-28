using System.Collections.Generic;
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

    public QueenRoom QueenRoom;

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
            //Highlight();
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

            var baseTile = plate.GetComponent<BaseTile>();

            if ((HighlightedTile == null || baseTile != HighlightedTile) && highlight == null)
            {
                if (baseTile.CurrTile == null)
                {
                    return;
                    highlight = Instantiate(new GameObject());
                    highlight.AddComponent<MeshRenderer>();
                    highlight.GetComponent<MeshRenderer>().material = baseTile.GetComponent<MeshRenderer>().material;
                    highlight.AddComponent<MeshFilter>();
                    highlight.GetComponent<MeshFilter>().sharedMesh = Instantiate(baseTile.GetComponent<MeshFilter>().sharedMesh);

                    //highlight = Instantiate(baseTile.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                }
                else
                {
                    highlight = Instantiate(baseTile.CurrTile);
                }

                HighlightedTile = baseTile;
                highlight.transform.SetParent(baseTile.transform, false);
                highlight.transform.localPosition += new Vector3(0, 4, 0);
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
