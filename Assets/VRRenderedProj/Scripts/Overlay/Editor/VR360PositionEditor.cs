using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VR360Position))]
[CanEditMultipleObjects]
public class VR360PositionEditor : Editor {

    bool showOffset = false;

    void OnSceneGUI() {
        VR360Position position = (VR360Position)target;

        // Position
        Vector3 pos = Handles.FreeMoveHandle(position.transform.position, Quaternion.identity, .03f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
        Quaternion rot = position.transform.rotation;
        if (pos != position.transform.position) {
            pos = findPositiontOnProjectionFromCamera(pos, position.OffsetFromSphere);
            position.transform.position = pos;

            if (position.RotateToCamera) {
                position.transform.LookAt(Vector3.zero);
                //position.transform.Rotate(new Vector3(0, 180, 0));
            }
        }
    }
    
    public override void OnInspectorGUI() {
        this.DrawDefaultInspector();
    }

    Vector3 findPositiontOnProjectionFromCamera(Vector3 pos, float offset=0f) {
        Camera cam = SceneView.lastActiveSceneView.camera;

        if (cam != null) {
            Vector3 screenPos = cam.WorldToScreenPoint(pos);
            Ray ray = cam.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<31)) {
                Vector3 dir = (hit.transform.position - hit.point).normalized;
                return hit.point + dir*offset;
            }
            else {
                return pos;
            }
        }
        else {
            return pos;
        }
    }

    // for removing default handle
    Tool LastTool = Tool.None;
    void OnEnable() {
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable() {
        Tools.current = LastTool;
    }
}
