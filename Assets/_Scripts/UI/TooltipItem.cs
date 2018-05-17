using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class TooltipItem : MonoBehaviour {

    [SerializeField, TextArea(3, 10)]
    public string text = string.Empty;

    public void ShowTooltip()
    {
        Tooltip.singleton.Show(text);
    }

    public void HideTooltip()
    {
        Tooltip.singleton.Hide();
    }
}
