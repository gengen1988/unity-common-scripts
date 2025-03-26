using UnityEngine;

public class StopAnimationAtStart : MonoBehaviour
{
    private void Start()
    {
        // if (TryGetComponent(out AnimancerComponent animancer))
        // {
        //     animancer
        // }
        if (TryGetComponent(out Animator animator))
        {
            // animator.StopPlayback();
            animator.speed = 0;
        }
    }
}