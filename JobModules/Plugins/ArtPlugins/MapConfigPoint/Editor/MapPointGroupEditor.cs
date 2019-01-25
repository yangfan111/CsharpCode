using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ArtPlugins.MapConfig
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapPointGroup))]
    public class MapPointGroupEditor : Editor {

        private void OnSceneGUI()
        {
   

           
                MapPointGroup mcp = target as MapPointGroup;
                if (mcp == null)
                {
                   
                    return;
                }
            Handles.color = Color.blue;
            var oldSkin_alignment = GUI.skin.label.alignment;
            var oldSkin_fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = 32;

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            foreach (Transform item in mcp.transform)
                {
                    int finalID = mcp.groupID * 100 + int.Parse(item.name);
                    foreach (Transform t in item.transform)
                        Handles.Label(t.position, finalID + "", GUI.skin.label);

                }
            
            GUI.skin.label.fontSize = oldSkin_fontSize;
            GUI.skin.label.alignment = oldSkin_alignment;


        }
    }

}