using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityProxy))]
public class PickupCollector : MonoBehaviour, IEntityFrame
{
    [SerializeField] private float attractRange = 2f;
    [SerializeField] private Vector2 offset;

    private readonly List<Collider2D> _queryBuffer = new();
    public Vector2 CenterPosition => (Vector2)transform.position + offset;

    private void OnDrawGizmosSelected()
    {
        // Draw the attraction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CenterPosition, attractRange);
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        // newly add to attract
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = 1 << CustomLayer.Collectable,
        };
        Physics2D.OverlapCircle(CenterPosition, attractRange, filter, _queryBuffer);
        foreach (var found in _queryBuffer)
        {
            var rootTrans = found.GetAttachedTransform();
            if (!rootTrans.TryGetComponent(out Pickup pickup))
            {
                continue;
            }

            pickup.AttractBy(this);
        }
    }
}