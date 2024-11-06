// using UnityEngine;
//
// public class BuffDealDamageOvertime : MonoBehaviour
// {
//     [SerializeField] float DamagePerSeconds = 100f;
//
//     private Buff _buff;
//     private ActorHealth _health;
//
//     private void Awake()
//     {
//         TryGetComponent(out _buff);
//         _buff.Owner.TryGetComponent(out _health);
//     }
//
//     private void OnEnable()
//     {
//         _buff.Owner.OnPerceive += HandleOwnerPerceive;
//     }
//
//     private void OnDisable()
//     {
//         _buff.Owner.OnPerceive -= HandleOwnerPerceive;
//     }
//
//     private void HandleOwnerPerceive(ActorOld buffOwner)
//     {
//         if (!_health)
//         {
//             return;
//         }
//
//         if (!ActorManager.IsAlive(buffOwner))
//         {
//             return;
//         }
//
//         float deltaTime = buffOwner.Timer.DeltaTime;
//         int stackCount = _buff.GetStackCount();
//         int damage = Mathf.FloorToInt(DamagePerSeconds * stackCount * deltaTime);
//         _health.DealDamage(damage);
//     }
// }