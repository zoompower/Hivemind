using Assets.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorkerRoomButton : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    [SerializeField]
    private Button myButton;

    private BaseController myBaseController;

    private void Start()
    {
        myBaseController = GameWorld.Instance.BaseControllerList.Where(controller => controller.TeamID == GameWorld.Instance.LocalTeamId).FirstOrDefault();
        GameWorld.Instance.UnitControllerList.Where(controller => controller.TeamId == GameWorld.Instance.LocalTeamId).FirstOrDefault().MindGroupList.OnAmountGet += UpdateText;
    }

    private void UpdateText(object sender, AmountChangedEventArgs args)
    {
        myText.text = $"Worker Room \n(Costs 10 rocks) \n({GameWorld.UnitLimit - args.Amount} Remaining)";
        if (args.Amount >= GameWorld.UnitLimit)
        {
            myButton.interactable = false;
        }
        else if (myBaseController.GetGameResources().GetResourceAmounts()[ResourceType.Rock] >= 10)
        {
            myButton.interactable = true;
        }
    }
}
