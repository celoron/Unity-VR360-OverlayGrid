using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR360Grid : MonoBehaviour {
    public float OffsetFromSphere = 0f;

    public List<Vector3> positions = new List<Vector3>();

    public int width= 4;
    public int height = 3;

    public bool ProjectOnSphere = true;

    public Mesh mesh = null;

    public void ClearAllPosition() {
        positions = new List<Vector3>();
    }
}
