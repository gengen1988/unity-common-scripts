using UnityEngine;

public class ActorInputMockup : MonoBehaviour
{
    private ActorOld _actorOld;

    private void OnEnable()
    {
        _actorOld = GetComponentInParent<ActorOld>();
        _actorOld.OnPerceive += HandlePerceive;
    }

    private void OnDisable()
    {
        _actorOld.OnPerceive -= HandlePerceive;
    }

    private void HandlePerceive(ActorOld actorOld)
    {
        actorOld.Intent.SetVector2(IntentKey.Move, Vector2.right);
    }
}