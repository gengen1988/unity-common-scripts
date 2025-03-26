using System;
using UnityEngine;

[Obsolete]
public class ActorWeaponManager2 : MonoBehaviour
{
    // private Actor _actor;
    // private readonly Dictionary<Transform, Weapon> _weaponBySocket = new();
    //
    // private void Awake()
    // {
    //     _actor = this.EnsureComponent<Actor>();
    //     _actor.OnMove += HandleActorMove;
    // }
    //
    // private void HandleActorMove()
    // {
    //     // tick weapons
    //     foreach (var weapon in _weaponBySocket.Values)
    //     {
    //         weapon.Tick(_actor.Clock.LocalDeltaTime);
    //     }
    // }
    //
    // public Weapon ChangeWeapon(Transform socketTrans, Weapon prefab = null)
    // {
    //     Debug.Assert(socketTrans, this);
    //
    //     var currentWeapon = _weaponBySocket.GetValueOrDefault(socketTrans);
    //     if (currentWeapon)
    //     {
    //         currentWeapon.Unmount();
    //         Destroy(currentWeapon.gameObject);
    //         _weaponBySocket.Remove(socketTrans);
    //     }
    //
    //     if (prefab)
    //     {
    //         var weapon = Instantiate(prefab, socketTrans);
    //         weapon.Mount(_actor.gameObject);
    //         _weaponBySocket[socketTrans] = weapon;
    //         return weapon;
    //     }
    //
    //     return null;
    // }
}