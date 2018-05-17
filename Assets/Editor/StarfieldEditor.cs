using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Starfield))]
public class StarfieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Starfield myScript = (Starfield)target;
        if(GUILayout.Button("Setup star field"))
        {
            myScript.CreateStarfield();
        }
    }
}