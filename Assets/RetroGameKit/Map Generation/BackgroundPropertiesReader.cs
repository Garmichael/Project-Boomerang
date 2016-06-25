
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class BackgroundPropertiesReader : MonoBehaviour {
    static public BackgroundProperties GetBackgroundProperties(string backgroundPath){
        return BackgroundProperties.Load(backgroundPath);
    }
}


public class BackgroundStrip {
    [XmlElement("Image")]
    public string Image;
}

[XmlRoot("BackgroundLayer")]
public class BackgroundProperties{

    [XmlArray("BackgroundStrips")]
    [XmlArrayItem("BackgroundStrip")]
    public List<BackgroundStrip> BackgroundStrips = new List<BackgroundStrip>();

    public static BackgroundProperties Load(string backgroundPath){
        TextAsset xml = Resources.Load<TextAsset>(backgroundPath);

        XmlSerializer serializer = new XmlSerializer(typeof(BackgroundProperties));

        StringReader reader = new StringReader(xml.text);

        BackgroundProperties properties = serializer.Deserialize(reader) as BackgroundProperties;

        reader.Close();

        return properties;

    }
}