using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace ArtPlugins
{
   
    public class MipmapsTools : EditorWindow 
    {
        [MenuItem("Window/MipmapsTools")]
        public static void init() {
            GetWindow<MipmapsTools>();
        }
        private const  string info= "这个工具用来设置 贴图的mipmaps bias，含义是远离镜头时降分辨率 渲染的时机 默认为-1(和0一样),负数表示远一点再降，清晰但性能低，正数表示近一点就降，模糊但性能高,用 -0.5，0.5等测试后 决定选用，具体性能影响将来补上说明";
        private int filterMode = 0;
        private float bias=-1;
        private  List<string> findRst;
        private void OnGUI()
        {
            for (int i = 0; i < info.Length; i+=60)
            {
                EditorGUILayout.LabelField(info.Substring(i,Mathf.Min(60,info.Length-i)));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("设置选定贴图mip maps ，只修改有勾选 mipmaps 的贴图，可以连法线等一起选中");
            //   EditorGUILayout.BeginHorizontal();
            filterMode= GUILayout.Toolbar(filterMode,new string[]{ "box","kaiser"});

            bias= EditorGUILayout.Slider("bias",bias, -5, 5);

            // EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("设置所选")) {
                
                foreach (var item in Selection.objects)
                {
                    if (item == null || item is Texture == false) continue;
                    var path = AssetDatabase.GetAssetPath(item);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter == null|| textureImporter.mipmapEnabled==false) continue;
                    var texture = item as Texture;
                    Debug.Log(textureImporter.mipMapBias);
                    textureImporter.mipMapBias = bias;
                    textureImporter.mipmapFilter = filterMode==0?TextureImporterMipFilter.BoxFilter: TextureImporterMipFilter.KaiserFilter;
                    AssetDatabase.ImportAsset(path);


                }
            };

            if (GUILayout.Button("统计所选")) {
                int disableMipsCount = 0;
                int boxCount = 0;
                int kaiserCount = 0;
                Dictionary<float, int> biasCount=new Dictionary<float, int>() ;
                foreach (var item in Selection.objects)
                {
                    if (item == null || item is Texture == false) continue;
                    var path = AssetDatabase.GetAssetPath(item);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter == null) continue;
                    if (textureImporter.mipmapEnabled)
                    {
                        if (textureImporter.mipmapFilter == TextureImporterMipFilter.BoxFilter) boxCount++; else kaiserCount++;
                        int bcount = 0;
                        if (biasCount.ContainsKey(textureImporter.mipMapBias))
                        {
                            bcount = biasCount[textureImporter.mipMapBias];
                        }
                        bcount++;
                        biasCount[textureImporter.mipMapBias] = bcount;


                    }
                    else {
                        disableMipsCount++;
                    }

                }
                findRst = new List<string>();
                findRst.Add("没设置 mipmaps 数量:"+ disableMipsCount);
                findRst.Add("box模式数量:"+ boxCount);
                findRst.Add("kaiser模式数量:"+ kaiserCount);
                foreach (var item in biasCount)
                {
                    findRst.Add("bias="+ item.Key+" 数量:" + item.Value);

                }

            };
            if (GUILayout.Button("清除统计"))
            {
                findRst = null;
            }
              if (GUILayout.Button("unity默认参数"))
            {
                bias = -1;
                filterMode = 0;
                 
               // Profiler.enabled = false;
               // Profiler.EndSample();

            }
            if (GUILayout.Button("性能统计"))
            {
                foreach (var item in Selection.objects)
                {
                    if (item == null || item is Texture == false) continue;
                    (item as Texture).mipMapBias = bias;
                }
            }

            EditorGUILayout.EndHorizontal();
            if (findRst != null) {
                foreach (var item in findRst)
                {
                    EditorGUILayout.LabelField(item);
                }
            }
        }
    }

}