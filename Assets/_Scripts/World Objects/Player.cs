using UnityEngine;
using System.Collections;

public class Player : WorldObject
{
    public Color color;

    public void Initialize(int _id, Color _color)
    {
        id = _id;
        color = _color;
    }
}
