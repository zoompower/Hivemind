using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorkerRoomButton : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    private void Start()
    {
        //eventsomewhereidk += UpdateText;
    }

    private void UpdateText(object sender, int args)
    {
        myText.text = $"Worker Room \n(Costs 10 rocks) \n({50 - args} Remaining)";
    }
}
