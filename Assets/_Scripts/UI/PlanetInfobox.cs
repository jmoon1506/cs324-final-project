using UnityEngine;
using UnityEngine.UI;

public class PlanetInfobox : MonoBehaviour
{
    public static PlanetInfobox singleton;
    public Vector2 screenPos = new Vector2(10, 10);
    private RectTransform infoboxRect;

    void Awake()
    {
        singleton = this;
        infoboxRect = GetComponent<RectTransform>();
    }

    public void Show(Planet target)
    {
		infoboxRect.anchoredPosition = new Vector2(screenPos.x, -infoboxRect.rect.height - screenPos.y);
        transform.Find("Planet").Find("Name").GetComponent<UnityEngine.UI.Text>().text = target.gameObject.name;
        transform.Find("Planet").Find("Icon").GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(target.color, Color.white, 0.1f);
    }

    public void Hide()
    {
		infoboxRect.anchoredPosition = screenPos;
    }
}
