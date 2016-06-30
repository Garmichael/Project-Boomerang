
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class AreaPropertiesReader : MonoBehaviour {

    static public Dictionary<string, List<string>> GetCSVProperties(string mapPath){
        string csv = ((TextAsset)Resources.Load(mapPath, typeof(TextAsset))).text;

        csv = csv.Replace("\r", "");

        Dictionary<string, List<string>> finalDictionary = new Dictionary<string, List<string>>();

        List<string> propertySets = csv.Split("\n"[0]).ToList<string>();
        string[] properties;
        string[] values;
        List<string> propertyEntry = new List<string>();

        foreach(string propertySet in propertySets){
            propertyEntry = new List<string>();
            properties = propertySet.Split(":"[0]);
            values = properties[1].Split(","[0]);

            foreach(string value in values){
                propertyEntry.Add(value);
            }

            finalDictionary.Add(properties[0], propertyEntry);
        }
        return finalDictionary;
    }

    static public AreaProperties GetAreaProperties(string areaPath){
        return AreaProperties.Load(areaPath);
    }
}


public class AreaMap {
    [XmlElement("Name")]
    public string Name;

    [XmlElement("X-Start")]
    public float XStart;

    [XmlElement("Y-Start")]
    public float YStart;
}

public class AreaBackground{
    [XmlElement("Name")]
    public string Name;

    [XmlElement("X-Parallax")]
    public float XParallax;

    [XmlElement("Y-Parallax")]
    public float YParallax;
}

public class AreaForeground{
    [XmlElement("Name")]
    public string Name;

    [XmlElement("X-Parallax")]
    public float XParallax;

    [XmlElement("Y-Parallax")]
    public float YParallax;
}

[XmlRoot("Area")]
public class AreaProperties{

    [XmlArray("Maps")]
    [XmlArrayItem("Map")]
    public List<AreaMap> Maps = new List<AreaMap>();

    [XmlArray("Backgrounds")]
    [XmlArrayItem("BackgroundLayer")]
    public List<AreaBackground> BackgroundLayers = new List<AreaBackground>();

    [XmlArray("Foregrounds")]
    [XmlArrayItem("ForegroundLayer")]
    public List<AreaForeground> ForegroundLayers = new List<AreaForeground>();

    public static AreaProperties Load(string areaPath){
        TextAsset xml = Resources.Load<TextAsset>(areaPath);

        if(xml == null){
            ErrorLogger.Log("Area Xml not found: " + areaPath + ".xml", "When using AreaBuilder.BuildArea, make sure you have that name.xml file in your Area Data directory");
        }

        XmlSerializer serializer = new XmlSerializer(typeof(AreaProperties));

        StringReader reader = new StringReader(xml.text);

        AreaProperties properties = serializer.Deserialize(reader) as AreaProperties;

        reader.Close();

        return properties;

    }
}