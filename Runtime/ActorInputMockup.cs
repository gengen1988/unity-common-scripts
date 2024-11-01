using UnityEngine;

public class ActorInputMockup : MonoBehaviour
{
    private Actor _actor;

    private void OnEnable()
    {
        _actor = GetComponentInParent<Actor>();
        _actor.OnPerceive += HandlePerceive;
    }

    private void OnDisable()
    {
        _actor.OnPerceive -= HandlePerceive;
    }

    private void HandlePerceive(Actor actor)
    {
        actor.Intent.SetVector2(IntentKey.Move, Vector2.right);
    }
}