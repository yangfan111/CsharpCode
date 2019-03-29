using Assets.Sources.Free.Render;
using Assets.Scripts.Utils.Coroutine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using Assets.App.Client.GameModules.Ui;

namespace Assets.Sources.Free.Utility
{
    public class UnityUiUtility
    {
        public static void EnsureComponent<T>(GameObject obj) where T : MonoBehaviour
        {
            if (obj.GetComponent<T>() == null)
                obj.AddComponent<T>();
        }

        public static GameObject CreateCanvas(int width, int height)
        {
            GameObject newCanvas = new GameObject("Canvas");
            var rectTransform = newCanvas.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, height);
            Canvas c = newCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = newCanvas.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            newCanvas.AddComponent<GraphicRaycaster>();
            var standAlone = newCanvas.AddComponent<StandaloneInputModule>();
            standAlone.forceModuleActive = true;
            newCanvas.AddComponent<EventSystem>();

            return newCanvas;
        }

        public static IUiObject CreateImageDisplayObject(string url, int width, int height)
        {
            var gameObject = new GameObject("Image");
            var image = gameObject.AddComponent<RawImage>();
            image.texture = ResourceUtility.GetTransparentTexture();
            CoroutineRunner.StartCoroutine(gameObject, ResourceUtility.LoadImage(image, url));
            var displayObj = new UnityUiObject(gameObject);
            displayObj.width = width;
            displayObj.height = height;
            return displayObj;
        }

        public static IUiObject CreateNumberTextDisplayObject(int fontIndex)
        {
            var gameObject = new GameObject("Number");
            var text = gameObject.AddComponent<Text>();
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.alignment = TextAnchor.UpperLeft;
            CoroutineRunner.StartCoroutine(gameObject, ResourceUtility.LoadNumberFont(text, fontIndex));
            var displayObject = new UnityUiObject(gameObject);
            return displayObject;
        }

        public static IUiObject CreateEmptyDisplayObject(string name = null)
        {
            var gameObject = new GameObject(name);
            gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<CanvasRenderer>();
            return new UnityUiObject(gameObject);
        }

        public static void SetTexture(IUiObject uiObject, string url)
        {
            var image = uiObject.gameObject.GetComponent<RawImage>();
            if (image == null)
                return;
            CoroutineRunner.StartCoroutine(uiObject.gameObject, ResourceUtility.LoadImage(image, url));
        }

        private static Dictionary<string, GameObject> objCache;

        public static GameObject RootCanvas
        {
            get { return UiCommon.UIManager.UIRoot; }
        }

        public static GameObject FindUIObject(GameObject parent, string path)
        {
            if (objCache == null)
            {
                objCache = new Dictionary<string, GameObject>();
            }

            if (parent == null)
            {
                Debug.LogError("");
            }
            string key = path + parent.GetInstanceID();

            if (!objCache.ContainsKey(key))
            {
                string[] pp = path.Split("/");

                if (parent != null)
                {
                    RectTransform[] recs = parent.GetComponentsInChildren<RectTransform>(true);
                    foreach (RectTransform rec in recs)
                    {
                        bool meet = true;
                        GameObject obj = rec.gameObject;
                        for (int i = pp.Length - 1; i >= 0; i--)
                        {
                            string p = pp[i];
                            if (obj.name.Trim() != p.Trim())
                            {
                                meet = false;
                                break;
                            }
                            if (obj.transform.parent != null)
                            {
                                obj = obj.transform.parent.gameObject;
                            }
                        }

                        if (meet)
                        {
                            objCache.Add(key, rec.gameObject);
                            break;
                        }
                    }
                }
            }

            if (!objCache.ContainsKey(key))
            {
                //Debug.Log(path + " is not found as GameObject.");
            }

            return objCache.GetOrDefault(key);
        }

        public static string GetPath(GameObject obj)
        {
            string s = obj.name;

            GameObject parent = obj;
            while(parent != null)
            {
                Transform tf = parent.GetComponent<Transform>();
                if(tf.parent != null)
                {
                    parent = tf.parent.gameObject;
                    s = parent.name + "/" + s;
                }
                else
                {
                    parent = null;
                }
            }

            return s;
        }

        public static GameObject GetFreeCanvas()
        {
            return UiCommon.UIManager.UIRoot;
        }

        public static GameObject FindUIObject(string path)
        {
            var canvas = GetFreeCanvas();
            return FindUIObject(canvas, path);
        }

    }
}
