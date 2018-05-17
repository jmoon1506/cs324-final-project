using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager myScript = (GameManager)target;
        if(GUILayout.Button("Setup test environment"))
        {
            myScript.SetupTest();
        }
    }
}
