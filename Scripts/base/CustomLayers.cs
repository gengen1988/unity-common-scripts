using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public struct CustomLayer
{
    public const int Default = 0; // used as physics query volume
    public const int Obstacle = 8;
    public const int Platform = 9;
    public const int Hitbox = 10;
    public const int Hurtbox = 11;
    public const int Collectable = 12;
}

public static class LayerUtil
{
    public static LayerMask LayerToMask(params int[] layer)
    {
        var mask = 0;
        foreach (var l in layer)
        {
            mask |= 1 << l;
        }

        return mask;
    }

    public static string GetLayerName(int layer)
    {
        return LayerMask.LayerToName(layer);
    }

#if UNITY_EDITOR
    [MenuItem("Tools / Create Layers")]
    private static void WriteCustomLayerNameToUnity()
    {
        // load tag manager
        var asset = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/TagManager.asset");
        using var tagManager = new SerializedObject(asset);
        var layersProp = tagManager.FindProperty("layers");

        // Get all the custom layer values using reflection to obtain constant fields
        var customLayers = typeof(CustomLayer)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => new
            {
                fi.Name,
                Value = (int)fi.GetValue(null)
            });

        // For each custom layer, set its name in Unity's layer settings
        foreach (var layer in customLayers)
        {
            var layerIndex = layer.Value;
            var layerName = layer.Name;

            // Check if the layer index is valid (Unity supports 8-31 layers)
            if (layerIndex >= 8 && layerIndex <= 31)
            {
                // Only set the name if the layer slot is empty or already has our name
                var layerProp = layersProp.GetArrayElementAtIndex(layerIndex);
                if (string.IsNullOrEmpty(layerProp.stringValue) || layerProp.stringValue == layerName)
                {
                    layerProp.stringValue = layerName;
                    Debug.Log($"Setting layer {layerIndex} to {layerName}");
                }
                else
                {
                    Debug.LogWarning($"Layer {layerIndex} already has name '{layerProp.stringValue}', not overwriting with '{layerName}'");
                }
            }
            else
            {
                Debug.LogError($"Layer index {layerIndex} for {layerName} is out of range (8-31)");
            }
        }

        // apply modifications
        tagManager.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}