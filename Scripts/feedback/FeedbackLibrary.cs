using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class FeedbackLibrary : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public CueChannel Channel;
        public Feedback Prefab;
    }

    [TableList]
    [SerializeField] private Entry[] entries;

    public IReadOnlyList<Entry> Entries => entries;

#if UNITY_EDITOR
    public void SetupEntry(CueChannel channel, Feedback feedback)
    {
        // Check if the entry already exists
        if (entries == null)
        {
            entries = new Entry[0];
        }

        // Look for an existing entry with this channel
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Channel == channel)
            {
                // Update existing entry
                entries[i].Prefab = feedback;
                return;
            }
        }

        // Add a new entry
        Array.Resize(ref entries, entries.Length + 1);
        entries[entries.Length - 1] = new Entry
        {
            Channel = channel,
            Prefab = feedback
        };

        // Mark the asset as dirty to ensure it gets saved
        EditorUtility.SetDirty(this);
    }

    public Feedback FindFeedbackByChannel(CueChannel channel)
    {
        if (!channel)
        {
            return null;
        }

        foreach (var entry in entries)
        {
            if (entry.Channel == channel)
            {
                return entry.Prefab;
            }
        }

        return null;
    }

    [Button]
    private void FindAllCues()
    {
        var allCueChannels = AssetDatabase.FindAssets("t:CueChannel");
        var existingChannels = new HashSet<CueChannel>();

        // Collect existing channels
        foreach (var entry in entries)
        {
            if (entry.Channel != null)
            {
                existingChannels.Add(entry.Channel);
            }
        }

        // Find new channels
        var newEntries = new List<Entry>();
        foreach (var guid in allCueChannels)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var cueChannel = AssetDatabase.LoadAssetAtPath<CueChannel>(path);

            if (!existingChannels.Contains(cueChannel))
            {
                newEntries.Add(new Entry
                {
                    Channel = cueChannel,
                    Prefab = null
                });
            }
        }

        // Add new entries to the array
        if (newEntries.Count > 0)
        {
            var oldLength = entries?.Length ?? 0;
            var newLength = oldLength + newEntries.Count;
            var newArray = new Entry[newLength];

            if (entries != null)
            {
                Array.Copy(entries, newArray, oldLength);
            }

            for (int i = 0; i < newEntries.Count; i++)
            {
                newArray[oldLength + i] = newEntries[i];
            }

            entries = newArray;

            Debug.Log($"Added {newEntries.Count} new CueChannel entries to the library.");
        }
        else
        {
            Debug.Log("No new CueChannels found to add.");
        }
    }
#endif
}