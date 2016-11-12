using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VRSphereProjection : MonoBehaviour {
    public VRRenderedObject vrImage;

    public GameObject leftSphere;
    public GameObject rightSphere;

    public bool swapViews = false;

    public Material leftMaterial;
    public Material rightMaterial;

    bool inited = false;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if(leftMaterial==null)
            leftMaterial = new Material(Shader.Find("Unlit/Texture"));
        if (rightMaterial == null)
            rightMaterial = new Material(Shader.Find("Unlit/Texture"));

        leftSphere.GetComponent<Renderer>().material = leftMaterial;
        rightSphere.GetComponent<Renderer>().material = rightMaterial;

        if (vrImage != null)
            SetVRImage(vrImage);

        inited = true;
    }

    public void SetVRImage(VRRenderedObject vrImage)
    {
        if (leftMaterial == null && rightMaterial == null) return;
        if (swapViews)
        {
            leftMaterial.mainTexture = vrImage.rightView;
            rightMaterial.mainTexture = vrImage.leftView;
        }
        else
        {
            leftMaterial.mainTexture = vrImage.leftView;
            rightMaterial.mainTexture = vrImage.rightView;
        }
    }
}
