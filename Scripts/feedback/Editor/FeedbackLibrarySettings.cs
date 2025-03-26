using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FeedbackLibrarySettings
{
    private const string DefaultCueChannelPath = "Assets/_Game/Database/Cues";
    private const string DefaultFeedbackPath = "Assets/_Game/Prefabs";
    private const string DefaultFeedbackLibraryGUID = "";

    private static string cueChannelPath;
    private static string feedbackPath;
    private static string feedbackLibraryGUID;

    public static string CueChannelPath
    {
        get
        {
            if (string.IsNullOrEmpty(cueChannelPath))
            {
                cueChannelPath = EditorPrefs.GetString("FeedbackLibrary_CueChannelPath", DefaultCueChannelPath);
            }

            return cueChannelPath;
        }
        set
        {
            cueChannelPath = value;
            EditorPrefs.SetString("FeedbackLibrary_CueChannelPath", value);
        }
    }

    public static string FeedbackPath
    {
        get
        {
            if (string.IsNullOrEmpty(feedbackPath))
            {
                feedbackPath = EditorPrefs.GetString("FeedbackLibrary_FeedbackPath", DefaultFeedbackPath);
            }

            return feedbackPath;
        }
        set
        {
            feedbackPath = value;
            EditorPrefs.SetString("FeedbackLibrary_FeedbackPath", value);
        }
    }

    public static FeedbackLibrary FeedbackLibrary
    {
        get
        {
            if (string.IsNullOrEmpty(feedbackLibraryGUID))
            {
                feedbackLibraryGUID = EditorPrefs.GetString("FeedbackLibrary_LibraryGUID", DefaultFeedbackLibraryGUID);
            }

            if (!string.IsNullOrEmpty(feedbackLibraryGUID))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(feedbackLibraryGUID);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    return AssetDatabase.LoadAssetAtPath<FeedbackLibrary>(assetPath);
                }
            }

            return null;
        }
        set
        {
            if (value != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(value);
                feedbackLibraryGUID = AssetDatabase.AssetPathToGUID(assetPath);
                EditorPrefs.SetString("FeedbackLibrary_LibraryGUID", feedbackLibraryGUID);
            }
            else
            {
                feedbackLibraryGUID = "";
                EditorPrefs.SetString("FeedbackLibrary_LibraryGUID", "");
            }
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Feedback Library", SettingsScope.Project)
        {
            label = "Feedback Library",
            guiHandler = _ =>
            {
                EditorGUILayout.Space(10);

                EditorGUI.BeginChangeCheck();

                var newCueChannelPath = EditorGUILayout.TextField("Cue Channel Path", CueChannelPath);
                var newFeedbackPath = EditorGUILayout.TextField("Feedback Path", FeedbackPath);
                var newFeedbackLibrary = EditorGUILayout.ObjectField("Project-wide Library", FeedbackLibrary, typeof(FeedbackLibrary), false);

                if (EditorGUI.EndChangeCheck())
                {
                    CueChannelPath = newCueChannelPath;
                    FeedbackPath = newFeedbackPath;
                    FeedbackLibrary = newFeedbackLibrary as FeedbackLibrary;
                }

                EditorGUILayout.Space(10);
                if (GUILayout.Button("Reset to Defaults"))
                {
                    CueChannelPath = DefaultCueChannelPath;
                    FeedbackPath = DefaultFeedbackPath;
                    FeedbackLibrary = null;
                }
            },
            keywords = new HashSet<string>(new[] { "Feedback", "Cue", "Channel", "Path", "Library" })
        };

        return provider;
    }

    /// <summary>
    /// Gets the project-wide feedback library.
    /// </summary>
    /// <returns>The project-wide feedback library, or null if not set.</returns>
    public static FeedbackLibrary GetProjectWideFeedbackLibrary()
    {
        return FeedbackLibrary;
    }
}