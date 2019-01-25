//Command Buffer测试
//by: puppet_master
//2017.5.26

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityScript.Lang;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ArtPlugins
{
    
    public class RTDetailTool : MonoBehaviour
    {
        public enum DataDetalFaceMode
        {A,B,A_B };
            [System.Serializable]
        public class DataDetalTextureLayer
        {
            public Texture2D texture;
            public DataDetalFaceMode faceMode;
            public Color32 color=Color.white;
            public Vector2 pos;
            public Vector2 scale = Vector2.one;
            public float rot = 0;

            public DataDetalTextureLayer() { }
            public DataDetalTextureLayer(DataDetalTextureLayer lay)
            {
                texture = lay.texture;
                color = lay.color;
                pos = lay.pos + new Vector2(0.1f, 0.1f);
                scale = lay.scale;
                rot = lay.rot;
            }
        }
        public Vector3 size = Vector3.one;

       
        public List<DataDetalTextureLayer> layers = new List<DataDetalTextureLayer>();

        internal DataDetalTextureLayer currentEditorLayer;


#if UNITY_EDITOR

        private Matrix4x4 getTexturetMatrix(DataDetalTextureLayer item,bool faceA) {
             
              return   Matrix4x4.TRS(new Vector3(item.pos.x - 0.5f * item.scale.x, faceA ? (item.pos.y / 2 - 0.5f * item.scale.y) : 0.5f + (item.pos.y / 2 - 0.5f * item.scale.y), 0), Quaternion.identity, new Vector3(item.scale.x, item.scale.y, 1))//scale and move
                        * Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(item.rot * 2, Vector3.forward), Vector3.one);//rot local and move ;
        }
        //也可以在OnPreRender中直接通过Graphics执行Command Buffer，不过OnPreRender和OnPostRender只在挂在相机的脚本上才有作用！！！
      public  void UpdateTexture(RenderTexture renderTexture ,Material desMaterial,Material rtMakerMaterial)
        {
 
            if (layers.Count == 0) {
                layers.Add(new DataDetalTextureLayer());
            }
            RenderTexture rt = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            UnityEngine.RenderTexture.active = rt;
   
            desMaterial.SetVector("_ScaleOffset", new Vector4(size.z, size.y*2, 0.5f, 0.25f));
 
            foreach (DataDetalTextureLayer item in layers)
            {


                rtMakerMaterial.color = item.color;
                if(item.faceMode==DataDetalFaceMode.A|| item.faceMode == DataDetalFaceMode.A_B)
                {
                    var mt = getTexturetMatrix(item, true);
                    rtMakerMaterial.SetMatrix("_transMt", GL.GetGPUProjectionMatrix(mt, false));
                    Graphics.Blit(item.texture, renderTexture, rtMakerMaterial);
                }
                if (item.faceMode == DataDetalFaceMode.B || item.faceMode == DataDetalFaceMode.A_B)
                {
                    var mt = getTexturetMatrix(item, false);
                    rtMakerMaterial.SetMatrix("_transMt", GL.GetGPUProjectionMatrix(mt, false));
                    Graphics.Blit(item.texture, renderTexture, rtMakerMaterial);
                }


            }
 
        }
     
#endif

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(Vector3.zero, size);
            var force2DPos = transform.position;
            force2DPos.x = 0;
            transform.position = force2DPos;
            if (currentEditorLayer == null) return;
            var vpos = new Vector2(transform.position.z, transform.position.y);
            vpos.x = (vpos.x + (size.z / 2)) / size.z;
            vpos.y = (vpos.y + (size.y / 2)) / size.y;


            currentEditorLayer.pos = vpos;


            var vScale = new Vector2(transform.localScale.z, transform.localScale.y);
            vScale.x = (vScale.x) / size.z;
            vScale.y = (vScale.y) / size.y;

            currentEditorLayer.scale = vScale;

            var vRot = transform.localRotation.eulerAngles.x;
            currentEditorLayer.rot = vRot;
        }


        public void SelectLayer(DataDetalTextureLayer willEditor)
        {
            currentEditorLayer = willEditor;
            var vpos = currentEditorLayer.pos;
            vpos.x = vpos.x * size.z - (size.z / 2);
            vpos.y = vpos.y * size.y - (size.y / 2);
            transform.position = new Vector3(0, vpos.y, vpos.x);
 
            var vScale = currentEditorLayer.scale;
            vScale.x = vScale.x * size.z;
            vScale.y = vScale.y * size.y;
            transform.localScale = new Vector3(1, vScale.y, vScale.x);


            var vRot = currentEditorLayer.rot ;
            transform.localRotation = Quaternion.Euler(vRot, 0, 0);
        }
        //void OnPreRender()
        //{
        //    //在正式渲染前执行Command Buffer
        //    Graphics.ExecuteCommandBuffer(commandBuffer);
        //}
    }

#if UNITY_EDITOR
    namespace ArtPlugins
    {
        [CustomEditor(typeof(RTDetailTool))]
        public class RTDetailToolEditor : Editor
        {
            public static RenderTexture renderTexture = null;
            public Renderer gunRender=null;
            public static Material replaceMaterial = null;

            void OnEnable()
            {
                EditorApplication.update += Update;
                if (replaceMaterial == null) {
                    replaceMaterial = new Material(Shader.Find("Unlit/AdditiveMatrix"));
                }
                RTDetailTool tg = (RTDetailTool)target;
                gunRender =tg.transform.parent.GetComponentInChildren<Renderer>();
                gunRender.sharedMaterial.shader = Shader.Find("Custom/RTDetail");
                if (renderTexture==null)
                renderTexture = RenderTexture.GetTemporary(1024, 1024, 16, RenderTextureFormat.BGRA32, RenderTextureReadWrite.Default, 4);
                gunRender.sharedMaterial.SetTexture("_ModifyTex", renderTexture);
                 //EditorApplication.update

            }
            void Update() {
                RTDetailTool tg = (RTDetailTool)target;
                tg.UpdateTexture(renderTexture, gunRender.sharedMaterial, replaceMaterial);

            }


            void OnDisable()
            {
                EditorApplication.update -= Update;
               // renderTexture.Release();
            }
           
            public override void OnInspectorGUI()
            {

                 // base.OnInspectorGUI();
                RTDetailTool tg = (RTDetailTool)target;
                tg.size.z = EditorGUILayout.Slider("横向范围",tg.size.z, 0.1f, 5f);
                tg.size.y = EditorGUILayout.Slider("纵向范围",tg.size.y, 0.1f, 5f);
              
                tg.size.x = 0;
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);

                    //  tg.OnDrawGizmosSelected();
                }
                foreach (var lay in tg.layers)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    lay.texture = EditorGUILayout.ObjectField(lay.texture, typeof(Texture2D), GUILayout.MinHeight(60), GUILayout.Width(60)) as Texture2D;
                    lay.faceMode= (RTDetailTool.DataDetalFaceMode)EditorGUILayout.EnumPopup(lay.faceMode) ;
                    EditorGUILayout.EndVertical();
                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(target);

                        //  tg.OnDrawGizmosSelected();
                    }
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    lay.color = EditorGUILayout.ColorField("", lay.color, GUILayout.Width(60));
                    var index = tg.layers.IndexOf(lay);
                    if (index > 0 && GUILayout.Button("⇧"))
                    {

                        tg.layers[index] = tg.layers[index - 1];
                        tg.layers[index - 1] = lay;

                    }

                    if (index < tg.layers.Count - 1 && GUILayout.Button("⇩"))
                    {

                        tg.layers[index] = tg.layers[index + 1];
                        tg.layers[index + 1] = lay;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("编辑"))
                    {
                        tg.SelectLayer(lay);
                    }
                    if (GUILayout.Button("新增"))
                    {
                        var clone = new RTDetailTool.DataDetalTextureLayer(lay);
                        tg.layers.Insert(tg.layers.IndexOf(lay), clone);
                        tg.SelectLayer(clone);

                    }

                    if (GUILayout.Button("删除"))
                    {
                        tg.currentEditorLayer = null;
                        tg.layers.Remove(lay);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                }
             

            }




        }
      }
#endif

}