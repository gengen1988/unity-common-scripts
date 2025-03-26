using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * - 当前武器
 * - 切换武器
 */
[Obsolete]
public class ActorWeaponManager : MonoBehaviour
{
    // [SerializeField] private Weapon initialWeapon;
    // [SerializeField] private Weapon internalWeapon;
    //
    // private Actor _actor;
    // private Weapon _primaryWeapon;
    //
    // private readonly List<Weapon> _weapons = new();
    //
    // private void Awake()
    // {
    //     _actor = this.EnsureComponent<Actor>();
    //     _actor.OnMove += HandleActorMove;
    //     // debug gameplay
    //     // ChangePrimaryWeapon(initialWeapon);
    // }
    //
    // private void Start()
    // {
    //     SetWeaponAsPrimary(internalWeapon);
    // }
    //
    // private void SetWeaponAsPrimary(Weapon weapon)
    // {
    //     weapon.Mount(_actor.gameObject);
    //     _primaryWeapon = weapon;
    //     _weapons.Add(weapon);
    // }
    //
    // public void ChangePrimaryWeapon(Weapon prefab)
    // {
    //     if (_primaryWeapon)
    //     {
    //         _weapons.Remove(_primaryWeapon);
    //         _primaryWeapon.Unmount();
    //         Destroy(_primaryWeapon.gameObject);
    //         _primaryWeapon = null;
    //     }
    //
    //     if (prefab)
    //     {
    //         var weapon = Instantiate(prefab, transform);
    //         weapon.Mount(_actor.gameObject);
    //         _primaryWeapon = weapon;
    //         _weapons.Add(weapon);
    //     }
    // }
    //
    // private void HandleActorMove()
    // {
    //     foreach (var weapon in _weapons)
    //     {
    //         weapon.Tick(_actor.Clock.LocalDeltaTime);
    //     }
    // }
    //
    // public Weapon GetPrimaryWeapon()
    // {
    //     return _primaryWeapon;
    // }
}