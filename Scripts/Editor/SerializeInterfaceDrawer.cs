using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeInterface<>))]
public class SerializeInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // draw fields
        var sourceProperty = property.FindPropertyRelative("value");
        EditorGUI.PropertyField(position, sourceProperty, label);

        // error check
        var referenceObject = sourceProperty.objectReferenceValue;
        var genericType = fieldInfo.FieldType.GetGenericArguments()[0];
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