using System;
using System.Reflection;
using UnityEngine;

public static class ComponentUtil
{
    public static void InitAutoReference(this MonoBehaviour component)
    {
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            AutoReferenceAttribute attribute = field.GetCustomAttribute<AutoReferenceAttribute>();
            if (attribute == null)
            {
                continue;
            }

            object value = GetFieldValue(component, field, attribute);
            field.SetValue(component, value);
        }
    }

    private static object GetFieldValue(MonoBehaviour component, FieldInfo field, AutoReferenceAttribute attribute)
    {
        Type fieldType = field.FieldType;
        if (fieldType.IsArray)
        {
            Type elementType = fieldType.GetElementType();
            CheckType(elementType);
            Array components = GetComponents(component, elementType, attribute.Source);
            if (attribute.Required && components.Length == 0)
            {
                LogError(component, elementType, attribute.Source);
            }

            return components;
        }
        else
        {
            CheckType(fieldType);
            Component found = GetComponent(component, fieldType, attribute.Source);
            if (attribute.Required && !found)
            {
                LogError(component, fieldType, attribute.Source);
            }

            return found;
        }
    }

    private static void CheckType(Type type)
    {
        Debug.Assert(type.IsInterface || typeof(Component).IsAssignableFrom(type));
    }

    private static Array GetComponents(MonoBehaviour component, Type elementType, FindIn source)
    {
        switch (source)
        {
            case FindIn.Self:
                return component.GetComponents(elementType);
            case FindIn.Parent:
                return component.GetComponentsInParent(elementType);
            case FindIn.Children:
                return component.GetComponentsInChildren(elementType);
            default:
                return Array.Empty<object>();
        }
    }

    private static Component GetComponent(MonoBehaviour component, Type fieldType, FindIn source)
    {
        switch (source)
        {
            case FindIn.Self:
                return component.GetComponent(fieldType);
            case FindIn.Parent:
                return component.GetComponentInParent(fieldType);
            case FindIn.Children:
                return component.GetComponentInChildren(fieldType);
            default:
                return null;
        }
    }

    private static void LogError(MonoBehaviour component, Type type, FindIn source)
    {
        string message = $"{component} requires {type}";
        switch (source)
        {
            case FindIn.Self:
                message += $" on {component.transform}";
                break;
            case FindIn.Parent:
                message += $" in parent of {component.transform}";
                break;
            case FindIn.Children:
                message += $" in children of {component.transform}";
                break;
        }

        Debug.LogError(message, component);
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class AutoReferenceAttribute : Attribute
{
    public FindIn Source;
    public bool Required;

    public AutoReferenceAttribute(FindIn source = FindIn.Self, bool required = true)
    {
        Source = source;
        Required = required;
    }
}

public enum FindIn
{
    Self,
    Parent,
    Children,
}