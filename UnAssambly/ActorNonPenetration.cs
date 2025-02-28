using UnityEngine;

public class ActorNonPenetration : MonoBehaviour
{
    [SerializeField] private float OverlapRadius = 0.5f;
    [SerializeField] private bool AllowPenetrateSetting;

    private Vector2 _placementPosition;
    private Vector2 _positionCorrection;
    private Rigidbody2D _rb;

    public float Radius => OverlapRadius;
    public Vector2 NextPlacementPosition => _placementPosition;
    public bool AllowPenetrate => AllowPenetrateSetting;

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out Actor actor);
        actor.OnPerceive += HandlePerceive;
    }

    private void HandlePerceive()
    {
        // prepare
        _placementPosition = transform.position;
        _positionCorrection = Vector2.zero;
    }

    /**
     * use this method instead of Rigidbody2D.MovePosition
     */
    public void MovePosition(Vector2 position)
    {
        _placementPosition = position;
    }

    public void AddCorrection(Vector2 correction)
    {
        _positionCorrection += correction;
    }

    public void Commit()
    {
        var toBeApply = _placementPosition + _positionCorrection;
        if (_rb)
        {
            _rb.MovePosition(toBeApply);
        }
        else
        {
            transform.position = toBeApply;
        }
    }
}