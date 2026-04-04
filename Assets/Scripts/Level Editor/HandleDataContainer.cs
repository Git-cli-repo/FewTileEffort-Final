using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class HandleDataContainer : MonoBehaviour
{
    public HandleEdgeType edgeType;
    public enum HandleEdgeType
    {
        Top, 
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    
}