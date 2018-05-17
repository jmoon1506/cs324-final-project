using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    public static Tooltip singleton;
    public Vector2 offset;
    private bool visible;
    private string text;
    private UnityEngine.UI.Text textBox;
    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    private Vector2 scalingFactor;

    void Awake()
    {
        singleton = this;
        textBox = GetComponentInChildren<UnityEngine.UI.Text>();
        tooltipRect = GetComponent<RectTransform>();
        canvasRect = transform.parent.GetComponent<RectTransform>();
        scalingFactor = new Vector2(canvasRect.rect.width / Screen.width, canvasRect.rect.height / Screen.height);
        offset = new Vector2(10.0f, -10.0f);
        Hide();
        //rect.anchoredPosition = new Vector2(200, -300);
    }

    void Update()
    {
        if (!visible)
        {
            return;
        }

        Vector2 screenPos = Input.mousePosition;
        screenPos.x = scalingFactor.x * (screenPos.x + offset.x);
        screenPos.y = scalingFactor.y * ((screenPos.y + offset.y) - Screen.height);

        if (screenPos.x + tooltipRect.sizeDelta.x > canvasRect.rect.width)
        {
            screenPos.x = canvasRect.rect.width - tooltipRect.sizeDelta.x;
        }
        if (-screenPos.y + tooltipRect.sizeDelta.y > canvasRect.rect.height)
        {
            //Debug.Log(tooltipRect.sizeDelta.y);
            screenPos.y = -canvasRect.rect.height + tooltipRect.sizeDelta.y;
        }
        tooltipRect.anchoredPosition = screenPos;
    }

    public void Show(string _text)
    {
        text = _text;
        Invoke("Draw", GameManager.singleton.toolTipDelay);
    }

    public void Hide()
    {
        CancelInvoke("Draw");
        textBox.text = "";
        visible = false;
        tooltipRect.anchoredPosition = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
        //rect.position = new Vector2(-100, 100);
    }

    private void Draw()
    {
        visible = true;
        textBox.text = text;
    }
}
