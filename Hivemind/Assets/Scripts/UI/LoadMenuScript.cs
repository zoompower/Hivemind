using UnityEngine;
using UnityEngine.UIElements;

public class LoadMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject prevPanel;
    private

    void Start()
    {
    }

    void Update()
    {

    }
    public void Back()
    {
        gameObject.SetActive(false);
        if (prevPanel != null)
        {
            prevPanel.SetActive(true);
        }
    }
}
