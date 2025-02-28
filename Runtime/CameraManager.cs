using Cinemachine;
using UnityEngine;
using Weaver;

public class CameraManager : WeaverSingletonBehaviour<CameraManager>
{
    [AssetReference] private static readonly CameraManager SingletonPrefab;

    [SerializeField] private CinemachineConfiner2D CameraConfiner;
    [SerializeField] private CinemachineVirtualCamera VirtualCamera;
    [SerializeField] private CinemachineTargetGroup TargetGroup;

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
        CameraConfiner.m_BoundingShape2D = shape;
    }

    public void SetCameraPosition(Vector2 position)
    {
        VirtualCamera.transform.position = position;
    }

    public void SetFollowTarget(Transform target, float weight = 1f, float radius = 5f)
    {
        // clear
        foreach (var targetInGroup in TargetGroup.m_Targets)
        {
            TargetGroup.RemoveMember(targetInGroup.target);
        }

        // add new
        TargetGroup.AddMember(target, weight, radius);

        // reset cache
        CameraConfiner.InvalidateCache();
    }

    public void SetBoundingShape(Collider2D col)
    {
        CameraConfiner.m_BoundingShape2D = col;
    }
}