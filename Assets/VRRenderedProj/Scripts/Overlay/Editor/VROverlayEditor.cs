using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VROverlay))]
[CanEditMultipleObjects]
public class VROverlayEditor : Editor {

    bool showOffset = false;

    void OnSceneGUI() {
        VROverlay overlay = (VROverlay)target;

        // Position
        Vector3 pos = Handles.FreeMoveHandle(overlay.transform.position, Quaternion.identity, .01f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
        Quaternion rot = overlay.transform.rotation;
        if (pos != overlay.transform.position) {
            pos = findPositiontOnProjectionFromCamera(pos, out rot).normalized * overlay.distance;
            overlay.transform.position = pos;
            overlay.transform.LookAt(Vector3.zero);
            overlay.transform.Rotate(new Vector3(0, 180, 0));
        }

        // Line end on Projection
        Vector3 posOnProj = Handles.FreeMoveHandle(overlay.positionOnProjection, Quaternion.identity, .01f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
        if (posOnProj != overlay.positionOnProjection) {
            posOnProj = findPositiontOnProjectionFromCamera(posOnProj, out rot);
            overlay.positionOnProjection = posOnProj;
        }

        // Offset
        Vector3 offset = overlay.transform.TransformPoint(overlay.offsetOnSelf);
        /*if (!showOffset) {
            showOffset = Handles.Button(offset, overlay.transform.rotation, 0.01f, 0.01f, Handles.CircleCap);
        }
        else {
            offset = Handles.PositionHandle(offset, overlay.transform.rotation);
        }*/
        offset = Handles.FreeMoveHandle(offset, Quaternion.identity, .003f, new Vector3(.5f, .5f, .5f), Handles.CircleCap);


        // Snap Offset
        offset = overlay.transform.InverseTransformPoint(offset);
        offset.z = 0;

        float snapOffset = 20;

        RectTransform rtransform = overlay.GetComponent<RectTransform>();
        float xMax = rtransform.rect.xMax;
        float xMin = rtransform.rect.xMin;
        if (offset.x > xMax - snapOffset && offset.x < xMax + snapOffset)
            offset.x = xMax;
        if (offset.x > xMin - snapOffset && offset.x < xMin + snapOffset)
            offset.x = xMin;

        float yMax = rtransform.rect.yMax;
        float yMin = rtransform.rect.yMin;
        if (offset.y > yMax - snapOffset && offset.y < yMax + snapOffset)
            offset.y = yMax;
        if (offset.y > yMin - snapOffset && offset.y < yMin + snapOffset)
            offset.y = yMin;

        Vector3 topLeft = new Vector3(xMin - snapOffset, yMax + snapOffset, 0);
        Vector3 topRight = new Vector3(xMax + snapOffset, yMax + snapOffset, 0);
        Vector3 bottomLeft = new Vector3(xMin - snapOffset, yMin - snapOffset, 0);
        Vector3 bottomRight = new Vector3(xMax + snapOffset, yMin - snapOffset, 0);

        topLeft = overlay.transform.TransformPoint(topLeft);
        topRight = overlay.transform.TransformPoint(topRight);
        bottomLeft = overlay.transform.TransformPoint(bottomLeft);
        bottomRight = overlay.transform.TransformPoint(bottomRight);

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topLeft, bottomLeft);

        Handles.DrawLine(bottomRight, topRight);
        Handles.DrawLine(bottomRight, bottomLeft);

        if (offset != overlay.offsetOnSelf) {
            overlay.offsetOnSelf = offset;
            overlay.UpdateLine();
        }

        overlay.UpdateLine();

        // draw line to projection sphere
        Handles.DrawLine(overlay.transform.position, findPositiontOnPRojectionFromSelf(overlay));

        EditorUtility.SetDirty(target);

        Event e = Event.current;
        switch (e.type) {
            case EventType.keyDown: {
                if (Event.current.keyCode == (KeyCode.K)) {
                    //Debug.Log("Key");
                    //SceneView.lastActiveSceneView.camera.transform.position = Vector3.zero;
                    SceneView.lastActiveSceneView.pivot = Vector3.zero;
                    SceneView.lastActiveSceneView.Repaint();
                }
                break;
            }
        }
    }
    
    public override void OnInspectorGUI() {
        if (GUILayout.Button("UPDATE")) {
            Object[] overlays = targets;

            foreach(Object overlayObject in overlays) {
                VROverlay overlay = overlayObject as VROverlay;

                Vector3 newPos = findPositiontOnPRojectionFromSelf(overlay).normalized * overlay.distance;
                overlay.transform.position = newPos;
                overlay.transform.LookAt(Vector3.zero);
                overlay.transform.Rotate(new Vector3(0, 180, 0));
                overlay.UpdateLine();
            }

        }

        VROverlay overlay_ = target as VROverlay;
        RectTransform rtransform = overlay_.GetComponent<RectTransform>();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<")) {
            SetOffset(new Vector2(rtransform.rect.xMin, 0));
        }
        if (GUILayout.Button("^")) {
            SetOffset(new Vector2(0, rtransform.rect.yMax));
        }
        if (GUILayout.Button("v")) {
            SetOffset(new Vector2(0, rtransform.rect.yMin));
        }
        if (GUILayout.Button(">")) {
            SetOffset(new Vector2(rtransform.rect.xMax, 0));
        }

        if (GUILayout.Button("RESET ON PROJECTION")) {
            overlay_.positionOnProjection = overlay_.transform.position+new Vector3(0,0.1f,0);
        }

        GUILayout.EndHorizontal();

        this.DrawDefaultInspector();
    }

    void SetOffset(Vector2 offset) {
        VROverlay overlay = target as VROverlay;
        overlay.offsetOnSelf = new Vector3(offset.x, offset.y, 0);
        overlay.UpdateLine();
    }

    Vector3 findPositiontOnProjectionFromCamera(Vector3 pos, out Quaternion rot) {
        Camera cam = SceneView.lastActiveSceneView.camera;

        if (cam != null) {
            Vector3 screenPos = cam.WorldToScreenPoint(pos);
            Ray ray = cam.ScreenPointToRay(screenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                rot = Quaternion.LookRotation(hit.normal);
                return hit.point;
            }
            else {
                rot = Quaternion.identity;
                return pos;
            }
        }
        else {
            Debug.Log("cam is nul");
            rot = Quaternion.identity;
            return pos;
        }
    }

    Vector3 findPositiontOnPRojectionFromSelf(VROverlay overlay) {
        RaycastHit hit;
        if (Physics.Raycast(overlay.transform.position, overlay.transform.position.normalized, out hit, 100.0f)) {
            return hit.point;
        }
        else {
            return overlay.transform.position;
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
