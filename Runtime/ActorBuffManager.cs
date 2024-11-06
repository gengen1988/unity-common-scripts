// using System.Collections.Generic;
// using UnityEngine;
//
// public class ActorBuffManager : MonoBehaviour
// {
//     private const string MESSAGE_ADD = "add";
//     private const string MESSAGE_REMOVE = "remove";
//
//     public class ManagedEntry
//     {
//         public BuffProfile Profile;
//         public Buff Buff;
//         public float ElapsedTime;
//         public StateMachine<ManagedEntry> StateMachine;
//         public ActorBuffManager Manager;
//     }
//
//     [SerializeField] private Transform BuffContainer;
//
//     private ActorOld _buffOwner;
//
//     private readonly List<ManagedEntry> _toBeAdd = new();
//     private readonly List<ManagedEntry> _toBeRemove = new();
//     private readonly Dictionary<BuffProfile, ManagedEntry> _entryByProfile = new();
//
//     public ActorOld Owner => _buffOwner;
//
//     private void Reset()
//     {
//         if (!BuffContainer)
//         {
//             BuffContainer = transform;
//         }
//     }
//
//     private void Awake()
//     {
//         TryGetComponent(out _buffOwner);
//         _buffOwner.OnPerceive += HandleOwnerPerceive;
//         _buffOwner.OnDie += HandleOwnerDie;
//     }
//
//     private void OnEnable()
//     {
//         // reset for pooling
//         _toBeAdd.Clear();
//         _toBeRemove.Clear();
//         _entryByProfile.Clear();
//     }
//
//     private void HandleOwnerPerceive(ActorOld buffOwner)
//     {
//         // scheduled remove
//         for (int i = _toBeRemove.Count - 1; i >= 0; i--)
//         {
//             ManagedEntry entry = _toBeRemove[i];
//             _toBeRemove.RemoveAt(i);
//             entry.StateMachine.SendEvent(MESSAGE_REMOVE);
//         }
//
//         // time out logic
//         float deltaTime = buffOwner.Timer.DeltaTime;
//         foreach (ManagedEntry entry in _entryByProfile.Values)
//         {
//             float lifeTime = entry.Profile.DefaultLifeTime;
//             if (lifeTime > 0 && entry.ElapsedTime > lifeTime)
//             {
//                 _toBeRemove.Add(entry); // schedule to next tick
//                 continue;
//             }
//
//             entry.ElapsedTime += deltaTime;
//         }
//
//         // scheduled add
//         for (int i = _toBeAdd.Count - 1; i >= 0; i--)
//         {
//             ManagedEntry entry = _toBeAdd[i];
//             _toBeAdd.RemoveAt(i);
//             entry.StateMachine.SendEvent(MESSAGE_ADD);
//         }
//     }
//
//     private void HandleOwnerDie(ActorOld buffOwner)
//     {
//         // cleanup
//         _toBeRemove.AddRange(_entryByProfile.Values);
//         foreach (ManagedEntry entry in _toBeRemove)
//         {
//             entry.StateMachine.SendEvent(MESSAGE_REMOVE);
//         }
//     }
//
//     public Buff AddBuff(BuffProfile profile)
//     {
//         if (!profile)
//         {
//             return null;
//         }
//
//         if (!ActorManager.IsAlive(_buffOwner))
//         {
//             Debug.LogWarning("owner is not alive", _buffOwner);
//             return null;
//         }
//
//         if (!_entryByProfile.TryGetValue(profile, out ManagedEntry entry))
//         {
//             entry = new ManagedEntry
//             {
//                 Manager = this,
//                 Profile = profile,
//             };
//             entry.StateMachine = new StateMachine<ManagedEntry>(entry);
//             entry.StateMachine.TransitionTo(Birth.Instance);
//         }
//
//         _toBeAdd.Add(entry); // event scheduling
//         return entry.Buff;
//     }
//
//     public void RemoveBuff(BuffProfile profile)
//     {
//         if (!profile)
//         {
//             Debug.LogError("remove null buff", _buffOwner);
//             return;
//         }
//
//         if (!ActorManager.IsAlive(_buffOwner))
//         {
//             Debug.LogWarning("owner is not alive", _buffOwner);
//             return;
//         }
//
//         if (!_entryByProfile.TryGetValue(profile, out ManagedEntry entry))
//         {
//             Debug.LogWarning($"buff not found: {profile}", _buffOwner);
//             return;
//         }
//
//         _toBeRemove.Add(entry);
//     }
//
//     public class Birth : State<ManagedEntry, Birth>
//     {
//         public override void OnEnter(ManagedEntry ctx)
//         {
//             ActorBuffManager manager = ctx.Manager;
//             BuffProfile profile = ctx.Profile;
//             GameObject prefab = profile.Prefab;
//             Transform container = manager.BuffContainer;
//             GameObject buffObject = StagingSystem.Prepare(t => Instantiate(prefab, t), container);
//             buffObject.TryGetComponent(out Buff buff);
//             buff.Init(manager, profile);
//             ctx.Buff = buff;
//             manager._entryByProfile[profile] = ctx;
//         }
//
//         public override void OnEvent(ManagedEntry ctx, GameEvent message)
//         {
//             switch (message)
//             {
//                 case MESSAGE_ADD:
//                     ctx.StateMachine.TransitionTo(Alive.Instance);
//                     return;
//                 case MESSAGE_REMOVE:
//                     ctx.StateMachine.TransitionTo(Dead.Instance);
//                     return;
//             }
//         }
//     }
//
//     public class Alive : State<ManagedEntry, Alive>
//     {
//         public override void OnEnter(ManagedEntry ctx)
//         {
//             Buff buff = ctx.Buff;
//             if (StagingSystem.IsStaging(buff.gameObject))
//             {
//                 StagingSystem.Commit(buff.gameObject);
//             }
//
//             ctx.Buff.Add();
//         }
//
//         public override void OnEvent(ManagedEntry ctx, GameEvent message)
//         {
//             switch (message)
//             {
//                 case MESSAGE_ADD:
//                     ctx.StateMachine.TransitionTo(this); // re-enter to trigger add event
//                     return;
//
//                 case MESSAGE_REMOVE:
//                     ctx.StateMachine.TransitionTo(Dead.Instance);
//                     return;
//             }
//         }
//     }
//
//     public class Dead : State<ManagedEntry, Dead>
//     {
//         public override void OnEnter(ManagedEntry ctx)
//         {
//             Dictionary<BuffProfile, ManagedEntry> index = ctx.Manager._entryByProfile;
//             if (index.ContainsKey(ctx.Profile))
//             {
//                 index.Remove(ctx.Profile);
//             }
//             else
//             {
//                 Debug.LogWarning($"buff already removed: {ctx.Profile}", ctx.Manager._buffOwner);
//                 return;
//             }
//
//             Buff buff = ctx.Buff;
//             if (StagingSystem.IsStaging(buff.gameObject))
//             {
//                 StagingSystem.Remove(buff.gameObject);
//             }
//
//             Destroy(buff.gameObject);
//         }
//     }
// }