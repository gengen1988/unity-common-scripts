using System;
using UnityEngine;

[Obsolete]
public class PrimaryWeaponMounter : MonoBehaviour
{
    // public event Action<Weapon> OnPrimaryWeaponChanged;
    //
    // [SerializeField] private Weapon primaryWeaponPrefab;
    // [SerializeField] private string primaryWeaponSocketName;
    // [SerializeField] private float shootTolerance = 5f;
    //
    // private Weapon _primaryWeapon;
    // private Actor _actor;
    //
    // private void Awake()
    // {
    //     _actor = this.EnsureComponent<Actor>();
    //     _actor.OnMove += HandleAfterMove;
    // }
    //
    // private void Start()
    // {
    //     var socketManager = this.EnsureComponent<SocketRegistry>();
    //     var primaryWeaponSocket = socketManager.GetSocketByName(primaryWeaponSocketName);
    //     var weaponManager = this.EnsureComponent<ActorWeaponManager2>();
    //     var weapon = weaponManager.ChangeWeapon(primaryWeaponSocket, primaryWeaponPrefab);
    //     _primaryWeapon = weapon;
    //     OnPrimaryWeaponChanged?.Invoke(weapon);
    // }
    //
    // private void HandleAfterMove()
    // {
    //     // var weapon = _primaryWeapon;
    //     // if (!_targetFinder.HasTarget)
    //     // {
    //     //     weapon.IsActivated = false;
    //     //     return;
    //     // }
    //     //
    //     // var targetPosition = _targetFinder.LastKnownTargetPosition;
    //     // var targetVelocity = _targetFinder.LastKnownTargetVelocity;
    //     // weapon.TryGetComponent(out WeaponShooter2D shooter);
    //     // var canHit = shooter.PredictAim(targetPosition, targetVelocity, out var predictAimVector);
    //     // if (!canHit)
    //     // {
    //     //     weapon.IsActivated = false;
    //     //     return;
    //     // }
    //     //
    //     // var currentAimVector = shooter.GetAimVector();
    //     // var angle = Vector2.Angle(currentAimVector.normalized, predictAimVector.normalized);
    //     // if (angle <= shootTolerance)
    //     // {
    //     //     weapon.IsActivated = true;
    //     // }
    //     // else
    //     // {
    //     //     weapon.IsActivated = false;
    //     // }
    // }
    //
    // public Weapon GetPrimaryWeapon()
    // {
    //     return _primaryWeapon;
    // }
}