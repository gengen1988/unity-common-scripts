using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const string LAYER_NAME = "Unit";

    [Button]
    private void CheckComponents()
    {
        gameObject.layer = LayerMask.NameToLayer(LAYER_NAME);
        Rigidbody2D rb = null;
        BoxCollider2D col = null;
        gameObject.EnsureComponent(ref rb, true);
        gameObject.EnsureComponent(ref col, true);
        col.isTrigger = true;
#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    public static ContactFilter2D ContactFilter => new ContactFilter2D
    {
        useTriggers = true,
        layerMask = LayerMask.GetMask(LAYER_NAME),
    };
}