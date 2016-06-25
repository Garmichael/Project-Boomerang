using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaBuilder : MonoBehaviour {

    public static string AreasFolder = "Maps/Area Data/";
    public static string BackgroundsFolder = "Maps/Area Backgrounds/";
    public static AreaProperties Area;

    static public void BuildArea(string area){
        Area = AreaPropertiesReader.GetAreaProperties(AreasFolder + area);

        BuildMaps();
//        BuildBackgrounds();
    }

    static void BuildMaps(){

        foreach(AreaMap Maps in Area.Maps){
            MapBuilder.BuildMap(Maps.Name, Maps.XStart, Maps.YStart);
        }

    }

    static void BuildBackgrounds(){
        BackgroundBuilder.BuildBackgrounds(Area.BackgroundLayers, Area.ForegroundLayers);
    }
}
