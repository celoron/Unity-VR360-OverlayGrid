using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MySphereMirror : MonoBehaviour {

    public bool swapOnStart = true;
    
	void Start () {
        if(swapOnStart)
            Swap();
	}

    void Swap()
    {
        Vector2[] vec2UVs = transform.GetComponent<MeshFilter>().mesh.uv;

        for (int i = 0; i < vec2UVs.Length; i++)
        {
            vec2UVs[i] = new Vector2(1.0f - vec2UVs[i].x, vec2UVs[i].y);
        }

        transform.GetComponent<MeshFilter>().mesh.uv = vec2UVs;
    }
}
