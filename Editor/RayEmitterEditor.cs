using System;
using UnityEditor;
using UnityEngine;

/*
[Obsolete]
[CustomEditor(typeof(RayEmitter))]
public class RayEmitterEditor : Editor
{
    void OnSceneGUI()
    {
        if (!(target is RayEmitter emitter)) return;

        var ray = emitter.GetRay();
        Handles.Label(emitter.aim, "ray aim");
        Handles.Label(ray.GetPoint(emitter.length), "ray end");
        Handles.DrawWireDisc(ray.origin, Vector3.back, emitter.length);

        EditorGUI.BeginChangeCheck();
        var newTargetPosition = Handles.PositionHandle(emitter.aim, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Change Look At Target Position");
            emitter.aim = newTargetPosition;
            var los = emitter.aim - emitter.transform.position;
            emitter.transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0,0, 90) * los);
        }
    }
}
*/