using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataEditor))]
internal class DataEditorDrawer : PropertyDrawer
{
    private Rect contentPosition;

    private float GatheringPropCount = 4;
    private float CombatPropCount = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Label (element X)
        label = EditorGUI.BeginProperty(position, label, property);
        contentPosition = EditorGUI.PrefixLabel(position, label);

        position.height = 16f;
        EditorGUI.indentLevel += 1;
        contentPosition = EditorGUI.IndentedRect(position);

        // Type enum
        contentPosition.y += 18f;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("type"));

        if (property.FindPropertyRelative("type").intValue == (int)MindType.Gathering)
        {
            // Carry Weight integer
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("CarryWeight"));

            // Preferred Resource enum
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("PreferredResource"));

            // Preferred Direction enum
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("PreferredDirection"));

            // Scouting bool
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Scouting"));
        }

        if (property.FindPropertyRelative("type").intValue == (int)MindType.Combat)
        {
            // Vision Radius integer
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("VisionRadius"));

            // Scouting bool
            contentPosition.y += 18f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("AttackingQueen"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16f + 18f * (1f + ((property.FindPropertyRelative("type").intValue == (int)MindType.Gathering) ? GatheringPropCount : CombatPropCount));
    }
}