using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ArtPlugins.MapConfig
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapPointISO))]
    public class MapPointISOEditor : Editor {

        private void OnSceneGUI()
        {



            MapPointISO mcp = target as MapPointISO;
                if (mcp == null)
                {
                   
                    return;
                }
            Handles.color = Color.blue;
            var oldSkin_alignment = GUI.skin.label.alignment;
            var oldSkin_fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = 32;

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
          
                    int finalID = mcp.PointID;
                
                        Handles.Label(mcp.transform.position, finalID + "", GUI.skin.label);
 
            
            GUI.skin.label.fontSize = oldSkin_fontSize;
            GUI.skin.label.alignment = oldSkin_alignment;


        }
    }

}