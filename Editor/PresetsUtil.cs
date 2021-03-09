using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;


public static class PresetsUtil
{
    const string TAG_MANAGER_PRESETS_PATH = "Assets/unity-common-scripts/Presets/TagManager.preset";
    const string PHYSICS_2D_SETTINGS_PRESETS_PATH = "Assets/unity-common-scripts/Presets/Physics2DSettings.preset";

    const string TAG_MANAGER_PATH = "ProjectSettings/TagManager.asset";
    const string PHYSICS_2D_SETTINGS_PATH = "ProjectSettings/Physics2DSettings.asset";

    [MenuItem("Tools / My Project / Load Tag and Physics Presets")]
    static void LoadPresets()
    {
        var tagPresets = AssetDatabase.LoadAssetAtPath<Preset>(TAG_MANAGER_PRESETS_PATH);
        var tagManager = AssetDatabase.LoadAssetAtPath<Object>(TAG_MANAGER_PATH);

        var physics2DPresets = AssetDatabase.LoadAssetAtPath<Preset>(PHYSICS_2D_SETTINGS_PRESETS_PATH);
        var physics2DSettings = AssetDatabase.LoadAssetAtPath<Object>(PHYSICS_2D_SETTINGS_PATH);

        tagPresets.ApplyTo(tagManager);
        physics2DPresets.ApplyTo(physics2DSettings);

        Debug.Log($"presets loaded");
    }
}