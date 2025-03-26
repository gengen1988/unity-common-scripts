using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorTools
{
    [MenuItem("Assets / Tools / Re-serialize Selection")]
    [MenuItem("GameObject / Tools / Re-serialize Selection")]
    private static void ReserializeSelection()
    {
        foreach (var go in Selection.objects)
        {
            EditorUtility.SetDirty(go);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets / Tools / Mark as Obsolete")]
    private static void MarkAsObsolete()
    {
        const string OBSOLETE_DIRECTORY = "Assets/_Obsolete";

        if (!Directory.Exists(OBSOLETE_DIRECTORY))
        {
            Directory.CreateDirectory(OBSOLETE_DIRECTORY);
        }

        foreach (var obj in Selection.objects)
        {
            if (!AssetDatabase.Contains(obj))
            {
                continue;
            }

            var oldPath = AssetDatabase.GetAssetPath(obj);
            var fileName = Path.GetFileName(oldPath);
            var newPath = Path.Combine(OBSOLETE_DIRECTORY, fileName) + ".bak";
            Debug.Log($"move {oldPath} to {newPath}");
            AssetDatabase.MoveAsset(oldPath, newPath);
        }

        AssetDatabase.Refresh();
    }
}