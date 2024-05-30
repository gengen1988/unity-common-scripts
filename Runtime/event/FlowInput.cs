using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public enum OutputType
{
	Field,
	Property,
	Method
}

[Serializable]
public class FlowInput<T>
{
	[SerializeField]
	private bool Dynamic = true;

	[SerializeField]
	private Component OutputInstance;

	[SerializeField]
	private OutputType OutputType;

	[SerializeField]
	private string OutputMemberName;

	[SerializeField]
	private T Value;

	private object _rawValue;

	public T Pull()
	{
		if (!Dynamic)
		{
			return Value;
		}

		// get raw value
		Component owner = OutputInstance;
		Type ownerType = owner.GetType();
		switch (OutputType)
		{
			case OutputType.Field:
				FieldInfo field = ownerType.GetField(OutputMemberName);
				_rawValue = field.GetValue(owner);
				break;
			case OutputType.Property:
				PropertyInfo property = ownerType.GetProperty(OutputMemberName);
				Assert.IsNotNull(property);
				_rawValue = property.GetValue(owner);
				break;
			case OutputType.Method:
				MethodInfo method = ownerType.GetMethod(OutputMemberName);
				Assert.IsNotNull(method);
				_rawValue = method.Invoke(owner, null);
				break;
		}

		// convert
		if (_rawValue is not T typedValue)
		{
			return default;
		}

		Value = typedValue;
		return typedValue;
	}
}