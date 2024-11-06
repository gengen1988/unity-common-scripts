// using UnityEngine;
//
// public class ActorAddBuffOnHit : MonoBehaviour
// {
//     public BuffProfile BuffToBeAdd;
//
//     private void Awake()
//     {
//         ActorHitManager hitManager = GetComponentInParent<ActorHitManager>();
//         hitManager.OnHit += HandleHit;
//     }
//
//     private void HandleHit(ActorOld hitSubject, ActorOld hurtSubject, HitEventData evtData)
//     {
//         // 可能之前被别的逻辑打死了
//         if (!ActorOld.IsAlive(hurtSubject))
//         {
//             return;
//         }
//
//         if (hurtSubject.TryGetComponent(out ActorBuffManager buffManager))
//         {
//             buffManager.AddBuff(BuffToBeAdd);
//         }
//     }
// }