using System;
using UnityEngine;

[Obsolete]
public class ChangeMouseLookCenterForPrimaryWeapon : MonoBehaviour
{
    // [SerializeField] private string lookupName = "Socket_MouseLookCenter";
    //
    // private void Awake()
    // {
    //     var primaryWeaponMounter = GetComponentInParent<PrimaryWeaponMounter>();
    //     primaryWeaponMounter.OnPrimaryWeaponChanged += HandlePrimaryWeaponChanged;
    // }
    //
    // private void HandlePrimaryWeaponChanged(Weapon primaryWeapon)
    // {
    //     var socketManager = primaryWeapon.GetComponent<SocketRegistry>();
    //     if (!socketManager)
    //     {
    //         return;
    //     }
    //
    //     var socketAimCenter = socketManager.GetSocketByName(lookupName);
    //     if (socketAimCenter)
    //     {
    //         var brainInput = GetComponent<BrainInput>();
    //         brainInput.mouseLookCenter = socketAimCenter;
    //     }
    // }
}