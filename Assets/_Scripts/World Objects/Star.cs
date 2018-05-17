using UnityEngine;
using System.Collections;

public class Star : WorldObject
{
    public Color color;
    public float size;

    public void Initialize(int _id, Color _color, float _size)
    {
        id = _id;
        color = _color;
        size = _size;
        transform.localScale = Vector3.one * size;
        GetComponent<SpriteRenderer>().color = color;

        // Visible circle
        Material mat = new Material(Shader.Find("Unlit/CircleHyperbolic"));
        mat.SetColor("_Color", color);
        mat.SetFloat("_Radius", 0.25f);
        mat.SetFloat("_Smoothness", 0.3f);
        MeshRenderer mr = transform.Find("Glow").gameObject.GetComponent<MeshRenderer>();
        mr.material = mat;
        /*
        // Hidden occluder circle
        Material mat2 = new Material(Shader.Find("Unlit/CircleHyperbolicOccluder"));
        mat2.SetColor("_Color", Color.clear);
        mat2.SetFloat("_Radius", 0.0f);
        mat2.SetFloat("_Smoothness", 0.0f);
        mat2.SetFloat("_AlphaCutoff", 0.0f);
        MeshRenderer mr2 = transform.Find("Occluder").GetComponent<MeshRenderer>();
        mr2.material = mat2;
        */

        // UI
        //GetComponent<TooltipWorldObject>().text = gameObject.name;
    }
}
