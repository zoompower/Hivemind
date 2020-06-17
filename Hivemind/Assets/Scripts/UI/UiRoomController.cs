using UnityEngine;

public class UiRoomController : MonoBehaviour
{
    private BaseController baseController;

    private void Start()
    {
        var controllers = FindObjectsOfType<BaseController>();

        foreach (var controller in controllers)
        {
            if (controller.TeamID == GameWorld.Instance.LocalTeamId)
            {
                baseController = controller;
                break;
            }
        }
    }

    public void SetTool(int tool)
    {
        baseController.SetTool(tool);
    }
}