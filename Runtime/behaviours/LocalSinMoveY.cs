using UnityEngine;
using Random = UnityEngine.Random;

public class LocalSinMoveY : MonoBehaviour
{
    private const float TWO_PI = Mathf.PI * 2.0f;

    [SerializeField] private float amplitude = 5;
    [SerializeField] private float period = 2f;
    [SerializeField] private bool randomizeInitialPhase = true;
    [SerializeField] private bool ignoreParentRotation;

    private float _elapsedTime;

    private void OnDrawGizmosSelected()
    {
        var position = transform.parent ? transform.parent.position : Vector3.zero;
        var rotation = ignoreParentRotation ? transform.localRotation : transform.rotation;

        // Draw min position
        var minPos = position + rotation * new Vector3(0, -amplitude);
        Gizmos.DrawWireSphere(minPos, 0.1f);

        // Draw max position
        var maxPos = position + rotation * new Vector3(0, amplitude);
        Gizmos.DrawWireSphere(maxPos, 0.1f);

        // Draw line between min and max
        Gizmos.DrawLine(minPos, maxPos);
    }

    private void OnEnable()
    {
        if (randomizeInitialPhase)
        {
            _elapsedTime = Random.Range(0, period);
        }
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        var time = MathUtil.Mod(_elapsedTime + deltaTime, period);
        var t = MathUtil.Remap(time, 0, period, 0, TWO_PI);
        var targetPosition = transform.localRotation * new Vector3(0, Mathf.Sin(t) * amplitude);
        if (ignoreParentRotation && transform.parent)
        {
            targetPosition = Quaternion.Inverse(transform.parent.rotation) * targetPosition;
        }

        transform.localPosition = targetPosition;
        _elapsedTime = time;
    }
}