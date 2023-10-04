using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Decompose2 : MonoBehaviour
{
    public float DestroyDelay;

    private GameObject _parent;
    private ParentConstraint _constraint;

    private void OnEnable()
    {
        _parent = transform.parent.gameObject;

        if (!TryGetComponent(out _constraint))
        {
            _constraint = gameObject.AddComponent<ParentConstraint>();
        }

        _constraint.SetSources(new List<ConstraintSource>
        {
            new ConstraintSource { sourceTransform = _parent.transform, weight = 1 }
        });

        _constraint.SetTranslationOffset(0, transform.localPosition);
        _constraint.SetRotationOffset(0, transform.localRotation.eulerAngles);
        _constraint.locked = true;
        _constraint.constraintActive = true;

        transform.SetParent(null);
    }

    private void Update()
    {
        if (!_parent)
        {
            _constraint.constraintActive = false;
            Destroy(gameObject, DestroyDelay);
            enabled = false;
        }
    }
}