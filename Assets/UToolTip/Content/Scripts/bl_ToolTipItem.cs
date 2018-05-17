using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class bl_ToolTipItem : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea(3, 10)]
    private string m_Text = string.Empty;
    [Space(5)]
    [Range(0.0f,5.0f)]
    public float WaitForShow = 0.2f;
    public bool TakeFromImage;
    public Sprite m_Icon = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (m_Icon == null && TakeFromImage)
        {
            if (GetComponent<Image>() != null)
            {
                m_Icon = GetComponent<Image>().sprite;
            }
        }

        bl_ToolTip.Instance.ShowToolTip(true, WaitForShow,m_Icon, m_Text);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        bl_ToolTip.Instance.ShowToolTip(false);
    }
}