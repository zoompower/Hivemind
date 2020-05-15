using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatMindUI : MonoBehaviour, IMindUI
{
    public GameObject Ui_IconObj { get; set; }

    public GameObject UI_IconField;
    public GameObject UIMindBuilder;

    // Start is called before the first frame update
    void Start()
    {
        Ui_IconObj = Instantiate(UI_IconField);
       Ui_IconObj.transform.SetParent(this.transform, false);
    }

    public void UpdateMindLayout()
    {
        RectTransform rect = Ui_IconObj.GetComponent<RectTransform>();
        rect.position = GetComponent<RectTransform>().position;
    }

    public IMind MakeNewMind()
    {
        return new CombatMind(0, 0);
    }
}
