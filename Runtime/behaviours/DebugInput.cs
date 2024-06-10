using System;
using UnityEngine;
using UnityEngine.Events;

public class DebugInput : MonoBehaviour
{
    public enum TriggerType
    {
        Once,
        Repeat,
    }

    [Serializable]
    public class ConfigEntry
    {
        public KeyCode Key;
        public TriggerType Type;
        public UnityEvent Handler;
    }

    public ConfigEntry[] Config;

    private void Reset()
    {
        Config = new[]
        {
            new ConfigEntry
            {
                Key = KeyCode.Z
            }
        };
    }

    private void Update()
    {
        foreach (ConfigEntry config in Config)
        {
            bool triggered = config.Type switch
            {
                TriggerType.Repeat => Input.GetKey(config.Key),
                TriggerType.Once => Input.GetKeyDown(config.Key),
                _ => false
            };

            if (triggered)
            {
                config.Handler.Invoke();
            }
        }
    }
}