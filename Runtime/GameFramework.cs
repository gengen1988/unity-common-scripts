using System;
using UnityEngine;

public class GameFramework : SingletonBehaviour<GameFramework>
{
    public event Action OnInit;

    protected override void UnityAwake()
    {
        // initial event processing
        GameEventBus.Publish<OnFrameworkInit>();
        GameEventBus.Instance.Process();
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        GameWorld.Instance.Tick(deltaTime);
    }
}