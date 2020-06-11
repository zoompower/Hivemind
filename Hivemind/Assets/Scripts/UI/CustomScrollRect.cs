using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        FindObjectOfType<UiController>().OnDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        FindObjectOfType<UiController>().OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        FindObjectOfType<UiController>().OnEndDrag(eventData);
    }
}