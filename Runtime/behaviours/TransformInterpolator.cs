using UnityEngine;

// 注意这个插帧和连续碰撞检测不兼容
// source: https://discussions.unity.com/t/motion-interpolation-solution-to-eliminate-fixedupdate-stutter/891926
/// <summary>
/// How to use TransformInterpolator properly:
/// 0. Make sure the gameObject executes its mechanics (transform-manipulations) in FixedUpdate().
/// 1. Make sure VSYNC is enabled. (?)
/// 2. Set the execution order for this script BEFORE all the other scripts that execute mechanics.
/// 3. Attach (and enable) this component to every gameObject that you want to interpolate (including the camera).
/// </summary>
[DefaultExecutionOrder(-5000)]
public class TransformInterpolator : MonoBehaviour
{
    private Transform _self;
    private TransformData _transformData;
    private TransformData _prevTransformData;
    private bool _isTransformInterpolated;

    private void Awake()
    {
        _self = transform;
    }

    //Init prevTransformData to interpolate from the correct state in the first frame the interpolation becomes active.
    //This can occur when the object is spawned/instantiated.
    private void OnEnable()
    {
        _prevTransformData.Position = _self.localPosition;
        _prevTransformData.Rotation = _self.localRotation;
        _prevTransformData.Scale = _self.localScale;
        _isTransformInterpolated = false;
    }

    private void FixedUpdate()
    {
        //Reset transform to its supposed current state just once after each Update/Drawing.
        if (_isTransformInterpolated)
        {
            _self.localPosition = _transformData.Position;
            _self.localRotation = _transformData.Rotation;
            _self.localScale = _transformData.Scale;
            _isTransformInterpolated = false;
        }

        //Cache current transform state as previous
        //(becomes "previous" by the next transform-manipulation
        //in FixedUpdate() of another component).
        _prevTransformData.Position = _self.localPosition;
        _prevTransformData.Rotation = _self.localRotation;
        _prevTransformData.Scale = _self.localScale;
    }

    private void LateUpdate() //Interpolate in Update() or LateUpdate().
    {
        //Cache the updated transform so that it can be restored in
        //FixedUpdate() after drawing.
        if (!_isTransformInterpolated)
        {
            _transformData.Position = _self.localPosition;
            _transformData.Rotation = _self.localRotation;
            _transformData.Scale = _self.localScale;

            //This promise matches the execution that follows after that.
            _isTransformInterpolated = true;
        }

        //(Time.time - Time.fixedTime) is the "unprocessed" time according to documentation.
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

        //Interpolate transform:
        _self.localPosition = Vector3.Lerp(_prevTransformData.Position, _transformData.Position, alpha);
        _self.localRotation = Quaternion.Slerp(_prevTransformData.Rotation, _transformData.Rotation, alpha);
        _self.localScale = Vector3.Lerp(_prevTransformData.Scale, _transformData.Scale, alpha);
    }
}