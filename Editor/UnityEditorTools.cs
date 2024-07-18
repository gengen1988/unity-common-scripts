using System.IO;
using UnityEditor;
using UnityEngine;

public static class UnityEditorTools
{
    [MenuItem("Assets / Tools / Set Selection Dirty")]
    [MenuItem("GameObject / Tools / Set Selection Dirty")]
    private static void SetSelectionDirty()
    {
        foreach (Object go in Selection.objects)
        {
            EditorUtility.SetDirty(go);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets / Tools / Mark as Obsolete")]
    public static void MarkAsObsolete()
    {
        string OBSOLETE_DIRECTORY = "Assets/_Obsolete";
        if (!Directory.Exists(OBSOLETE_DIRECTORY))
        {
            Directory.CreateDirectory(OBSOLETE_DIRECTORY);
        }

        foreach (Object obj in Selection.objects)
        {
            if (!AssetDatabase.Contains(obj))
            {
                continue;
            }

            string oldPath = AssetDatabase.GetAssetPath(obj);
            string fileName = Path.GetFileName(oldPath);
            string newPath = Path.Combine(OBSOLETE_DIRECTORY, fileName) + ".bak";
            Debug.Log($"move {oldPath} to {newPath}");
            AssetDatabase.MoveAsset(oldPath, newPath);
        }
    }
}