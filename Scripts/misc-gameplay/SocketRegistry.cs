using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SocketRegistry : MonoBehaviour
{
    [SerializeField] private NamedPair<Transform>[] foundSocket;

    private readonly Dictionary<string, Transform> _socketByName = new();

    private void Awake()
    {
        _socketByName.Clear();
        foreach (var entry in foundSocket)
        {
            _socketByName.Add(entry.Name, entry.Value);
        }
    }

    public Transform GetSocketByName(string socketName)
    {
        return _socketByName.GetValueOrDefault(socketName);
    }

    [Button]
    private void FindSockets()
    {
        foundSocket = UnityUtil.TraverseTransform(transform, true)
            .Where(trans => trans.name.StartsWith("Socket_"))
            .Select(trans => new NamedPair<Transform>
            {
                Name = trans.name,
                Value = trans
            })
            .ToArray();
    }
}