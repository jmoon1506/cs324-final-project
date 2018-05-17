using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipGUIItem : TooltipItem, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler {

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }
}
