using System;
using UnityEngine;

// public class LocalEventBus : MonoBehaviour
// {
//     private readonly GameEventBus _eventBus = new();
//
//     public static void Emit<T>(Transform trans, Action<T> initializer = null) where T : GameEvent, new()
//     {
//         var component = trans.EnsureComponent<LocalEventBus>();
//         component._eventBus.Emit(initializer);
//     }
//
//     public static void Subscribe<T>(Transform trans, Action<T> listener) where T : GameEvent, new()
//     {
//         var component = trans.EnsureComponent<LocalEventBus>();
//         component._eventBus.Subscribe(listener);
//     }
//
//     public static void Unsubscribe<T>(Transform trans, Action<T> listener) where T : GameEvent, new()
//     {
//         var component = trans.EnsureComponent<LocalEventBus>();
//         component._eventBus.Unsubscribe(listener);
//     }
// }