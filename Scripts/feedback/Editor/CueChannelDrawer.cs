using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CueChannel))]
public class CueChannelDrawer : PropertyDrawer
{
    private const float BUTTON_SPACING = 5f;
    private const float FIND_BUTTON_WIDTH = 75f;
    private const float CREATE_BUTTON_WIDTH = 55f;
    private const float EXISTING_VALUE_BUTTON_SPACE = 80f;
    private const float NO_VALUE_BUTTON_SPACE = 60f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var isMonoBehaviour = property.serializedObject.targetObject is MonoBehaviour;
        if (!isMonoBehaviour)
        {
            // For non-MonoBehaviour objects (like ScriptableObjects), use the default property drawer
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // Draw the object field
        var hasValue = property.objectReferenceValue != null;
        Rect propertyRect = AdjustPropertyRect(position, hasValue);
        EditorGUI.PropertyField(propertyRect, property, label);

        // Draw the appropriate button based on whether we have a value
        Rect buttonRect = GetButtonRect(position, propertyRect);
        if (hasValue)
        {
            HandleExistingValue(buttonRect, property);
        }
        else
        {
            HandleNoValue(buttonRect, property);
        }

        EditorGUI.EndProperty();
    }

    private Rect AdjustPropertyRect(Rect position, bool hasValue)
    {
        Rect propertyRect = position;
        if (hasValue)
        {
            propertyRect.width -= EXISTING_VALUE_BUTTON_SPACE; // Make room for the Find Feedback button
        }
        else
        {
            propertyRect.width -= NO_VALUE_BUTTON_SPACE;
        }

        return propertyRect;
    }

    private Rect GetButtonRect(Rect position, Rect propertyRect)
    {
        Rect buttonRect = position;
        buttonRect.x = propertyRect.x + propertyRect.width + BUTTON_SPACING;
        buttonRect.width = position.width - propertyRect.width - BUTTON_SPACING;
        return buttonRect;
    }

    private void HandleExistingValue(Rect position, SerializedProperty property)
    {
        position.width = FIND_BUTTON_WIDTH;
        var cueChannel = property.objectReferenceValue as CueChannel;
        var feedbackLibrary = FeedbackLibrarySettings.GetProjectWideFeedbackLibrary();

        if (feedbackLibrary == null)
        {
            HandleNoLibraryCase(position);
        }
        else
        {
            var feedback = feedbackLibrary.FindFeedbackByChannel(cueChannel);
            if (feedback == null)
            {
                HandleNoFeedbackCase(position, cueChannel, feedbackLibrary);
            }
            else
            {
                HandleExistingFeedbackCase(position, feedback);
            }
        }
    }

    private void HandleNoLibraryCase(Rect position)
    {
        // Case 1: No project-wide library
        GUI.contentColor = Color.yellow;
        if (GUI.Button(position, "No Library"))
        {
            EditorUtility.DisplayDialog(
                "Feedback Library Not Set",
                "Please set a Feedback Library in the Project Settings.",
                "OK"
            );
            // Open the settings window
            SettingsService.OpenProjectSettings("Project/Feedback Library");
        }

        GUI.contentColor = Color.white;
    }

    private void HandleNoFeedbackCase(Rect position, CueChannel cueChannel, FeedbackLibrary feedbackLibrary)
    {
        // Case 2: No feedback found
        if (GUI.Button(position, "Create"))
        {
            CreateFeedbackPrefab(cueChannel, feedbackLibrary);
        }
    }

    private void HandleExistingFeedbackCase(Rect position, Feedback feedback)
    {
        // Case 3: Has feedback assigned
        if (GUI.Button(position, "Find"))
        {
            EditorGUIUtility.PingObject(feedback);
        }
    }

    private void HandleNoValue(Rect position, SerializedProperty property)
    {
        position.width = CREATE_BUTTON_WIDTH;
        if (GUI.Button(position, "Create"))
        {
            CreateCueChannel(property);
        }
    }

    private void CreateFeedbackPrefab(CueChannel cueChannel, FeedbackLibrary feedbackLibrary)
    {
        // Get the base path for feedback prefabs
        var basePath = FeedbackLibrarySettings.FeedbackPath;

        // Ensure the directory exists
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        // Create a new feedback prefab
        var feedbackName = $"Feedback_{cueChannel.name.Substring(cueChannel.name.IndexOf('_') + 1)}";
        var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{basePath}/{feedbackName}.prefab");

        // Create a new GameObject with a Feedback component
        var go = new GameObject(feedbackName);
        go.AddComponent<Feedback>();

        // Create the prefab asset
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, uniquePath);

        // Destroy the temporary GameObject
        Object.DestroyImmediate(go);

        // Register the feedback in the library
        feedbackLibrary.SetupEntry(cueChannel, prefab.GetComponent<Feedback>());

        // Save the library
        EditorUtility.SetDirty(feedbackLibrary);
        AssetDatabase.SaveAssets();

        // Ping the new feedback prefab
        EditorGUIUtility.PingObject(prefab);

        // Show a success message
        Debug.Log($"Created and registered feedback prefab for {cueChannel.name}");
    }

    private void CreateCueChannel(SerializedProperty property)
    {
        // Create a new CueChannel asset
        var newCueChannel = ScriptableObject.CreateInstance<CueChannel>();

        // Generate a unique path in the Assets folder
        var assetName = "Cue_NewCue";
        var basePath = FeedbackLibrarySettings.CueChannelPath;

        // Ensure the directory exists
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        // Create a unique filename
        var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{basePath}/{assetName}.asset");

        // Create the asset
        AssetDatabase.CreateAsset(newCueChannel, uniquePath);
        AssetDatabase.SaveAssets();

        // Assign the new asset to the property
        property.objectReferenceValue = newCueChannel;
        property.serializedObject.ApplyModifiedProperties();

        // Register the new CueChannel in the project-wide FeedbackLibrary
        var feedbackLibrary = FeedbackLibrarySettings.GetProjectWideFeedbackLibrary();
        if (feedbackLibrary != null)
        {
            // Register with null feedback to create the relationship
            feedbackLibrary.SetupEntry(newCueChannel, null);
            Debug.Log($"Registered {newCueChannel.name} in the Feedback Library");
        }

        // Ping the new asset in the Project window
        EditorGUIUtility.PingObject(newCueChannel);
    }
}