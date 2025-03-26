using System;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


[Obsolete] // replace by Turret2D
// this script mapping 2d side scroll view (xy plane) control vector to 3d model which +z is forward,
// model already wrapped by a transform to align +x as forward
// add this script on turret transform (yaw rotation one)
[DefaultExecutionOrder(CustomExecutionOrder.Late)]
public class TurretControl2D : MonoBehaviour
{
    // [SerializeField] private Transform yawTrans; // for pitch, change x-axis only
    // [SerializeField] private Transform pitchTrans; // for pitch, change x-axis only
    // [SerializeField] private float yawSpeed = 180;
    // [SerializeField] private float pitchSpeed = 90;
    // // [SerializeField] private TurretMode turretMode = TurretMode.Automatic;
    //
    // // [SerializeField] [MinMaxSlider(-180, 180)]
    // // private Vector2 yawLimit = new(-180, 180);
    //
    // [MinMaxSlider(-90, 90)]
    // [SerializeField] private Vector2 pitchLimit = new(-30, 10); // negative value is up
    // [SerializeField] private float alignAngleTolerance = 5;
    //
    // private Transform _mountTrans; // to determine forward and up for yaw
    // private Pawn _pawn;
    // private GameClock _clock;
    // private GameEntityBridge _entity;
    //
    //
    // private Vector3 _los;
    //
    // // public override void DrawGizmos()
    // // {
    // //     // var mountTrans = transform;
    // //     // var mountCenter = mountTrans.position;
    // //     // var mountUp = mountTrans.up;
    // //
    // //     // using (Draw.WithColor(Color.green))
    // //     // {
    // //     //     Draw.SolidCircle(mountCenter, mountUp, 2f);
    // //     // }
    // //
    // //     // if (Application.isPlaying)
    // //     // {
    // //     //     // Draw line of sight vector from mount center
    // //     //     using (Draw.WithColor(Color.red))
    // //     //     {
    // //     //         // Draw the line of sight vector with a reasonable length (e.g., 5 units)
    // //     //         Draw.Line(mountCenter, mountCenter + _los.normalized * 5f);
    // //     //     }
    // //     // }
    // // }
    //
    // private void Awake()
    // {
    //     _entity = GetComponentInParent<GameEntityBridge>();
    //     _mountTrans = transform;
    // }
    //
    // private ITurretTargetDesignator _designator;
    // private WeaponShooter2D _shooter;
    //
    // public void FixedUpdate()
    // {
    //     var deltaTime = _entity.Clock.LocalDeltaTime;
    //     if (!_designator.TargetLocked)
    //     {
    //         return;
    //     }
    //
    //     var targetPosition = _designator.GetTargetPosition();
    //     var targetVelocity = _designator.GetTargetVelocity();
    //     if (!_shooter.PredictInterceptDirection(targetPosition, targetVelocity, out var interceptVector))
    //     {
    //         return;
    //     }
    //
    //     if (IsAligned(interceptVector))
    //     {
    //         _shooter.Fire();
    //     }
    //
    //     RotateTurret(interceptVector, deltaTime);
    // }
    //
    // public bool IsAligned(Vector2 los)
    // {
    //     var launchVector = _shooter.GetLaunchDirection();
    //     var deltaAngle = Vector3.Angle(los, launchVector);
    //     return Mathf.Abs(deltaAngle) < alignAngleTolerance;
    // }
    //
    // public void RotateTurret(Vector2 los, float deltaTime)
    // {
    //     // Skip rotation if line of sight is too small
    //     if (_los.sqrMagnitude < 0.01f)
    //     {
    //         return;
    //     }
    //
    //     // Check if the turret can align with the line of sight
    //     // If the los is directly opposite to the mount forward, it might not be possible to align
    //     var mountUp = _mountTrans.up;
    //     var mountForward = _mountTrans.forward;
    //     var losOnMountUpPlane = Vector3.ProjectOnPlane(_los, mountUp);
    //     var losOnMountUpAxis = Vector3.Project(_los, mountUp);
    //     var desiredYaw = Vector3.SignedAngle(mountForward, losOnMountUpPlane, mountUp);
    //     var height = Vector3.Dot(_los, mountUp);
    //     var radius = (_los - losOnMountUpAxis).magnitude;
    //     var desiredPitch = -Mathf.Atan2(height, radius) * Mathf.Rad2Deg;
    //
    //     // If the projected vector is too small, the turret can't align properly
    //     if (losOnMountUpPlane.sqrMagnitude < 0.001f)
    //     {
    //         return;
    //     }
    //
    //     // rotate turret yaw
    //     if (yawTrans)
    //     {
    //         var nextYawLocalRotation = Quaternion.RotateTowards(
    //             yawTrans.localRotation,
    //             Quaternion.Euler(0, desiredYaw, 0),
    //             yawSpeed * deltaTime
    //         );
    //         yawTrans.localRotation = nextYawLocalRotation;
    //     }
    //
    //     // rotate barrel pitch
    //     desiredPitch = Mathf.Clamp(desiredPitch, pitchLimit.x, pitchLimit.y); // angle limit
    //     var nextPitchLocalRotation = Quaternion.RotateTowards(
    //         pitchTrans.localRotation,
    //         Quaternion.Euler(desiredPitch, 0, 0),
    //         pitchSpeed * deltaTime
    //     );
    //     pitchTrans.localRotation = nextPitchLocalRotation;
    // }
}

public interface ITurretTargetDesignator
{
    bool TargetLocked { get; set; }
    Vector2 GetTargetPosition();
    Vector2 GetTargetVelocity();
}