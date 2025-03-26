using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ModifierProfile : ScriptableObject
{
    public Modifier Prefab;
    public float DefaultLifeTime = -1f;
    public int StackCapacity = 1;

#if UNITY_EDITOR
    [Button]
    private void FindPrefab()
    {
        var prefab = AuthoringUtil.FindAndLoad<GameObject>($"{name} t:prefab");
        if (prefab)
        {
            // Ping the prefab in the Project window to highlight it
            prefab.TryGetComponent(out Prefab);
            EditorGUIUtility.PingObject(prefab);
            EditorUtility.SetDirty(this);
        }
        else
        {
            Debug.LogAssertion("Prefab was not found.", this);
        }
    }

    [Button]
    private void CreatePrefab()
    {
        var prefabName = name;
        var go = new GameObject(prefabName);
        go.AddComponent<Modifier>();

        // Get the asset path of this profile
        var assetPath = AssetDatabase.GetAssetPath(this);
        Debug.Log($"Creating buff prefab for profile at: {assetPath}");

        // Get the directory of the profile to store the prefab in the same folder
        var profileDirectory = Path.GetDirectoryName(assetPath);
        var prefabPath = $"{profileDirectory}/{prefabName}.prefab";

        // Create the prefab and save it
        var prefabAsset = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        DestroyImmediate(go); // Clean up the temporary GameObject

        // Assign the Buff component from the prefab to this profile
        Prefab = prefabAsset.GetComponent<Modifier>();

        // Ping the newly created prefab in the Project window to highlight it
        EditorUtility.SetDirty(this);
        EditorGUIUtility.PingObject(prefabAsset);
        Debug.Log($"Created and assigned buff prefab: {prefabPath}");
    }

    [Button]
    private void DebugAdd(ModifierManager subject)
    {
        if (!subject)
        {
            Debug.LogWarning("please assign a buff manager on inspector to test the buff.");
            return;
        }

        Debug.Log($"add {this} to {subject}");
        subject.AddModifier(this);
    }
#endif
}

public enum ModifierState
{
    Born,
    Alive,
    Dead,
}