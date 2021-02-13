using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pattern))]
public class PatternEditor : Editor
{
    Pattern pattern;
    public void OnEnable()
    {
        pattern = (Pattern)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        if (GUILayout.Button("Setup Values"))
        {
            pattern.CreateValues();
            EditorUtility.SetDirty(pattern);
        }
    }
}
