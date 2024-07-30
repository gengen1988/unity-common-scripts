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
        Rigidbody2D rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        BoxCollider2D col = gameObject.GetOrAddComponent<BoxCollider2D>();
        col.isTrigger = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        gameObject.GetOrAddComponent<IFFTransponder>();
        gameObject.GetOrAddComponent<HurtSubject>();
        gameObject.GetOrAddComponent<HealthSubject>();


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