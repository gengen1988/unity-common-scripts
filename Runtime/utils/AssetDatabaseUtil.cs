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
		var filename = Path.GetFileName(oldPath);
		var newPath = Path.Combine(targetFolder, filename);
		Debug.Log($"move {oldPath} to {newPath}");
		AssetDatabase.MoveAsset(oldPath, newPath);
	}

	/**
	 * create folder recursively
	 */
	public static void Mkdirp()
	{
		throw new NotImplementedException();
	}
}