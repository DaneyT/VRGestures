using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System;

//[XmlRoot("GestureCollection")]
[XmlRoot("ArrayOfGestureObject")]
public class GestureContainer
{
    //Bron:http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer

    [XmlArray("Gestures")]
    [XmlArrayItem("Gesture")]
    public List<GestureObject> Gestures = new List<GestureObject>();

    [XmlIgnoreAttribute]
    private int m_iGestureAmountWriten = 0;
    //Serlization bron: https://msdn.microsoft.com/en-us/library/10y9yyta.aspx
    /// <summary>
    /// Serializes an object, in this case it is an: List<GestureObject>
    /// </summary>
    /// <param name="filename">The path in which this document will be saved</param>
    /// <param name="headelement"/>The name of the dictionary entry</param>
    public void SerializeObject(Dictionary<string, List<GestureObject>> gestureDict, string filename)
    {
        Debug.Log("Writing With XmlTextWriter");

        FileInfo fInfo = new FileInfo(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");

        bool append = false;

        var serializer = new XmlSerializer(typeof(List<GestureObject>));
        var settings = new XmlWriterSettings();
        XmlSerializerNamespaces emptyNamepsaces  = new XmlSerializerNamespaces();

        if (fInfo.Exists)
        {
            append = true;
            Debug.Log("Appending");
            emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
        }
        else
        {
            Debug.Log("Create new file");
            settings.Indent = true;
            m_iGestureAmountWriten = 0; //reset "list"counter because we started a new document
        }

        using (var stream = new StreamWriter(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml", append, Encoding.UTF8))
        using (var writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartElement("GestureCollection");  //write a Root header which will contain all gestures
            foreach(KeyValuePair<string, List<GestureObject>> pair in gestureDict)
            {
                writer.WriteStartElement(pair.Key);
                serializer.Serialize(writer, pair.Value, emptyNamepsaces);
                writer.WriteEndElement();
                writer.WriteWhitespace("  ");
            }
            writer.WriteEndElement();
            writer.Close();
        }
    }

    /// <summary>
    /// Deserializes an object which will return the XML file data into a list
    /// </summary>
    /// <returns>Returns a list of gestureobjects which must be saved in a List<GestureObject></returns>
    public Dictionary<String,List<GestureObject>> DeSerializeObject(string path)
    {

        //Create a dictionary which will be returned at the end
        Dictionary<String, List<GestureObject>> GestureDict = new Dictionary<String, List<GestureObject>>();

        XDocument xdocu = XDocument.Load(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");

        //Get total gesture count
        List<XElement> xmlValues = xdocu.Elements().Elements().ToList();


        for (int i = 0; i < xmlValues.Count; i++)
        {
            //Create a empty list of GestureObjects
            List<GestureObject> GestureList = new List<GestureObject>();
            //XElement firstElement = xmlValues[i].Element().First();
            XElement firstElement = xmlValues[i];

            List<XElement> xmlEntries = xdocu.Descendants(firstElement.Name).Descendants("ArrayOfGestureObject").Descendants("GestureObject")
            .ToList();

            //ToDO: Comeup with a better way to get the XML data
            foreach (XElement xe in xmlEntries)
            {
                GestureObject gestureObj = new GestureObject();
                gestureObj.m_iAcceleration = Convert.ToSingle(xe.Element("Acceleration").Value);
                gestureObj.m_iSpeed = Convert.ToSingle(xe.Element("Acceleration").Value);
                gestureObj.m_vPosition = new Vector3(Convert.ToSingle(xe.Element("Position").Element("x").Value), Convert.ToSingle(xe.Element("Position").Element("y").Value), Convert.ToSingle(xe.Element("Position").Element("z").Value));
                gestureObj.m_vRotation = new Vector3(Convert.ToSingle(xe.Element("Rotation").Element("x").Value), Convert.ToSingle(xe.Element("Rotation").Element("y").Value), Convert.ToSingle(xe.Element("Rotation").Element("z").Value));

                GestureList.Add(gestureObj);
            }
            GestureDict.Add(firstElement.Name.ToString(), GestureList);
        }
        return GestureDict;
    }

}
