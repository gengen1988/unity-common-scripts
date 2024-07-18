#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

public static class AssetUtil
{
    public static void MoveAssetToFolder(Object asset, string targetFolder)
    {
        if (!AssetDatabase.Contains(asset))
        {
            Debug.LogError($"{asset} is not asset");
            return;
        }

        string oldPath = AssetDatabase.GetAssetPath(asset);
        string fileName = Path.GetFileName(oldPath);
        string newPath = Path.Combine(targetFolder, fileName);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        Debug.Log($"move {oldPath} to {newPath}");
        AssetDatabase.MoveAsset(oldPath, newPath);
    }

    /**
     * Create folders recursively.
     * Deprecated, use Directory.CreateDirectory instead.
     */
    [Obsolete]
    public static void Mkdirp(string path)
    {
        char[] separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        string[] segments = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        string parent = "";

        foreach (string segment in segments)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parent, segment)))
            {
                AssetDatabase.CreateFolder(parent, segment);
            }

            parent = Path.Combine(parent, segment);
        }
    }

    /**
     * Create a new asset at path. Also create missing folders.
     */
    public static void CreateAsset(Object asset, string path)
    {
        string directoryPath = Path.GetDirectoryName(path);
        Assert.IsNotNull(directoryPath, $"given path is not valid: {path}");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        AssetDatabase.CreateAsset(asset, path);
    }

    public static T FindAndLoad<T>(string filter, string[] searchInFolder = null) where T : Object
    {
        return FindAndLoadAll<T>(filter, searchInFolder).First();
    }

    public static IEnumerable<T> FindAndLoadAll<T>(string filter, string[] searchInFolder = null) where T : Object
    {
        string[] result = searchInFolder == null
            ? AssetDatabase.FindAssets(filter)
            : AssetDatabase.FindAssets(filter, searchInFolder);

        return result
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>);
    }
}
#endif