using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Weaver;

public class CameraManager : WeaverSingletonBehaviour<CameraManager>
{
    [AssetReference] private static readonly CameraManager SingletonPrefab;

    [FormerlySerializedAs("CameraConfiner")] [SerializeField]
    private CinemachineConfiner2D cameraConfiner;
    [FormerlySerializedAs("VirtualCamera")] [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [FormerlySerializedAs("TargetGroup")] [SerializeField]
    private CinemachineTargetGroup targetGroup;

    private Camera _mainCamera;

    public Camera MainCamera => _mainCamera;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
    }

    private void RefreshStage(Stage stage)
    {
        if (stage == null)
        {
            return;
        }

        var shape = stage.BoundingShape;
        cameraConfiner.m_BoundingShape2D = shape;
    }

    public void SetCameraPosition(Vector2 position)
    {
        virtualCamera.transform.position = position;
    }

    public void SetFollowTarget(Transform target, float weight = 1f, float radius = 5f)
    {
        // clear
        foreach (var targetInGroup in targetGroup.m_Targets)
        {
            targetGroup.RemoveMember(targetInGroup.target);
        }

        // add new
        targetGroup.AddMember(target, weight, radius);

        // reset cache
        cameraConfiner.InvalidateCache();
    }

    public void SetBoundingShape(Collider2D col)
    {
        cameraConfiner.m_BoundingShape2D = col;
    }
}