
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

public class MapPropertiesReader : MonoBehaviour {

	static public Dictionary<string, List<string>> GetCSVProperties(string mapPath){
        string csv = "";

        TextAsset textAsset = Resources.Load<TextAsset>(mapPath);

        if(textAsset != null){
		    csv = textAsset.text;
        } else {
            ErrorLogger.Log("Could not find collision data for tilesheet: " + mapPath + ".txt", "Check your map's _property file and make sure you have the _collisions.txt file in the TileSheets directory");
        }

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

    static public List<List<string>> GetTextMap(string mapPath){

        string csv = ((TextAsset)Resources.Load(mapPath, typeof(TextAsset))).text;
        csv = csv.Replace("\r", "");

        List<List<string>> finalArray = new List<List<string>>();

        List<string> rows = csv.Split("\n"[0]).ToList<string>();

        foreach(string row in rows){
            if(row != ""){
                finalArray.Add(row.Split(","[0]).ToList<string>());
            }
        }

        return finalArray;
    }

    static public MapProperties GetMapProperties(string mapPath){
        return MapProperties.Load(mapPath);
    }
}


public class MapLayer {
    [XmlElement("Name")]
    public string Name;

    [XmlElement("LayerId")]
    public float LayerId;

    [XmlElement("Tilesheet")]
    public string Tilesheet;

    [XmlElement("UseColliders")]
    public bool UseColliders;
}

public class MapView {
    [XmlElement("Camera-Transition-Mode")]
    public string CameraTransitionMode;

    [XmlElement("X-Start")]
    public float XStart;

    [XmlElement("X-End")]
    public float XEnd;

    [XmlElement("Y-Start")]
    public float YStart;

    [XmlElement("Y-End")]
    public float YEnd;
}

[XmlRoot("Map")]
public class MapProperties{

    [XmlArray("Layers")]
    [XmlArrayItem("Layer")]
    public List<MapLayer> Layers = new List<MapLayer>();

    [XmlArray("Views")]
    [XmlArrayItem("View")]
    public List<MapView> Views = new List<MapView>();

    public static MapProperties Load(string mapPath){
        TextAsset xml = Resources.Load<TextAsset>(mapPath + "_properties");

        if(xml == null){
            ErrorLogger.Log("Map Properties file not found: " + mapPath+"_properties.xml", "When referencing maps in your area xml file, make sure that same name exists in the map data directory");
        }
        XmlSerializer serializer = new XmlSerializer(typeof(MapProperties));

        StringReader reader = new StringReader(xml.text);

        MapProperties properties = serializer.Deserialize(reader) as MapProperties;

        reader.Close();

        return properties;

    }
}