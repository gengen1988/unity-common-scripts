using System.Collections.Generic;
using UnityEngine;


public class ServiceRegistry : MonoBehaviour
{
	readonly Dictionary<string, object> store = new Dictionary<string, object>();

	void Start()
	{
		Init();
	}

	public void Init()
	{
		var providers = GetComponentsInChildren<IServiceProvider>();
		var consumers = GetComponentsInChildren<IServiceConsumer>();

		foreach (var provider in providers)
		{
			provider.OnRegisterService(this);
		}

		foreach (var consumer in consumers)
		{
			consumer.OnFetchService(this);
		}
	}

	public void RegisterService(string serviceName, object service)
	{
		Debug.Log($"[{name}] register service: \"{serviceName}\" [{service.GetType()}]", this);

		if (store.ContainsKey(serviceName))
		{
			Debug.LogWarning($"[{name}] service {serviceName} already registered", this);
		}

		store[serviceName] = service;
	}

	public bool TryGetService<T>(string serviceName, out T service) where T : class
	{
		service = null;
		if (!store.TryGetValue(serviceName, out var result))
		{
			Debug.LogWarning($"[{name}] no service: \"{serviceName}\"", this);
			return false;
		}

		service = result as T;
		if (service == null)
		{
			Debug.LogWarning(
				$"[{name}] service type mismatch: \"{serviceName}\" is [{result.GetType()}], not [{typeof(T)}]", this);
			return false;
		}

		return true;
	}
}

public interface IServiceProvider
{
	void OnRegisterService(ServiceRegistry registry);
}

public interface IServiceConsumer
{
	void OnFetchService(ServiceRegistry registry);
}