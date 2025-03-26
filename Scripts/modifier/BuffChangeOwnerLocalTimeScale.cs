using UnityEngine;

// public class BuffChangeOwnerLocalTimeScale : MonoBehaviour, ISubmoduleMount<Buff>, ISubmoduleUnmount<Buff>
// {
//     [SerializeField] private TimeDomain Domain;
//     [SerializeField] private float LocalTimeScale = 1f;
//
//     private Actor _actor;
//
//     public void Mount(Buff submodule)
//     {
//         submodule.Owner.TryGetComponent(out _actor);
//         _actor.Clock.EnterDomain(Domain, LocalTimeScale);
//     }
//
//     public void Unmount(Buff submodule)
//     {
//         _actor.Clock.LeaveDomain(Domain);
//     }
// }