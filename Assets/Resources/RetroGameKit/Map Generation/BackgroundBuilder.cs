using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundBuilder : MonoBehaviour {

    protected static GameObject BackgroundContainer;

    public static string AreaBackgroundsFolder = "Maps/Area Backgrounds/";

    static public void BuildBackgrounds(List<AreaBackground> BackgroundLayers, List<AreaForeground> ForegroundLayers){
        BackgroundContainer = new GameObject("BackgroundContainer");
        BackgroundContainer.transform.position = new Vector3(0,0,0);

        foreach(AreaBackground BackgroundLayer in BackgroundLayers){
            int CurrentLayerCount = 0;
            BackgroundProperties CurrentLayerProperties = BackgroundPropertiesReader.GetBackgroundProperties(AreaBackgroundsFolder + BackgroundLayer.Name);

            foreach(BackgroundStrip Strip in CurrentLayerProperties.BackgroundStrips){
                GameObject NewBackgroundLayer = new GameObject("BackgroundLayer");
                NewBackgroundLayer.transform.parent = BackgroundContainer.transform;

                for(int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        GameObject NewBackgroundPiece = new GameObject("BackgroundPiece");
                        SpriteRenderer renderer = NewBackgroundPiece.AddComponent<SpriteRenderer>();
                        renderer.sprite = Resources.Load<Sprite>(AreaBackgroundsFolder + Strip.Image);
                        NewBackgroundPiece.transform.parent = NewBackgroundLayer.transform;
                        NewBackgroundPiece.transform.localPosition = new Vector3(renderer.bounds.size.x * i, renderer.bounds.size.y * j, 50 - CurrentLayerCount);
                    }
                }
                CurrentLayerCount++;
            }
        }

    }
}
