using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitController))]
public class UnitControllerEditor : Editor
{
    bool foldout;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UnitController myTarget = target as UnitController;

        myTarget.MindCount = EditorGUILayout.IntField("Mind Count", myTarget.MindCount);


        foldout = EditorGUILayout.Foldout(foldout, "Mind List");
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
