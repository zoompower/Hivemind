using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitController))]
public class UnitControllerEditor : Editor
{
    bool foldout = false;

    Rect[] buttonRects = new Rect[0];

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UnitController myTarget = target as UnitController;
        foldout = EditorGUILayout.Foldout(foldout, "Mind List", true);
        if (foldout)
        {
            EditorGUI.indentLevel++;
            myTarget.MindCount = Mathf.Clamp(EditorGUILayout.IntField("Mind Count", myTarget.MindCount), 0, 10);

            if (myTarget.MindCount != buttonRects.Length)
            {
                Array.Resize(ref buttonRects, myTarget.MindCount);

                Array.Resize(ref myTarget.mindDatas, myTarget.MindCount);

                for (int i = 0; i < myTarget.mindDatas.Length; i++)
                {
                    if (myTarget.mindDatas[i] == null)
                    {
                        myTarget.mindDatas[i] = new DataEditor();
                    }
                }
            }

            for (int i = 0; i < myTarget.MindCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel($"{myTarget.mindDatas[i].type} Mind");
                if (GUILayout.Button($"Open"))
                {
                    PopupWindow.Show(buttonRects[i], new MindPopup(myTarget.mindDatas[i]));
                }
                if (Event.current.type == EventType.Repaint) buttonRects[i] = GUILayoutUtility.GetLastRect();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }
}
