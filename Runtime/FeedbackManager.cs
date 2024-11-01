using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * primary class definition
 */
[Obsolete] // due to contains no actual feature
public class FeedbackManager : SingletonBehaviour<FeedbackManager>
{
    private readonly List<Feedback> _allFeedbacks = new();

    private void Update()
    {
        // float deltaTime = Time.deltaTime;
        // for (int i = _allFeedbacks.Count - 1; i >= 0; i--)
        // {
        //     Feedback feedback = _allFeedbacks[i];
        //     if (feedback.IsFinished())
        //     {
        //         _allFeedbacks.RemoveAt(i);
        //         PoolWrapper.Despawn(feedback.gameObject);
        //         continue;
        //     }
        //
        //     feedback.Tick(deltaTime);
        // }
    }
}