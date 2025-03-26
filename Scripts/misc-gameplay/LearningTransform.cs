using System;
using Drawing;
using Sirenix.OdinInspector;
using UnityEngine;

public class LearningTransform : MonoBehaviour
{
    public Transform Target;
    public Transform YawTrans, PitchTrans;

    [ShowInInspector, ReadOnly]
    private Quaternion _desiredRotation;

    private Transform _mountTrans;

    private void Reset()
    {
        // if (!YawTrans)
        // {
        //     // YawTrans = GetComponentInChildren<LearningTransform>()?.transform;
        // }
    }

    private void Awake()
    {
        _mountTrans = YawTrans.parent.transform;
    }

    private void Update()
    {
        var origin = transform.position;
        var targetPosition = Target.position;
        var los = targetPosition - origin;
        Draw.Arrow(origin, origin + los, Color.magenta);
        Draw.Label2D(origin + los, "target");

        var mountUp = _mountTrans.up;
        var mountForward = _mountTrans.forward;
        Draw.Arrow(origin, origin + mountForward, Color.blue);
        Draw.Arrow(origin, origin + mountUp, Color.green);
        Draw.Label2D(origin + mountUp, "mount up");
        Draw.Label2D(origin + mountForward, "mount forward");

        var losOnMountUpPlane = Vector3.ProjectOnPlane(los, mountUp);
        Draw.DashedLine(origin, origin + losOnMountUpPlane, .02f, .02f, Color.magenta);
        Draw.DashedLine(origin + los, origin + losOnMountUpPlane, .02f, .02f, Color.magenta);

        var desiredYaw = Vector3.SignedAngle(mountForward, losOnMountUpPlane, mountUp);
        Draw.Label2D(origin + losOnMountUpPlane, $"desired yaw: {desiredYaw:F0}");

        var yawForward = YawTrans.forward;
        var deltaYaw = Vector3.SignedAngle(yawForward, losOnMountUpPlane, mountUp);
        var losOnMountUpAxis = Vector3.Project(los, mountUp);
        var height = Vector3.Dot(los, mountUp);
        var radius = (los - losOnMountUpAxis).magnitude;
        Draw.DashedLine(origin + losOnMountUpAxis, origin + los, .02f, .02f, Color.magenta);

        var center = origin + mountUp * height;
        Draw.Circle(origin, mountUp, radius, Color.yellow);

        Draw.Label2D(center, $"height: {height}");
        Draw.Line(origin, origin + yawForward * radius, Color.yellow);
        Draw.Label2D(origin + yawForward * radius, $"delta yaw: {deltaYaw:F0}, ratio: {1 - Mathf.Abs(deltaYaw) / 180f:P0}");

        var yawRight = YawTrans.right;
        Draw.Circle(origin, yawRight, radius, Color.cyan);

        var pitchForward = PitchTrans.forward;
        Draw.Line(origin, origin + pitchForward * radius, Color.cyan);

        var pitchAngle = -Mathf.Atan2(height, radius) * Mathf.Rad2Deg;
        var desiredPitch = YawTrans.rotation * Quaternion.Euler(pitchAngle, 0, 0) * Vector3.forward;
        Draw.DashedLine(origin, origin + desiredPitch * radius, .02f, .02f, Color.magenta);
        var deltaPitch = Vector3.SignedAngle(pitchForward, desiredPitch, yawRight);
        Draw.Label2D(origin + pitchForward * radius, $"delta pitch: {deltaPitch:F0}");


        // var mountForwardOn2DPlane = Vector3.ProjectOnPlane(mountForward, Vector3.forward);
        // Draw.DashedLine(origin, origin + mountForwardOn2DPlane, .02f, .02f, Color.blue);
        // Draw.DashedLine(origin + mountForward, origin + mountForwardOn2DPlane, .02f, .02f, Color.blue);
        // Quaternion.LookRotation(targetPosition - origin, Vector3.up);
        // Quaternion.


        // var yawNormal = YawTrans.right;
        // var yawForward = YawTrans.forward;
        // var yawUp = YawTrans.up;
        // var losOnYawUpPlane = Vector3.ProjectOnPlane(los, yawUp);
        // var losOnYawUpAxis = Vector3.Project(los, yawUp);
        // Draw.Arrow(origin, origin + los, Color.magenta);
        // Draw.Arrow(origin, origin + losOnYawUpPlane, Color.green);
        // Draw.Arrow(origin, origin + losOnYawUpAxis, Color.blue);
        //
        // var height = Vector3.Dot(los - losOnYawUpPlane, yawUp);
        // var radius = (los - losOnYawUpAxis).magnitude;
        // var pitchAngle = Mathf.Atan2(height, radius) * Mathf.Rad2Deg;
        // _desiredRotation = Quaternion.Euler(-pitchAngle, 0f, 0f);
        // PitchTrans.localRotation = _desiredRotation;
    }
}