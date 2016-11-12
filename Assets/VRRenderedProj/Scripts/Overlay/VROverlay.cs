using UnityEngine;
using System.Collections;

public class VROverlay : MonoBehaviour {

    public Vector3 positionOnProjection;
    public Vector3 offsetOnSelf;

    LineRenderer line;

    public float distance= 0.6f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void UpdateLine() { 
        if(Line() != null)
        {
            line.SetPosition(0, transform.TransformPoint(offsetOnSelf));
            line.SetPosition(1, positionOnProjection);
        }
    }

    public LineRenderer Line()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();
        return line;
    }
}
