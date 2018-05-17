using UnityEngine;

public class TooltipWorldObject : TooltipItem 
{
    void Start()
    {
        string objText = gameObject.name;
        Ship ship = GetComponent<Ship>();
        if (ship != null)
        {
            if (ship.location != 0)
            {
                objText = objText + "\nOrbiting " + GameManager.singleton.Find(ship.location).name;
            }
        }
        text = objText;
    }

    void OnMouseEnter()
    {
        ShowTooltip();
    }

    void OnMouseExit()
    {
        HideTooltip();
    }
}
