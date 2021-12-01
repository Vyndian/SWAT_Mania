using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WeightedObject))]
public class WeightedObjectDrawer : PropertyDrawer
{
    private float whiteSpace = 40f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var objectRect = new Rect(position.x, position.y, position.width - whiteSpace, position.height);
        var chanceRect = new Rect(position.x + position.width - whiteSpace, position.y, whiteSpace, position.height);
        EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("value"), GUIContent.none);
        EditorGUI.PropertyField(chanceRect, property.FindPropertyRelative("chance"), GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
