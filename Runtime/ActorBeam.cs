using UnityEngine;

/**
 * beam and field should share some common: they all belongs to one actor
 * shooter should pass on and off signals to shot
 * beam is different from flame thrower, beam rotate affect previous shots, and flame not
 */
public class ActorBeam : MonoBehaviour
{
    private void OnEnable()
    {
        var hitManager = GetComponentInParent<ActorHitManager>();
        hitManager.OnHit += HandleHit;
    }

    private void HandleHit(Actor hitSubject, Actor hurtSubject, HitEventData evtData)
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(Actor moveSubject, float localDeltaTime)
    {
        throw new System.NotImplementedException();
    }
}