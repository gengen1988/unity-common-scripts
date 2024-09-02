using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(InterfaceReference<>))]
public class InterfaceReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // draw fields
        SerializedProperty sourceProperty = property.FindPropertyRelative("Source");
        EditorGUI.PropertyField(position, sourceProperty, label);

        // error check
        Object referenceObject = sourceProperty.objectReferenceValue;
        Type genericType = fieldInfo.FieldType.GetGenericArguments()[0];
        if (!referenceObject)
        {
            EditorGUILayout.HelpBox($"reference not set", MessageType.Error);
        }
        else if (!genericType.IsInstanceOfType(referenceObject))
        {
            EditorGUILayout.HelpBox($"{referenceObject} must implements {genericType.Name}", MessageType.Error);
        }

        EditorGUI.EndProperty();
    }
}