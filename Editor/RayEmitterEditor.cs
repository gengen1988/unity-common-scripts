using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RayEmitter))]
public class RayEmitterEditor : Editor
{
    void OnSceneGUI()
    {
        if (!(target is RayEmitter emitter)) return;

        var ray = emitter.GetRay();
        Handles.Label(emitter.direction, "ray direction");
        Handles.Label(ray.GetPoint(emitter.length), "ray end");
        Handles.DrawWireDisc(ray.origin, Vector3.back, emitter.length);

        EditorGUI.BeginChangeCheck();
        var newTargetPosition = Handles.PositionHandle(emitter.direction, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Look At Target Position");
            emitter.direction = newTargetPosition;
        }
    }
}