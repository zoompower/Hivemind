using Assets.Scripts;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorkerRoomButton : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    private void Start()
    {
        GameWorld.Instance.UnitControllerList.Where(controller => controller.TeamId == GameWorld.Instance.LocalTeamId).FirstOrDefault().MindGroupList.OnAmountGet += UpdateText;
    }

    private void UpdateText(object sender, AmountChangedEventArgs args)
    {
        myText.text = $"Worker Room \n(Costs 10 rocks) \n({GameWorld.UnitLimit - args.Amount} Remaining)";
    }
}
