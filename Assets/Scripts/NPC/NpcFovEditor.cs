// Field of view visualisation Pt. 1 https://www.youtube.com/watch?v=rQG9aUWarwE&t=1215s
// Field of view visualisation Pt. 2 https://www.youtube.com/watch?v=73Dc5JTCmKI&t=653s

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleNpcFov))]
public class NpcFovEditor : Editor
{
    void OnSceneGUI()
    {
        SimpleNpcFov fov = (SimpleNpcFov)target;
        Handles.color = Color.white;

        //Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius); // Draw radius.

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius); // Draw arc.

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }

}
