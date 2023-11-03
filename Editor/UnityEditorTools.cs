using UnityEditor;

public static class UnityEditorTools
{
	[MenuItem("Assets / Tools / Set Selection Dirty")]
	[MenuItem("GameObject / Tools / Set Selection Dirty")]
	private static void SetSelectionDirty()
	{
		foreach (var go in Selection.objects)
		{
			EditorUtility.SetDirty(go);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}