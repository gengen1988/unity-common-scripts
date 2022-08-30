using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AssetDatabaseUtil
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
		Debug.Log($"move {oldPath} to {newPath}");
		AssetDatabase.MoveAsset(oldPath, newPath);
	}

	/**
	 * Create folders recursively
	 */
	public static void Mkdirp(string path)
	{
		var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		var segments = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		var parent = "";

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
		var directoryPath = Path.GetDirectoryName(path);
		Mkdirp(directoryPath);
		AssetDatabase.CreateAsset(asset, path);
	}
}