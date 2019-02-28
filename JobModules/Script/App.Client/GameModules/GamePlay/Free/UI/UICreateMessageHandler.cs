using App.Client.GameModules.GamePlay.Free.UI;
using Core.Utils;
using Free.framework;
using UnityEngine;
using Utils.Singleton;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;
using Assets.App.Client.GameModules.Ui;
using Core.Free;

namespace Assets.Sources.Free.UI
{
    public class UiCreateMessageHandler : ISimpleMesssageHandler
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(UiCreateMessageHandler));
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_UI_CREATE;
        }

        public void Handle(SimpleProto data)
        {
            if (data.Bs[1] == true)
            {
                data.Bs[1] = false;
                SingletonManager.Get<FreeUiManager>().CacheUI(data.Ss[0], data);
            }
            else
            {
                var ui = Build(data);

                var old = SingletonManager.Get<FreeUiManager>().GetUi(ui.Key);

                if (old != null)
                {
                    old.IsDisabled = true;
                    SingletonManager.Get<FreeUiManager>().RemoveUi(old);
                    Object.Destroy(old.gameObject);
                }

                SingletonManager.Get<FreeUiManager>().AddUi(ui);
                if (ui.AtBottom)
                {
                    ui.gameObject.transform.SetSiblingIndex(0);
                }
            }
        }

        private SimpleFreeUI Build(SimpleProto simpleProto)
        {
            //var canvas = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == "Canvas");
            //if (canvas == null)
            //{
            //    canvas = UnityUiUtility.CreateCanvas(Screen.width, Screen.height);
            //    FreeLayoutConverter.FullScreen(canvas.GetComponent<RectTransform>());
            //}

            //canvas = UiCommon.UIManager.GetLayer(UIComponent.UI.UILayer.Base);

            //EventSystem.current = canvas.GetComponent<EventSystem>();
            //var freeUI = canvas.transform.Find("FreeUI");
            //if (freeUI == null)
            //{
            //    var freeUIObj = new GameObject("FreeUI");
            //    freeUIObj.transform.parent = canvas.transform;
            //    var rectTransform = freeUIObj.AddComponent<RectTransform>();
            //    //rectTransform.anchorMin = new Vector2(0, 1);
            //    //rectTransform.anchorMax = new Vector2(0, 1);
            //    FreeLayoutConverter.FullScreen(rectTransform);
            //    //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            //    freeUIObj.AddComponent<CanvasRenderer>();

            //    freeUI = freeUIObj.transform;
            //}

            int layer = simpleProto.Ks[0];

            Transform freeUI = UiCommon.UIManager.GetLayer((UIComponent.UI.UILayer)layer).transform;

            var root = new GameObject(simpleProto.Ss[0]);

            string parent = simpleProto.Ss[1];

            root.transform.SetParent(freeUI, false);

            GameObject parentObj = null;
            if (!string.IsNullOrEmpty(parent))
            {
                parentObj = GameObject.Find(parent);
                if (parentObj != null)
                {
                    root.transform.SetParent(parentObj.transform, false);
                }
            }

            //root.transform.localPosition = Vector3.zero;

            RectTransform rt = root.AddComponent<RectTransform>();

            root.AddComponent<CanvasRenderer>();

            var ui = root.AddComponent<SimpleFreeUI>();
            ui.Key = simpleProto.Ss[0];
            ui.Visible = simpleProto.Bs[0];
            ui.AtBottom = simpleProto.Bs[1];

            FreeLayoutConverter.FullScreen(rt);

            for (var i = 0; i < simpleProto.Ks.Count - 1; i++)
            {
                var newPo = FreeUIUtil.GetInstance().GetComponent(simpleProto.Ks[i + 1]);
                if (newPo == null)
                {
                    Logger.ErrorFormat("Free component not exist {0}", simpleProto.Ks[i + 1]);
                    continue;
                }
                ui.AddComponent(newPo);
                newPo.SetPos(ui, simpleProto.Fs[i * 2], simpleProto.Fs[i * 2 + 1],
                    simpleProto.Ins[i * 4], simpleProto.Ins[i * 4 + 1],
                    simpleProto.Ins[i * 4 + 2], simpleProto.Ins[i * 4 + 3]);

                newPo.Initial(simpleProto.Ss[i * 3 + 2], simpleProto.Ss[i * 3 + 3]);
                newPo.EventKey = simpleProto.Ss[i * 3 + 4];
                if (newPo is FreeListComponent)
                {
                    newPo.Initial(simpleProto.Ps[i]);
                }


            }

            ui.OrignalHeight = ui.Height;
            ui.OrignalWidth = ui.Width;
            ui.OrignalX = ui.X;
            ui.OrignalY = ui.Y;

            ui.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            return ui;
        }


    }
}
