using UnityEngine;

/**
 * 起到的作用是一个独立的作用域，用于查找相关组件。除此以外不应有其他副作用
 */
public class Actor : MonoBehaviour
{
    public static T GetComponentInActor<T>(GameObject from) where T : Component
    {
        Actor actor = from.GetComponentInParent<Actor>();
        if (!actor)
        {
            return null;
        }

        T result = actor.GetComponentInChildren<T>();
        return result;
    }
}