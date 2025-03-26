#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

public static class AuthoringUtil
{
    public static void MoveAssetToFolder(Object asset, string targetFolder)
    {
        if (!AssetDatabase.Contains(asset))
        {
            Debug.LogError($"{asset} is not asset");
            return;
        }

        var oldPath = AssetDatabase.GetAssetPath(asset);
        var fileName = Path.GetFileName(oldPath);
        var newPath = Path.Combine(targetFolder, fileName);

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
        var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        var segments = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        var parent = "";

        foreach (var segment in segments)
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
        var directoryPath = Path.GetDirectoryName(path);
        Assert.IsNotNull(directoryPath, $"given path is not valid: {path}");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        AssetDatabase.CreateAsset(asset, path);
    }

    public static T FindAndLoad<T>(string filter, string[] searchInFolder = null) where T : Object
    {
        return FindAndLoadAll<T>(filter, searchInFolder).FirstOrDefault();
    }

    public static IEnumerable<T> FindAndLoadAll<T>(string filter, string[] searchInFolder = null) where T : Object
    {
        var result = searchInFolder == null
            ? AssetDatabase.FindAssets(filter)
            : AssetDatabase.FindAssets(filter, searchInFolder);

        return result
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>);
    }
}
#endif