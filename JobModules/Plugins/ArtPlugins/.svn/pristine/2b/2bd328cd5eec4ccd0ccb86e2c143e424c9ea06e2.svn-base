using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace ArtPlugins {

    public class MultiTagManager : EditorWindow
    {
        private static Vector2 offset = new Vector2(0, 2);
        public static Color gameObjectFontColor = Color.black;
        public static Color prefabOrgFontColor = Color.black;
        public static Color prefabModFontColor = Color.white;
        public static Color inActiveColor = new Color(0.01f, 0.4f, 0.25f);
        public static int[] seachModeMaks = new int[6];
        [MenuItem("Window/MultiTagManager")]
        private static void init() {
            GetWindow<MultiTagManager>();


        }
        private void OnEnable()
        {
           // EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyWindowItemOnGUI;
          //  EditorApplication.hierarchyWindowItemOnGUI = HandleHierarchyWindowItemOnGUI;
             

        }
        
            private void OnGUI()
        {
             int fontSize = 14;
            // EditorGUILayout.ObjectField(GUI.skin,typeof(GUISkin),false) as GUISkin;
            GUI.skin.button.fontSize = fontSize;
         //   GUI.skin.toggle.fontSize = fontSize;
            GUI.skin.box.fontSize = fontSize;
            GUI.skin.box.padding =new RectOffset(-2,-2,-2,-2);
       //     GUI.skin.window.fontSize = fontSize;
        //    GUI.skin.verticalScrollbarDownButton.fontSize = fontSize;
         //   GUI.skin.scrollView.fontSize = fontSize;
         //   GUI.skin.textField.fontSize = fontSize;
        //    GUI.skin.textArea.fontSize = fontSize;
          //  GUI.skin.horizontalSlider.fontSize = fontSize;
            GUILayout.Label("快速查找:");
            for (int i = 0; i < (int)MultiTag.TagEnum.Max; i++)
            {
              
                if (GUILayout.Button(MultiTag.TagEnumName[i]))
                {
                    List<GameObject> sels = new List<GameObject>();
                    foreach (var mt in FindObjectsOfType<MultiTag>())
                    {
                        if (mt.btags[i])
                        {
                            sels.Add(mt.gameObject);
                        }
                    }
                    Selection.objects = sels.ToArray();
                }
            }
            GUILayout.Label("高级查找:");


            for (int i = 0; i < 3; i++)
            {
                GUILayout.BeginHorizontal();
                 GUILayout.Label("属于:");
                    seachModeMaks[i*2]= EditorGUILayout.MaskField(seachModeMaks[i * 2], MultiTag.TagEnumName, GUI.skin.box);
                GUILayout.Label("但不属于:");
                seachModeMaks[i * 2 + 1] = EditorGUILayout.MaskField(seachModeMaks[i * 2 + 1], MultiTag.TagEnumName, GUI.skin.box);
                GUILayout.EndHorizontal();
                if(i!=2)
                GUILayout.Label("或者:");
            }

            if (GUILayout.Button("查找")) {


                 List<GameObject> sels = new List<GameObject>();
                foreach (var mt in FindObjectsOfType<MultiTag>())
                {
                    for (int i = 0; i < seachModeMaks.Length/2; i++)
                    {
                        if (include(mt.tagMask, seachModeMaks[i*2]) && !include(mt.tagMask, seachModeMaks[i*2+1]))
                        {
                            sels.Add(mt.gameObject);
                        }
                    }
                  
                }
                 Selection.objects = sels.ToArray();
            }
            if (GUILayout.Button("还原"))
            {
                for (int i = 0; i < seachModeMaks.Length; i++)
                {
                    seachModeMaks[i]= 0;
                }
            }

            }

        private bool include(int checkFlag, int allAllow)
        {
            
            return (checkFlag & allAllow) != 0;
        }
 
    }
}