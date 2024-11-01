using System;
using UnityEngine;

/**
 * 注意 iff 对象可能不仅限与 unit 之间。projectile 也可能参与 iff 判断
 * 也就是 iff 不一定跟着 unit
 * 另外，iff 不能设置在子 Transform 上，需要和 rigidbody 在一个对象上
 */
public class IFFTransponder : MonoBehaviour
{
    public string Identity;

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(Identity))
        {
            Identity = Identity.Trim();
        }
    }

    [Obsolete]
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

    [Obsolete]
    public bool FilterByFriend(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        rb.TryGetComponent(out IFFTransponder otherTransponder);
        return !IsFoe(otherTransponder);
    }

    [Obsolete]
    public bool FilterByFoe(Collider2D col)
    {
        Rigidbody2D rb = col.attachedRigidbody;
        rb.TryGetComponent(out IFFTransponder otherTransponder);
        return IsFoe(otherTransponder);
    }

    public static void CopyIdentity(GameObject srcObject, GameObject destObject)
    {
        if (srcObject.TryGetComponent(out IFFTransponder src) && destObject.TryGetComponent(out IFFTransponder dest))
        {
            dest.Identity = src.Identity;
        }
    }

    public static void CopyIdentity(Component srcObject, Component destObject)
    {
        if (srcObject.TryGetComponent(out IFFTransponder src) && destObject.TryGetComponent(out IFFTransponder dest))
        {
            dest.Identity = src.Identity;
        }
    }

    public static bool IsFriend(Component a, Component b)
    {
        return IsFriend(a.gameObject, b.gameObject);
    }

    public static bool IsFoe(Component a, Component b)
    {
        return IsFoe(a.gameObject, b.gameObject);
    }

    public static bool IsFriend(GameObject a, GameObject b)
    {
        string id1 = GetIdentity(a);
        string id2 = GetIdentity(b);
        if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2))
        {
            return false;
        }

        return id1 == id2;
    }

    public static bool IsFoe(GameObject a, GameObject b)
    {
        string id1 = GetIdentity(a);
        string id2 = GetIdentity(b);
        if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2))
        {
            return false;
        }

        return id1 != id2;
    }

    private static string GetIdentity(GameObject src)
    {
        return src.TryGetComponent(out IFFTransponder transponder) ? transponder.Identity : null;
    }
}