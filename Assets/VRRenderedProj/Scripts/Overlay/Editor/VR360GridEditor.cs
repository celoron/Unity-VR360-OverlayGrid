using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VR360Grid))]
[CanEditMultipleObjects]
public class VR360GridEditor : Editor {

    bool showOffset = false;

    void OnSceneGUI() {
        VR360Grid grid = (VR360Grid)target;

        for (int i = 0; i < grid.positions.Count; i++) {
            Vector3 pos = grid.positions[i];
            Vector3 newPos = Handles.FreeMoveHandle(pos, Quaternion.identity, .03f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
            Handles.Label(pos, "Pos" + i);
            if (pos != newPos) {
                newPos = findPositiontOnProjectionFromCamera(newPos, grid.OffsetFromSphere);

                grid.positions[i] = newPos;
            }
        }

        if(grid.GetComponent<MeshFilter>()!=null)
            CreateGrid();

        if (grid.positions.Count == 4) {
            Handles.DrawLine(grid.positions[0], grid.positions[1]);
            Handles.DrawLine(grid.positions[1], grid.positions[2]);
            Handles.DrawLine(grid.positions[2], grid.positions[3]);
            Handles.DrawLine(grid.positions[3], grid.positions[0]);

            for(int i=0; i<grid.width+1; i++) {
                for(int j=0; j<grid.height+1; j++) {
                    Handles.DotCap(0, GetPos(i, j), Quaternion.identity, 0.01f);
                }
            }
        }
    }

    Vector3 GetPos(int i, int j) {
        VR360Grid grid = (VR360Grid)target;

        float hor = (i * 1.0f) / grid.width;
        float ver = (j * 1.0f) / grid.height;

        Vector3 A = grid.positions[0] + (grid.positions[1] - grid.positions[0]) * hor;
        Vector3 B = grid.positions[3] + (grid.positions[2] - grid.positions[3]) * hor;

        Vector3 P = A + (B - A) * ver;

        if (grid.ProjectOnSphere) {
            return findPositiontOnProjection(P, grid.OffsetFromSphere);
        }
        else {
            return P;
        }

    }

    public void CreateGrid(bool forceCreate= false) {
        VR360Grid grid = (VR360Grid)target;

        MeshFilter meshFilter = grid.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) {
            meshFilter = grid.gameObject.AddComponent<MeshFilter>();
            grid.gameObject.AddComponent<MeshRenderer>();
        }

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int w = grid.width + 1;
        int h = grid.height + 1;

        Vector3[] vertices = new Vector3[(w) * (h)];
        int[] tri = new int[(w-1) * (h-1) * 6];
        Vector3[] normals = new Vector3[(w) * (h)];
        Vector2[] uv = new Vector2[(w) * (h)];

        int triInd = 0;
        for (int j = 0; j < h; j++) {
            for (int i = 0; i < w; i++) {
                int ind = i + j * w;

                vertices[ind] = GetPos(i, j);
                normals[ind] = -Vector3.forward;
                uv[ind] = new Vector2((i * 1.0f) / (w-1) , 1.0f-(j * 1.0f) / (h-1));

                if (j < grid.height && i < grid.width) {

                    tri[triInd * 6 + 0] = i + j * w;
                    tri[triInd * 6 + 1] = (i + 1) + j * w;
                    tri[triInd * 6 + 2] = (i + 1) + (j + 1) * w;

                    tri[triInd * 6 + 3] = i + j * w;
                    tri[triInd * 6 + 4] = (i + 1) + (j + 1) * w;
                    tri[triInd * 6 + 5] = i + (j + 1) * w;

                    triInd++;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;
        
        /*uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);*/
    }

    public override void OnInspectorGUI() {
        VR360Grid grid = (VR360Grid)target;

        this.DrawDefaultInspector();

        for (int i = 0; i < grid.positions.Count; i++) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pos" + i);
            if (GUILayout.Button("X")) {
                grid.positions.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Position")) {
            Vector3 pos = new Vector3(0, 0, 0);
            if (grid.positions.Count > 0) {
                Vector3 last = grid.positions[grid.positions.Count - 1];
                pos = last;
            }
            grid.positions.Add(pos);
        }

        if (GUILayout.Button("Clear All Positions")) {
            grid.ClearAllPosition();
        }

        GUILayout.Label("Grid creating", EditorStyles.boldLabel);
        GUILayout.Label("Can Create: "+ (grid.positions.Count==4) );

        if (grid.positions.Count == 4 && GUILayout.Button("CreateGrid")) {
            CreateGrid(true);
        }
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

    Vector3 findPositiontOnProjection(Vector3 pos, float offset = 0f) {
        Ray ray = new Ray(Vector3.zero, pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 31)) {
            Vector3 dir = (hit.transform.position - hit.point).normalized;
            return hit.point + dir * offset;
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
