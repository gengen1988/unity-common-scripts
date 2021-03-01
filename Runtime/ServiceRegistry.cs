using System.Collections.Generic;
using UnityEngine;


public class ServiceRegistry : MonoBehaviour
{
    readonly Dictionary<string, object> store = new Dictionary<string, object>();

    void Awake()
    {
        var providers = GetComponentsInChildren<IServiceProvider>();
        var consumers = GetComponentsInChildren<IServiceConsumer>();

        foreach (var provider in providers)
        {
            provider.RegisterService(this);
        }

        foreach (var consumer in consumers)
        {
            consumer.FetchService(this);
        }
    }

    public void RegisterService(string serviceName, object service)
    {
        Debug.Log($"[{name}] register service: \"{serviceName}\" [{service.GetType()}]");
        store[serviceName] = service;
    }

    public bool TryGetService<T>(string serviceName, out T service) where T : class
    {
        var success = store.TryGetValue(serviceName, out var result);
        if (!success)
        {
            Debug.LogWarning($"[{name}] no service name: \"{serviceName}\"");
            service = null;
            return false;
        }

        service = result as T;
        if (service == null)
        {
            Debug.LogWarning(
                $"[{name}] service type mismatch: \"{serviceName}\" is [{result.GetType()}], not [{typeof(T)}]");
        }

        return service != null;
    }
}


public interface IServiceProvider
{
    void RegisterService(ServiceRegistry registry);
}

public interface IServiceConsumer
{
    void FetchService(ServiceRegistry registry);
}