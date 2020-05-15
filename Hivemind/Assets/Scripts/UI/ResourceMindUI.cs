using UnityEngine;

public class ResourceMindUI : MonoBehaviour, IMindUI
{
    public GameObject UI_IconField;
    public GameObject UIMindBuilder;
    public GameObject Ui_IconObj { get; set; }

    public void UpdateMindLayout()
    {
        var rect = Ui_IconObj.GetComponent<RectTransform>();
        rect.position = GetComponent<RectTransform>().position;
    }

    public IMind MakeNewMind()
    {
        return new Gathering(ResourceType.Unknown, 1, Gathering.Direction.None);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Ui_IconObj = Instantiate(UI_IconField);
        Ui_IconObj.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}