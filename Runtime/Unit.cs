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
        Rigidbody2D rb = gameObject.EnsureComponent<Rigidbody2D>();
        BoxCollider2D col = gameObject.EnsureComponent<BoxCollider2D>();
        col.isTrigger = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        gameObject.EnsureComponent<IFFTransponder>();
        gameObject.EnsureComponent<HurtSubject>();
        gameObject.EnsureComponent<HealthSubject>();


#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    public static ContactFilter2D ContactFilter => new ContactFilter2D
    {
        useTriggers = true,
        useLayerMask = true,
        layerMask = LayerMask.GetMask(LAYER_NAME),
    };
}