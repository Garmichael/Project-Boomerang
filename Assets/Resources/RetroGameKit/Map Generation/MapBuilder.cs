using UnityEngine;
using System.Collections.Generic;
using System;

public class MapBuilder : MonoBehaviour {

    public static MapProperties mapProperties;
    public static Dictionary<string, List<string>> TilesetCollisions = new Dictionary<string, List<string>>();
    public static List<MapLayer> MapLayers = new List<MapLayer>();
    public static List<MapView> MapViews = new List<MapView>();
    public static GameObject MapContainer;
    static string MapDataFolder = "Maps/Map Data/";
    static string TilesetDataFolder = "Maps/Tilesheets/";
    static Sprite[] AllTiles;
    static string MapPath;
    static string MapName;
    static float X;
    static float Y;

    static public void BuildMap(string mapName, float x, float y){
        MapPath = MapDataFolder + mapName;
        MapName = mapName;
        mapProperties = MapPropertiesReader.GetMapProperties(MapPath);
        MapLayers = mapProperties.Layers;
        MapViews = mapProperties.Views;
        X = x;
        Y = y;

        CreateMapContainer();

        BuildMapViews();
        BuildMapLayers();
    }

    static void BuildMapViews(){
        GameObject NewView;
        int ViewCount = 0;

        foreach (MapView View in MapViews) {

            NewView = GameObject.CreatePrimitive(PrimitiveType.Cube);

            NewView.name = MapName + "_View_" + ViewCount;
            NewView.transform.parent = MapContainer.transform;
            NewView.transform.localPosition = new Vector3(X + View.XStart + (View.XEnd/2), Y + View.YStart + (View.YEnd/2), 0);
            NewView.transform.localScale = new Vector3(View.XEnd, View.YEnd, 20);

            NewView.AddComponent<MapViewController>();
            ViewCount++;
        }
    }

    static void BuildMapLayers() {
        GameObject ThisLayer;

        foreach (MapLayer Layer in MapLayers){

            Debug.Log(Layer.LayerId);

            if(MapContainer.transform.Find("MapLayer_" + Layer.LayerId) != null) {
                ThisLayer = MapContainer.transform.Find("MapLayer_" + Layer.LayerId).gameObject;
            } else {
                ThisLayer = new GameObject("MapLayer_" + Layer.LayerId);
                ThisLayer.transform.parent = MapContainer.transform;
                ThisLayer.transform.localPosition = new Vector3(0, 0, Layer.LayerId);
            }

            AllTiles = Resources.LoadAll<Sprite>(TilesetDataFolder + Layer.Tilesheet);
            TilesetCollisions = MapPropertiesReader.GetCSVProperties(TilesetDataFolder + Layer.Tilesheet + "_collisions");

            TossTilesheetErrors(Layer.Tilesheet);
            BuildLayer(ThisLayer, Layer);
        }
    }

    static void CreateMapContainer(){
        MapContainer = GameObject.Find("MapContainer");
        if(MapContainer == null){
            MapContainer = new GameObject();
            MapContainer.name = "MapContainer";
            MapContainer.transform.position = new Vector3(0, 0, 0);
        }
    }

    static void BuildLayer(GameObject container, MapLayer layer){
        List<List<string>> TextMap;
        GameObject currentTile;

        TextMap = MapPropertiesReader.GetTextMap(MapPath + "_layer_" + layer.Name);

        TextMap.Reverse();

        int posX = 0;
        int posY = 0;

        foreach(List<string> mapRow in TextMap){
            posX = 0;

            foreach(string mapCel in mapRow){
                currentTile = BuildTile(mapCel, layer);

                if(currentTile != null){
                    currentTile.transform.parent = container.transform;
                    currentTile.transform.localPosition = new Vector3(X + posX, Y + posY, 0);
                    currentTile.GetComponent<SpriteRenderer>().sortingLayerName = "map_" + layer;
                }

                posX++;
            }

            posY++;
        }
    }

    static GameObject BuildTile(string mapCel, MapLayer layer){
        GameObject NewTile;
        Sprite NewTileSprite;
        int TileId;

        int.TryParse(mapCel, out TileId);

        if(TileId != null && TileId != 0){
            NewTile = Resources.Load("RetroGameKit/Tile Prefabs/BlankTile", typeof(GameObject)) as GameObject;
            NewTile = Instantiate(NewTile, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            NewTileSprite = AllTiles[TileId];
            NewTile.GetComponent<SpriteRenderer>().sprite = NewTileSprite;
            if(layer.UseColliders){
                AddCollider(NewTile, mapCel);
            }
        } else {
            NewTile = null;
        }

        return NewTile;
    }

    static GameObject GetResource(string prefabName){
        return Resources.Load("RetroGameKit/Tile Prefabs/" + prefabName, typeof(GameObject)) as GameObject;
    }

    static void BuildCollider(string mapCel, string tileType, GameObject NewTile, GameObject Resource, Vector3 position, Quaternion rotation){
        if(TileIs(mapCel, tileType)) {
            GameObject Collider = Instantiate(Resource, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            Collider.transform.parent = NewTile.transform;
            Collider.transform.localPosition = position;
            Collider.transform.rotation = rotation;
            Collider.transform.localScale = new Vector3(1,1,20);
            SetColliderProperty(Collider, mapCel);

            if(!EngineProperties.DebugShowColliders) {
                Collider.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    static void SetColliderProperty(GameObject collider, string mapCel){
        if(TileIs(mapCel, "solid")){
            collider.layer = LayerMask.NameToLayer("Solid");
        }
        if(TileIs(mapCel, "solidOnTop")){
            collider.layer = LayerMask.NameToLayer("SolidOnTop");
        }
    }

    static bool TileIs(string mapCel, string property){
        return TilesetCollisions.ContainsKey(property) && TilesetCollisions[property].Contains(mapCel);
    }

    static void AddCollider(GameObject NewTile, string mapCel){
        BuildCollider(mapCel, "square", NewTile, GetResource("Collision_square"), new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));

        BuildCollider(mapCel, "elbow_bottomleft", NewTile, GetResource("Collision_elbow"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "elbow_bottomright", NewTile, GetResource("Collision_elbow"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "elbow_topleft", NewTile, GetResource("Collision_elbow"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "elbow_topright", NewTile, GetResource("Collision_elbow"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "quarter_bottomleft", NewTile, GetResource("Collision_quarter"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "quarter_bottomright", NewTile, GetResource("Collision_quarter"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "quarter_topleft", NewTile, GetResource("Collision_quarter"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "quarter_topright", NewTile, GetResource("Collision_quarter"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "half_bottom", NewTile, GetResource("Collision_half"), new Vector3(1, 0.5f, 0), Quaternion.Euler(0,0,90));
        BuildCollider(mapCel, "half_top", NewTile, GetResource("Collision_half"), new Vector3(1, 1, 0), Quaternion.Euler(0,0,90));
        BuildCollider(mapCel, "half_left", NewTile, GetResource("Collision_half"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "half_right", NewTile, GetResource("Collision_half"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));

        BuildCollider(mapCel, "slope_1x1_surface_ground_right", NewTile, GetResource("Collision_slope_1x1_surface"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x1_surface_ground_left", NewTile, GetResource("Collision_slope_1x1_surface"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x1_notch_ground_right", NewTile, GetResource("Collision_slope_1x1_notch"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x1_notch_ground_left", NewTile, GetResource("Collision_slope_1x1_notch"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x1_surface_ceiling_right", NewTile, GetResource("Collision_slope_1x1_surface"), new Vector3(1, 1, 0), Quaternion.Euler(0,0,90));
        BuildCollider(mapCel, "slope_1x1_surface_ceiling_left", NewTile, GetResource("Collision_slope_1x1_surface"), new Vector3(0, 1, 0), Quaternion.Euler(0,180,90));
        BuildCollider(mapCel, "slope_1x1_notch_ceiling_right", NewTile, GetResource("Collision_slope_1x1_notch"), new Vector3(1, 1, 0), Quaternion.Euler(0,0,90));
        BuildCollider(mapCel, "slope_1x1_notch_ceiling_left", NewTile, GetResource("Collision_slope_1x1_notch"), new Vector3(0, 1, 0), Quaternion.Euler(0,180,90));

        BuildCollider(mapCel, "slope_1x2_surface_ground_right_a", NewTile, GetResource("Collision_slope_1x2_surface_a"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x2_surface_ground_right_b", NewTile, GetResource("Collision_slope_1x2_surface_b"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x2_notch_ground_right", NewTile, GetResource("Collision_slope_1x2_notch"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x2_surface_ground_left_a", NewTile, GetResource("Collision_slope_1x2_surface_a"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x2_surface_ground_left_b", NewTile, GetResource("Collision_slope_1x2_surface_b"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x2_notch_ground_left", NewTile, GetResource("Collision_slope_1x2_notch"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x2_surface_ceiling_right_a", NewTile, GetResource("Collision_slope_1x2_surface_a"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x2_surface_ceiling_right_b", NewTile, GetResource("Collision_slope_1x2_surface_b"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x2_surface_ceiling_left_a", NewTile, GetResource("Collision_slope_1x2_surface_a"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x2_surface_ceiling_left_b", NewTile, GetResource("Collision_slope_1x2_surface_b"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x2_notch_ceiling_right", NewTile, GetResource("Collision_slope_1x2_notch"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x2_notch_ceiling_left", NewTile, GetResource("Collision_slope_1x2_notch"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));

        BuildCollider(mapCel, "slope_1x3_surface_ground_right_c", NewTile, GetResource("Collision_slope_1x3_surface_c"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x3_surface_ground_right_b", NewTile, GetResource("Collision_slope_1x3_surface_b"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x3_surface_ground_right_a", NewTile, GetResource("Collision_slope_1x3_surface_a"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x3_surface_ground_left_c", NewTile, GetResource("Collision_slope_1x3_surface_c"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x3_surface_ground_left_b", NewTile, GetResource("Collision_slope_1x3_surface_b"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x3_surface_ground_left_a", NewTile, GetResource("Collision_slope_1x3_surface_a"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x3_notch_ground_right", NewTile, GetResource("Collision_slope_1x3_notch"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x3_notch_ground_left", NewTile, GetResource("Collision_slope_1x3_notch"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_right_c", NewTile, GetResource("Collision_slope_1x3_surface_c"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_right_b", NewTile, GetResource("Collision_slope_1x3_surface_b"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_right_a", NewTile, GetResource("Collision_slope_1x3_surface_a"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_left_c", NewTile, GetResource("Collision_slope_1x3_surface_c"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_left_b", NewTile, GetResource("Collision_slope_1x3_surface_b"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x3_surface_ceiling_left_a", NewTile, GetResource("Collision_slope_1x3_surface_a"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x3_notch_ceiling_right", NewTile, GetResource("Collision_slope_1x3_notch"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x3_notch_ceiling_left", NewTile, GetResource("Collision_slope_1x3_notch"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));

        BuildCollider(mapCel, "slope_1x4_surface_ground_right_d", NewTile, GetResource("Collision_slope_1x4_surface_d"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_right_c", NewTile, GetResource("Collision_slope_1x4_surface_c"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_right_b", NewTile, GetResource("Collision_slope_1x4_surface_b"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_right_a", NewTile, GetResource("Collision_slope_1x4_surface_a"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_left_d", NewTile, GetResource("Collision_slope_1x4_surface_d"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_left_c", NewTile, GetResource("Collision_slope_1x4_surface_c"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_left_b", NewTile, GetResource("Collision_slope_1x4_surface_b"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x4_surface_ground_left_a", NewTile, GetResource("Collision_slope_1x4_surface_a"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x4_notch_ground_right", NewTile, GetResource("Collision_slope_1x4_notch"), new Vector3(1, 0, 0), Quaternion.Euler(0,0,0));
        BuildCollider(mapCel, "slope_1x4_notch_ground_left", NewTile, GetResource("Collision_slope_1x4_notch"), new Vector3(0, 0, 0), Quaternion.Euler(0,180,0));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_right_d", NewTile, GetResource("Collision_slope_1x4_surface_d"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_right_c", NewTile, GetResource("Collision_slope_1x4_surface_c"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_right_b", NewTile, GetResource("Collision_slope_1x4_surface_b"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_right_a", NewTile, GetResource("Collision_slope_1x4_surface_a"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_left_d", NewTile, GetResource("Collision_slope_1x4_surface_d"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_left_c", NewTile, GetResource("Collision_slope_1x4_surface_c"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_left_b", NewTile, GetResource("Collision_slope_1x4_surface_b"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x4_surface_ceiling_left_a", NewTile, GetResource("Collision_slope_1x4_surface_a"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
        BuildCollider(mapCel, "slope_1x4_notch_ceiling_right", NewTile, GetResource("Collision_slope_1x4_notch"), new Vector3(1, 1, 0), Quaternion.Euler(0,180,180));
        BuildCollider(mapCel, "slope_1x4_notch_ceiling_left", NewTile, GetResource("Collision_slope_1x4_notch"), new Vector3(0, 1, 0), Quaternion.Euler(0,0,180));
    }

    static void TossTilesheetErrors(string imageName){
        if(AllTiles.Length == 0){
            ErrorLogger.Log("Could not find image for tilesheet: " + imageName, "Ensure that an image by the correct name exists in the correct directory");
        }

        if(AllTiles.Length == 1){
            ErrorLogger.Log("Image for tilesheet has not been sliced properly: " + imageName, "In the IDE, edit the properties of the image to set Sprite Mode to Multiple, then slice the sprite.");
        }
    }
}
