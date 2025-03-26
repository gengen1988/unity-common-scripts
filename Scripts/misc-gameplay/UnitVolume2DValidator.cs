#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;

[assembly: RegisterValidationRule(typeof(UnitVolume2DValidator), Name = "Unit Volume Validator", Description = "validate unit's volume")]
public class UnitVolume2DValidator : RootObjectValidator<Unit>
{
    protected override void Validate(ValidationResult result)
    {
        var actor = Object;
        var volumeLayer = CustomLayer.Default;

        if (!actor.TryGetComponent(out Collider2D collider))
        {
            result.AddError("Unit missing its volume collider.")
                .WithFix(() =>
                {
                    var newCollider = actor.gameObject.AddComponent<BoxCollider2D>();
                    newCollider.isTrigger = true;
                });
        }
        else if (!collider.isTrigger)
        {
            result.AddError("Volume collider should be trigger.")
                .WithFix(() => collider.isTrigger = true);
        }

        if (actor.gameObject.layer != volumeLayer)
        {
            result.AddError($"Volume layer should be {LayerUtil.GetLayerName(volumeLayer)}.");
        }

        var mismatchColliders = actor.GetComponentsInChildren<Collider2D>()
            .Where(col => col.transform != actor.transform)
            .Where(col => col.gameObject.layer == volumeLayer)
            .ToArray();

        foreach (var childrenCollider in mismatchColliders)
        {
            result.AddError($"Collider '{childrenCollider.gameObject.name}' is mismatched.");
        }
    }
}
#endif