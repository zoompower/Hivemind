using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitController))]
public class UnitControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UnitController myTarget = target as UnitController;

        myTarget.MindCount = EditorGUILayout.IntField("Mind Count", myTarget.MindCount);

        EditorGUI.indentLevel++;
        for (int i = 0; i < myTarget.MindCount; i++)
        {
            
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Build Object"))
        {
            
        }
    }
}
