using System;
using UnityEngine;

[Obsolete]
public class BrainPerceiveHurt : MonoBehaviour
{
    // private Hurtable _hurtable;
    // private Brain _brain;
    //
    // private void Awake()
    // {
    //     _brain = GetComponentInParent<Brain>();
    //     _brain.TryGetComponent(out _hurtable);
    // }
    //
    // private void OnEnable()
    // {
    //     _hurtable.OnHurt += HandleHurt;
    // }
    //
    // private void OnDisable()
    // {
    //     _hurtable.OnHurt -= HandleHurt;
    // }
    //
    // private void HandleHurt(HitInfo obj)
    // {
    //     var hitSubject = obj.HitEntity.GetBridge().GetComponentInParent<Unit>();
    //     // --- set blackboard
    //     // _brain.
    //     // _whoHitMe.Add(hitSubject);
    // }
}