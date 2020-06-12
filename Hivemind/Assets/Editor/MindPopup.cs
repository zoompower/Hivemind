using UnityEditor;
using UnityEngine;

class MindPopup : PopupWindowContent
{
    DataEditor data = new DataEditor();

    public MindPopup(DataEditor data)
    {
        this.data = data;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public override void OnGUI(Rect rect)
    {
        data.type = (MindType)EditorGUILayout.EnumPopup(data.type);

        if (data.type == MindType.Gathering)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Carry Weight");
            data.CarryWeight = Mathf.Clamp(EditorGUILayout.IntField(data.CarryWeight), 0, 5);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Preferred Resource");
            data.PreferredResource = (ResourceType)EditorGUILayout.EnumPopup(data.PreferredResource);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Preferred Direction");
            data.PreferredDirection = (Gathering.Direction)EditorGUILayout.EnumPopup(data.PreferredDirection);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scouting");
            data.Scouting = EditorGUILayout.Toggle(data.Scouting);
            EditorGUILayout.EndHorizontal();
        }
    }
}