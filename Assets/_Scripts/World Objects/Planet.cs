using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : WorldObject
{
    public static Planet selected;
    public static Planet hover;
    public Vector3 mousePos;

    public Color color;
    public float size;

    public List<Ship> ships;

    public void Initialize(int _id, Color _color, float _size)
    {
        id = _id;
        color = _color;
        size = _size;
        ships = new List<Ship>();
        transform.localScale = Vector3.one * size;
        transform.Find("Body").GetComponent<SpriteRenderer>().color = color;
        GetComponent<Orbit>().orbitColor = new Color(color.r, color.g, color.b, 0.5f);
        /*
        Material mat = new Material(Shader.Find("Unlit/CircleHyperbolicOccluder"));
        mat.SetColor("_Color", color);
        mat.SetFloat("_Radius", 0.1f);
        mat.SetFloat("_Smoothness", 0.005f);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = mat;
        */

        // UI
        //GetComponent<TooltipWorldObject>().text = gameObject.name;
    }

    void OnMouseEnter()
    {
        Hover();
    }

    void OnMouseExit()
    {
        Unhover();
    }

    void OnMouseUpAsButton()
    {
        Select();
    }

    void Update()
    {
        // Selection logic
        if (selected == this)
        {
            /*
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider == null)
                {
                    //Debug.Log(hit.collider.name);
                    Deselect();
                }
            }
            */
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (mousePos == Input.mousePosition)
                {
                    Deselect();
                }
                else
                {
                    mousePos = new Vector3();
                }
            }
        }
    }

    public void Hover()
    {
        hover = this;
        transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.Lerp(color, Color.white, 0.25f);
    }

    public void Unhover()
    {
        if (hover == this)
        {
            hover = null;
        }
        transform.Find("Body").GetComponent<SpriteRenderer>().color = color;
    }

    public void Select()
    {
        if (selected != null)
        {
            selected.Deselect();
        }
        selected = this;
        //GetComponent<SpriteRenderer>().color = color;
        //GetComponent<SpriteRenderer>().color = Color.magenta;
        PlanetInfobox.singleton.Show(this);
    }

    public void Deselect()
    {
        //GetComponent<SpriteRenderer>().color = color;
        if (hover == null || hover == this)
        {
            selected = null;
            PlanetInfobox.singleton.Hide();
        }
    }

    /*
    public void AddShip(Ship ship)
    {
    }

    public void RemoveShip(Ship ship)
    {
    }
    */
}
