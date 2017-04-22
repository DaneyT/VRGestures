using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot("GestureCollection")]

public class GestureObject{

    [XmlElement("Acceleration")]
    public float m_iAcceleration;
    [XmlElement("Speed")]
    public float m_iSpeed;
    [XmlElement("Position")]
    public Vector3 m_vPosition;
    [XmlElement("Rotation")]
    public Vector3 m_vRotation;
    //Add starting area??

    [XmlIgnoreAttribute]
    public GameObject m_cGestureObjectPrimitive;

    //Constructor
    public GestureObject() 
    {
    }

}
