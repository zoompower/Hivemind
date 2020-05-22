using UnityEngine;

public class UiRoomController : MonoBehaviour
{
    BaseController baseController;

    void Start()
    {
        baseController = FindObjectOfType<BaseController>();
    }

    public void SetTool(int tool)
    {
        baseController.SetTool(tool);
    }
}
