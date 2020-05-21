using UnityEngine;

public class CombatMindUI : MonoBehaviour, IMindUI
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
        return new CombatMind(0, 0);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Ui_IconObj = Instantiate(UI_IconField);
        Ui_IconObj.transform.SetParent(transform, false);
    }
}