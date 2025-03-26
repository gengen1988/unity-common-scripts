using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Turret2D : MonoBehaviour, ITurret
{
    private const float LOS_DEADZONE = 0.01f;

    [SerializeField] private Transform yawTrans; // for yaw, change y-axis only
    [SerializeField] private Transform pitchTrans; // for pitch, change x-axis only
    [SerializeField] private float yawSpeed = 180;
    [SerializeField] private float pitchSpeed = 90;
    [SerializeField] private float alignAngleTolerance = 1f;
    [SerializeField] private float alignTime = .5f;
    [SerializeField] private float recoilRecoveryTime = .1f;

    // [SerializeField] [MinMaxSlider(-180, 180)]
    // private Vector2 yawLimit = new(-180, 180);

    [SerializeField]
    [MinMaxSlider(-180, 180)]
    private Vector2 pitchLimit = new(-30, 10); // negative value is up

    private bool _isRecoiling;
    private Transform _mountTrans; // to determine forward and up for yaw
    private ITurretManager _manager;
    private Shooter2D _shooter;
    private float _alreadyAlignTime;

    // public override void DrawGizmos()
    // {
    //     // var mountTrans = transform;
    //     // var mountCenter = mountTrans.position;
    //     // var mountUp = mountTrans.up;
    //
    //     // using (Draw.WithColor(Color.green))
    //     // {
    //     //     Draw.SolidCircle(mountCenter, mountUp, 2f);
    //     // }
    //
    //     // if (Application.isPlaying)
    //     // {
    //     //     // Draw line of sight vector from mount center
    //     //     using (Draw.WithColor(Color.red))
    //     //     {
    //     //         // Draw the line of sight vector with a reasonable length (e.g., 5 units)
    //     //         Draw.Line(mountCenter, mountCenter + _los.normalized * 5f);
    //     //     }
    //     // }
    // }

    private void Awake()
    {
        _mountTrans = transform;
        TryGetComponent(out _shooter);
        _manager = GetComponentInParent<ITurretManager>();

        _shooter.OnLaunch += HandleLaunch;
    }

    private void OnEnable()
    {
        _manager.AddTurret(this);
    }

    private void OnDisable()
    {
        _manager.RemoveTurret(this);
    }

    public void RotateTowards(Vector2 los, float deltaTime)
    {
        // Skip rotation if recoiling
        if (_isRecoiling)
        {
            return;
        }

        // Skip rotation if line of sight is too small
        if (los.sqrMagnitude < LOS_DEADZONE)
        {
            return;
        }

        // Check if the turret can align with the line of sight
        // If the los is directly opposite to the mount forward, it might not be possible to align
        var mountUp = _mountTrans.up;
        var mountForward = _mountTrans.forward;
        var losOnMountUpPlane = Vector3.ProjectOnPlane(los, mountUp);
        var losOnMountUpAxis = Vector3.Project(los, mountUp);
        var desiredYaw = Vector3.SignedAngle(mountForward, losOnMountUpPlane, mountUp);
        var height = Vector3.Dot(los, mountUp);
        var radius = ((Vector3)los - losOnMountUpAxis).magnitude;
        var desiredPitch = -Mathf.Atan2(height, radius) * Mathf.Rad2Deg;

        // If the projected vector is too small, the turret can't align properly
        if (losOnMountUpPlane.sqrMagnitude < 0.001f)
        {
            return;
        }

        // if (desiredPitch < pitchLimit.x || desiredPitch > pitchLimit.y)
        // {
        //     return;
        // }

        // rotate turret yaw
        if (yawTrans)
        {
            var nextYawLocalRotation = Quaternion.RotateTowards(
                yawTrans.localRotation,
                Quaternion.Euler(0, desiredYaw, 0),
                yawSpeed * deltaTime
            );
            yawTrans.localRotation = nextYawLocalRotation;
        }

        // rotate barrel pitch
        desiredPitch = Mathf.Clamp(desiredPitch, pitchLimit.x, pitchLimit.y); // angle limit
        var nextPitchLocalRotation = Quaternion.RotateTowards(
            pitchTrans.localRotation,
            Quaternion.Euler(desiredPitch, 0, 0),
            pitchSpeed * deltaTime
        );
        pitchTrans.localRotation = nextPitchLocalRotation;
    }

    public bool IsAligned(Vector2 los, float deltaTime)
    {
        if (los.sqrMagnitude < LOS_DEADZONE)
        {
            _alreadyAlignTime = 0;
            return false;
        }

        // Get the current forward direction of the turret
        var turretForward = (Vector2)pitchTrans.forward;

        // Calculate the angle between the turret's forward direction and the line of sight
        var angle = Vector2.Angle(turretForward, los);

        // Check if the angle is within the tolerance
        if (angle > alignAngleTolerance)
        {
            _alreadyAlignTime = 0;
            return false;
        }
        else
        {
            _alreadyAlignTime += deltaTime;
            return _alreadyAlignTime > alignTime;
        }
    }

    private void HandleLaunch(Projectile projectile, Vector2 position, Vector2 velocity)
    {
        StartCoroutine(RecoilProcedure());
    }

    // TODO refactor to frame perfect to reflect local time scale
    private IEnumerator RecoilProcedure()
    {
        _isRecoiling = true;
        yield return new WaitForSeconds(recoilRecoveryTime); // Adjust time as needed
        _isRecoiling = false;
    }

    public Shooter2D GetShooter()
    {
        return _shooter;
    }
}