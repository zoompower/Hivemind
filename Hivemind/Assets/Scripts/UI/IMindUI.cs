using UnityEngine;

internal interface IMindUI
{
    GameObject Ui_IconObj { get; set; }

    void UpdateMindLayout();

    IMind MakeNewMind();
}