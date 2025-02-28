using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FlowInput<>))]
public class FlowInputDrawer : PropertyDrawer
{
    private const float SWITCH_WIDTH = 16;
    private const float SWITCH_SPACING = 4;
    private const float LABEL_SPACING = 2;
    private const float FIELD_SPACING = 2;

    private int _lines = 1;
    private Rect _position;
    private GUIContent _label;
    private SerializedProperty _isOverrideProperty;
    private SerializedProperty _valueProperty;
    private SerializedProperty _sourceObjectProperty;
    private SerializedProperty _extractTypeProperty;
    private SerializedProperty _memberNameProperty;

    private float LineHeight => EditorGUIUtility.singleLineHeight;
    private float LineSpace => EditorGUIUtility.standardVerticalSpacing;
    private float LabelWidth => EditorGUIUtility.labelWidth;
    private float LabelBoundWidth => LabelWidth + LABEL_SPACING;
    private Type GenericType => fieldInfo.FieldType.GetGenericArguments()[0];

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // setup references
        _position = position;
        _label = label;
        _isOverrideProperty = property.FindPropertyRelative("IsOverride");
        _valueProperty = property.FindPropertyRelative("Value");
        _sourceObjectProperty = property.FindPropertyRelative("SourceObject");
        _extractTypeProperty = property.FindPropertyRelative("ExtractType");
        _memberNameProperty = property.FindPropertyRelative("MemberName");

        // draw
        EditorGUI.BeginProperty(position, label, property);

        DrawModeSwitch();
        if (_isOverrideProperty.boolValue)
        {
            DrawOverrideMode();
        }
        else
        {
            DrawReflectionMode();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return _lines * LineHeight + (_lines - 1) * LineSpace;
    }

    private void DrawModeSwitch()
    {
        // calc rect
        var switchRect = new Rect
        (
            _position.x + _position.width - SWITCH_WIDTH,
            _position.y,
            SWITCH_WIDTH,
            LineHeight
        );

        // draw
        EditorGUI.PropertyField(switchRect, _isOverrideProperty, GUIContent.none);

        // update position
        _position.width = _position.width - SWITCH_WIDTH - SWITCH_SPACING;
    }

    private void DrawOverrideMode()
    {
        _lines = 1;

        if (_valueProperty == null)
        {
            var labelRect = new Rect
            (
                _position.x,
                _position.y,
                LabelWidth,
                LineHeight
            );

            var messageRect = new Rect
            (
                _position.x + LabelBoundWidth,
                _position.y,
                _position.width - LabelBoundWidth,
                LineHeight
            );
            EditorGUI.LabelField(labelRect, _label);
            EditorGUI.HelpBox(messageRect, "value not serializable", MessageType.Info);
        }
        else
        {
            EditorGUI.PropertyField(_position, _valueProperty, _label);
        }
    }

    private void DrawReflectionMode()
    {
        _lines = 1;

        var labelRect = new Rect
        (
            _position.x,
            _position.y,
            LabelWidth,
            LineHeight
        );

        var remaining = new Rect
        (
            _position.x + LabelBoundWidth,
            _position.y,
            _position.width - LabelBoundWidth,
            LineHeight
        );

        var fieldWidth = remaining.width / 2 - FIELD_SPACING / 2;

        var leftRect = new Rect
        (
            remaining.x,
            remaining.y,
            fieldWidth,
            LineHeight
        );

        var rightRect = new Rect
        (
            remaining.x + remaining.width / 2 + FIELD_SPACING / 2,
            remaining.y,
            fieldWidth,
            LineHeight
        );

        // draw reference
        EditorGUI.LabelField(labelRect, _label);
        EditorGUI.PropertyField(leftRect, _sourceObjectProperty, GUIContent.none);

        var component = _sourceObjectProperty.objectReferenceValue;
        if (!component)
        {
            EditorGUI.HelpBox(rightRect, "no reference", MessageType.Error);
        }
        else
        {
            // find all options
            var options = FindOptions(GenericType, component.GetType());
            if (options.Length == 0)
            {
                EditorGUI.HelpBox(rightRect, "no matched type", MessageType.Warning);
            }
            else
            {
                // draw options selection
                var extractType = (ExtractType)_extractTypeProperty.enumValueIndex;
                var memberName = _memberNameProperty.stringValue;
                var currentOptionIndex = FindOptionIndex(options, extractType, memberName);
                var lines = GetDisplayText(options);
                var newIndex = EditorGUI.Popup(rightRect, currentOptionIndex, lines);
                if (newIndex < 0)
                {
                    EditorGUI.HelpBox(rightRect, "not set", MessageType.Warning);
                }
                else
                {
                    // update properties
                    var selected = options[newIndex];
                    if (selected.ExtractType != extractType)
                    {
                        _extractTypeProperty.enumValueIndex = (int)selected.ExtractType;
                    }

                    if (selected.MemberName != memberName)
                    {
                        _memberNameProperty.stringValue = selected.MemberName;
                    }
                }
            }
        }
    }

    private static string[] GetDisplayText(FlowOption[] options)
    {
        return options.Select(option => option.DisplayText).ToArray();
    }

    private static int FindOptionIndex(FlowOption[] options, ExtractType type, string memberName)
    {
        return Array.FindIndex(options, option => option.MemberName == memberName && option.ExtractType == type);
    }

    private static FlowOption[] FindOptions(Type genericType, Type sourceType)
    {
        const string displayPattern = "{0}{1} {2}{3}";

        var matchedFields = sourceType.GetFields()
            .Where(field => !field.IsDefined(typeof(ObsoleteAttribute)))
            .Where(field => genericType.IsAssignableFrom(field.FieldType))
            .Select(field => new FlowOption
            {
                ExtractType = ExtractType.Field,
                MemberName = field.Name,
                DisplayText = string.Format
                (
                    displayPattern,
                    field.IsStatic ? "static " : "",
                    field.FieldType.Name,
                    field.Name,
                    ""
                )
            });

        var matchedProperties = sourceType.GetProperties()
            .Where(property => !property.IsDefined(typeof(ObsoleteAttribute)))
            .Where(property => genericType.IsAssignableFrom(property.PropertyType))
            .Select(property => new FlowOption
            {
                ExtractType = ExtractType.Property,
                MemberName = property.Name,
                DisplayText = string.Format
                (
                    displayPattern,
                    property.GetGetMethod().IsStatic ? "static " : "",
                    property.PropertyType.Name,
                    property.Name,
                    " { get }"
                )
            });

        var matchedMethod = sourceType.GetMethods()
            .Where(method => !method.IsDefined(typeof(ObsoleteAttribute))) // no obsoleted
            .Where(method => genericType.IsAssignableFrom(method.ReturnType)) // subclass type matched
            .Where(method => method.GetParameters().Length == 0) // without arguments
            .Where(method => !(method.IsSpecialName && method.Name.StartsWith("get_"))) // no properties
            .Select(method => new FlowOption
            {
                ExtractType = ExtractType.Method,
                MemberName = method.Name,
                DisplayText = string.Format
                (
                    displayPattern,
                    method.IsStatic ? "static " : "",
                    method.ReturnType.Name,
                    method.Name,
                    " ( )"
                )
            });

        return Enumerable.Empty<FlowOption>()
            .Concat(matchedFields)
            .Concat(matchedProperties)
            .Concat(matchedMethod)
            .ToArray();
    }
}

public struct FlowOption
{
    public string DisplayText;
    public string MemberName;
    public ExtractType ExtractType;
}