using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// This class saves the realtime (gesture)controller points
/// </summary>
public class GesturePoints {

    public float m_iDistanceToLast;
    public float m_iAngleToLast;
    public GameObject m_cCurrentAreaPrimitive;
    public Vector3 m_vPosition;

    //Constructor
    public GesturePoints()
    {
    }

}


