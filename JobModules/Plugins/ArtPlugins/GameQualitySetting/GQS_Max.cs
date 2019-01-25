using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace ArtPlugins
{
    public class GQS_Max :MonoBehaviour
    {

      //  public PostProcessProfile maxPostProcessProfile;
        public LB_LightingProfile maxLB_LightingProfile;

        [Header("原来的设置  ")]
      //  public PostProcessProfile normalPostProcessProfile;
        public LB_LightingProfile normalLB_LightingProfile;
        private void Awake()
        {
            bool maxQualityMode = QualitySettings.names[QualitySettings.GetQualityLevel()].ToLower().StartsWith("max_");
            if (maxQualityMode) {
                LB_LightingBoxHelper help = FindObjectOfType<LB_LightingBoxHelper>();
                help.mainLightingProfile = maxLB_LightingProfile;
                FindObjectOfType<PostProcessVolume>().sharedProfile = maxLB_LightingProfile.postProcessingProfile;// maxPostProcessProfile;
               
            }
           // print();
        }


#if UNITY_EDITOR
        [ContextMenu("select MAX")]
        void selectMax() {
           
            LB_LightingBoxHelper help = FindObjectOfType<LB_LightingBoxHelper>();
            help.mainLightingProfile = maxLB_LightingProfile;
            FindObjectOfType<PostProcessVolume>().sharedProfile = maxLB_LightingProfile.postProcessingProfile;// maxPostProcessProfile;
           // help.Toggle_Effects();
           // help.Toggle_Effects();
                System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
               LB_LightingBox window = (LB_LightingBox)UnityEditor.EditorWindow.GetWindow<LB_LightingBox>("Lighting Box 2", true, new System.Type[] { inspectorType });
            window.getOrCreateHelp();
            //    if (window == null)
            //    {
            //        Debug.LogError("先打开 lightingbox窗口");
            //        return;
            //    }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(help.gameObject.scene);
        }
        [ContextMenu("select Normal")]
        void selectNormal()
        {
            
            LB_LightingBoxHelper help = FindObjectOfType<LB_LightingBoxHelper>();
            help.mainLightingProfile=normalLB_LightingProfile;
            FindObjectOfType<PostProcessVolume>().sharedProfile = normalLB_LightingProfile.postProcessingProfile;// normalPostProcessProfile;
           // help.Toggle_Effects();
           // help.Toggle_Effects();
            System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            LB_LightingBox window = (LB_LightingBox)UnityEditor.EditorWindow.GetWindow<LB_LightingBox>("Lighting Box 2", true, new System.Type[] { inspectorType });
            window.getOrCreateHelp();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(help.gameObject.scene);

        }
#endif
    }

}
 