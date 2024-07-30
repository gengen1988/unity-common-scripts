using UnityEngine;

/**
 * 注意 iff 对象可能不仅限与 unit 之间。projectile 也可能参与 iff 判断
 * 也就是 iff 不一定跟着 unit
 */
public class IFFTransponder : MonoBehaviour
{
    public string Identity;

    // public bool IsFoe()
    // {
    // }
    //
    // public bool IsFriend()
    // {
    // }
    //
    // public bool IsNotFoe()
    // {
    // }
    //
    // public bool IsNotFriend()
    // {
    // }
    //
    // public bool IsNeutral()
    // {
    // }

    public bool IsFoe(IFFTransponder otherTransponder)
    {
        // neutral if not set identity
        if (string.IsNullOrEmpty(Identity) || !otherTransponder || string.IsNullOrEmpty(otherTransponder.Identity))
        {
            return false;
        }

        // neutral if other not response
        if (!otherTransponder || string.IsNullOrEmpty(otherTransponder.Identity))
        {
            return false;
        }

        return Identity != otherTransponder.Identity;
    }

    public bool FilterByFriend(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        rb.TryGetComponent(out IFFTransponder otherTransponder);
        return !IsFoe(otherTransponder);
    }

    public bool FilterByFoe(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        rb.TryGetComponent(out IFFTransponder otherTransponder);
        return IsFoe(otherTransponder);
    }
}