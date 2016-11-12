using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VRRendered", menuName = "VR/VRRendered", order = 1)]
public class VRRenderedObject : ScriptableObject
{
    public Texture2D leftView;
    public Texture2D rightView;
}
