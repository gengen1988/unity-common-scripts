using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PullSource
{
	public OutputType MemberType;
	public string MemberName;
	public string DisplayText;
}

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
	private SerializedProperty _dynamicProperty;
	private SerializedProperty _valueProperty;
	private SerializedProperty _referenceProperty;
	private SerializedProperty _outputTypeProperty;
	private SerializedProperty _outputNameProperty;

	private float LineHeight => EditorGUIUtility.singleLineHeight;
	private float LineSpace => EditorGUIUtility.standardVerticalSpacing;
	private float LabelWidth => EditorGUIUtility.labelWidth;
	private float LabelBoundWidth => LabelWidth + LABEL_SPACING;
	private bool IsDynamic => _dynamicProperty.boolValue;
	private Object SourceInstance => _referenceProperty.objectReferenceValue;
	private Type RequireType => fieldInfo.FieldType.GetGenericArguments()[0];

	private OutputType OutputType
	{
		get => (OutputType)_outputTypeProperty.enumValueIndex;
		set => _outputTypeProperty.enumValueIndex = (int)value;
	}

	private string OutputName
	{
		get => _outputNameProperty.stringValue;
		set => _outputNameProperty.stringValue = value;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// update data
		_position = position;
		_label = label;
		FindProperties(property);

		// draw
		EditorGUI.BeginProperty(position, label, property);

		DrawModeSwitch();

		if (IsDynamic)
		{
			DrawDynamicMode();
		}
		else
		{
			DrawManualMode();
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
		Rect switchRect = new
		(
			_position.x + _position.width - SWITCH_WIDTH,
			_position.y,
			SWITCH_WIDTH,
			LineHeight
		);

		// draw
		EditorGUI.PropertyField(switchRect, _dynamicProperty, GUIContent.none);

		// update position
		_position.width = _position.width - SWITCH_WIDTH - SWITCH_SPACING;
	}

	private void DrawManualMode()
	{
		_lines = 1;

		if (_valueProperty == null)
		{
			Rect labelRect = new
			(
				_position.x,
				_position.y,
				LabelWidth,
				LineHeight
			);

			Rect messageRect = new
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

	private void DrawDynamicMode()
	{
		_lines = 1;

		Rect labelRect = new
		(
			_position.x,
			_position.y,
			LabelWidth,
			LineHeight
		);

		Rect remaining = new
		(
			_position.x + LabelBoundWidth,
			_position.y,
			_position.width - LabelBoundWidth,
			LineHeight
		);

		float fieldWidth = remaining.width / 2 - FIELD_SPACING / 2;

		Rect leftRect = new
		(
			remaining.x,
			remaining.y,
			fieldWidth,
			LineHeight
		);

		Rect rightRect = new
		(
			remaining.x + remaining.width / 2 + FIELD_SPACING / 2,
			remaining.y,
			fieldWidth,
			LineHeight
		);

		EditorGUI.LabelField(labelRect, _label);
		EditorGUI.PropertyField(leftRect, _referenceProperty, GUIContent.none);

		if (!SourceInstance)
		{
			EditorGUI.HelpBox(rightRect, "no reference", MessageType.Error);
		}
		else
		{
			PullSource[] options = FindOptions(SourceInstance.GetType(), RequireType);
			if (options.Length == 0)
			{
				EditorGUI.HelpBox(rightRect, "no matched type", MessageType.Warning);
			}
			else
			{
				int currentIndex = FindOptionIndex(options, OutputType, OutputName);
				string[] texts = options.Select(option => option.DisplayText).ToArray();
				int newIndex = EditorGUI.Popup(rightRect, currentIndex, texts);
				if (newIndex < 0)
				{
					EditorGUI.HelpBox(rightRect, "reference changed", MessageType.Warning);
				}
				else
				{
					// update properties
					PullSource newOption = options[newIndex];
					OutputType = newOption.MemberType;
					OutputName = newOption.MemberName;
				}
			}
		}
	}

	private void FindProperties(SerializedProperty property)
	{
		_dynamicProperty = property.FindPropertyRelative("Dynamic");
		_valueProperty = property.FindPropertyRelative("Value");
		_referenceProperty = property.FindPropertyRelative("OutputInstance");
		_outputTypeProperty = property.FindPropertyRelative("OutputType");
		_outputNameProperty = property.FindPropertyRelative("OutputMemberName");
	}

	private int FindOptionIndex(PullSource[] options, OutputType outputType, string outputName)
	{
		for (int i = 0; i < options.Length; ++i)
		{
			PullSource option = options[i];
			if (option.MemberName == outputName && option.MemberType == outputType)
			{
				return i;
			}
		}

		return -1;
	}

	private PullSource[] FindOptions(Type ownerType, Type givenType)
	{
		const string displayPattern = "{0}{1} {2}{3}";

		IEnumerable<PullSource> matchedFields = ownerType.GetFields()
			.Where(field => !field.IsDefined(typeof(ObsoleteAttribute)))
			.Where(field => givenType.IsAssignableFrom(field.FieldType))
			.Select(field => new PullSource
			{
				MemberName = field.Name,
				MemberType = OutputType.Field,
				DisplayText = string.Format
				(
					displayPattern,
					field.IsStatic ? "static " : "",
					field.FieldType.Name,
					field.Name,
					""
				)
			});

		IEnumerable<PullSource> matchedProperties = ownerType.GetProperties()
			.Where(property => !property.IsDefined(typeof(ObsoleteAttribute)))
			.Where(property => givenType.IsAssignableFrom(property.PropertyType))
			.Select(property => new PullSource
			{
				MemberName = property.Name,
				MemberType = OutputType.Property,
				DisplayText = string.Format
				(
					displayPattern,
					property.GetGetMethod().IsStatic ? "static " : "",
					property.PropertyType.Name,
					property.Name,
					" { get }"
				)
			});

		IEnumerable<PullSource> matchedMethod = ownerType.GetMethods()
			.Where(method => !(method.IsSpecialName && method.Name.StartsWith("get_"))) // no properties
			.Where(method => !method.IsDefined(typeof(ObsoleteAttribute))) // no obsoleted
			.Where(method => method.GetParameters().Length == 0) // without arguments
			.Where(method => givenType.IsAssignableFrom(method.ReturnType)) // sub class
			.Select(method => new PullSource
			{
				MemberName = method.Name,
				MemberType = OutputType.Method,
				DisplayText = string.Format
				(
					displayPattern,
					method.IsStatic ? "static " : "",
					method.ReturnType.Name,
					method.Name,
					" ( )"
				)
			});

		return matchedFields.Concat(matchedProperties).Concat(matchedMethod).ToArray();
	}
}