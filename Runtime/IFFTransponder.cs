using UnityEngine;

/**
 * 注意 iff 对象可能不仅限与 unit 之间。projectile 也可能参与 iff 判断
 * 也就是 iff 不一定跟着 unit
 */
public class IFFTransponder : MonoBehaviour
{
    public string Identity;

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

    // public bool IsFoe(Unit otherUnit)
    // {
    //     // neutral if not set identity
    //     if (string.IsNullOrEmpty(Identity))
    //     {
    //         return false;
    //     }
    //
    //     // neutral if other not response
    //     if (!otherUnit
    //         || !otherUnit.TryGetComponent(out IFFTransponder otherTransponder)
    //         || string.IsNullOrEmpty(otherTransponder.Identity))
    //     {
    //         return false;
    //     }
    //
    //     return Identity == otherTransponder.Identity;
    // }
}