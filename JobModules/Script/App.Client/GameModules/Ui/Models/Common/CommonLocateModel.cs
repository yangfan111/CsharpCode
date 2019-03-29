using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonLocateModel : ClientAbstractModel, IUiSystem
    {
        ILocationUiAdapter adapter;
        private bool isGameObjectCreated = false;

        private static int ruleWidth = 720;
        private static int showRuleWidth = 512;
        private static float showAngel = 360f / ruleWidth * showRuleWidth;
        private static int angelPreGirl = 15;
        private static int pixelPreGirl = ruleWidth / 360 * angelPreGirl;
        private static float rulerImageOffSet = 0.166f;

        private RawImage rulerRawImage;
        private RawImage scaleRulerRawImage;
        private RawImage rulerMaskRawIamge;
        private Rect rulerRawImageRect;
        private Rect scaleRulerRawImageRect;
        private Rect rulerMaskRawIamgeRect;
        private Text angelText;

        private Dictionary<float, MarkItem> markList;

        private Transform root;
        private Transform markRoot;
        private RectTransform markRootRect;
      
        private Dictionary<float, string> locationConfig = new Dictionary<float, string>();
        //标记 相关变量
        Transform markModel = null;

        private float lastAngel = -190;

        private CommonLocateViewModel _viewModel = new CommonLocateViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonLocateModel(ILocationUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;

            markList = new Dictionary<float, MarkItem>();
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            PreparedLocationConfig();
            DynamicCreateGameObject();
        }

       public override void Update(float interval)
        {
            if (!isVisible) return;

            UpdateLocationGroup();
        }

        private void PreparedLocationConfig()
        {
            locationConfig.Clear();         
            locationConfig.Add(0, I2.Loc.ScriptLocalization.client_common.word5);
            locationConfig.Add(45, I2.Loc.ScriptLocalization.client_common.word6);
            locationConfig.Add(90, I2.Loc.ScriptLocalization.client_common.word7);
            locationConfig.Add(135, I2.Loc.ScriptLocalization.client_common.word8);
            locationConfig.Add(180, I2.Loc.ScriptLocalization.client_common.word9);
            locationConfig.Add(225, I2.Loc.ScriptLocalization.client_common.word10);
            locationConfig.Add(270, I2.Loc.ScriptLocalization.client_common.word11);
            locationConfig.Add(315, I2.Loc.ScriptLocalization.client_common.word12);
            locationConfig.Add(360, I2.Loc.ScriptLocalization.client_common.word5);
        }
        private void DynamicCreateGameObject()
        {
            rulerRawImage = FindChildGo("RulerRawImage").GetComponent<RawImage>();
            rulerRawImageRect = rulerRawImage.uvRect;
            root = FindChildGo("Show");
            markModel = FindChildGo("mark");
            markRoot = FindChildGo("markRoot");
            angelText = FindChildGo("AngelText").GetComponent<Text>();
            markRootRect = markRoot.GetComponent<RectTransform>();
            markModel.gameObject.SetActive(false);
        }
       
        private void UpdateLocationGroup()
        {
            if (adapter != null && isGameObjectCreated)
            {
                MovingControl(adapter._CurFaceDirection);
            }
        }

        private void MovingControl(float curPlayerFaceDirection)  //curPlayerFaceDirection 《0，360》 顺时针方向
        {
            //变化超过1度才刷新
            if (Mathf.Abs(lastAngel - curPlayerFaceDirection) < 0.2) return;
            lastAngel = curPlayerFaceDirection;

            rulerRawImageRect.x = 1 - (curPlayerFaceDirection + 180) / 360 + rulerImageOffSet;
            rulerRawImage.uvRect = rulerRawImageRect;

            var angel = curPlayerFaceDirection < 0 ? -curPlayerFaceDirection : 360 - curPlayerFaceDirection;
            UpdateAngelText(angel);
            UpdateMarkList(angel);
        }
        private void UpdateAngelText(float angel)
        {
           
//            Debug.Log(angel);
            var showTextAngel = Mathf.Round(angel / 5) * 5;
            var showText = string.Empty;
            if (locationConfig.ContainsKey(showTextAngel))
            {
                showText = locationConfig[showTextAngel];
            }
            else
            {
                showText = showTextAngel.ToString();
            }
            if (showText.Equals(angelText.text) == false) angelText.text = showText;
        }

        private void UpdateMarkList(float angel)
        {
            var teamMarkInfos = adapter.TeamPlayerMarkInfos;
//             var teamMarkInfos = TeamPlayerMarkInfos;
            for (int j = 0; teamMarkInfos.Count > 0 && j < teamMarkInfos.Count; j++)
            {
                var markInfo = teamMarkInfos[j];
                if (!markList.ContainsKey(markInfo.Angel))
                {
                    var item = GetMarkItem();
                    item.Angel = markInfo.Angel;
                    markList.Add(markInfo.Angel, item);
                    item.Img.color = new Color(markInfo.MarkColor.r, markInfo.MarkColor.g, markInfo.MarkColor.b, item.Img.color.a);
                }

                var markItem = markList[markInfo.Angel];
                markItem.IsOutDate = false;

                var itemAngel = markInfo.Angel - angel;
                if(itemAngel > 0) itemAngel = itemAngel > 180 ? itemAngel - 360 : itemAngel;
                if(itemAngel < 0) itemAngel = itemAngel < -180 ? itemAngel + 360 : itemAngel;
                if ((itemAngel < 0 && itemAngel < - showAngel /2f) || (itemAngel > 0 && itemAngel > showAngel / 2f))
                {
                    markItem.MarkTf.gameObject.SetActive(false);
                    continue;
                }
                if(markItem.MarkTf.gameObject.active == false) markItem.MarkTf.gameObject.SetActive(true);
                markItem.MarkRtf.anchoredPosition =  new Vector2(itemAngel / angelPreGirl * pixelPreGirl, markItem.MarkRtf.anchoredPosition.y);
                markItem.Img.color = new Color(markInfo.MarkColor.r, markInfo.MarkColor.g, markInfo.MarkColor.b, Mathf.Lerp(0.3f,1, 1 - Mathf.Abs(itemAngel) / (showAngel / 2f)));
            }

            foreach (var pair in markList)
            {
                if (pair.Value.Unused == false)
                {
                    if (pair.Value.IsOutDate)
                    {
                        pair.Value.MarkRtf.gameObject.SetActive(false);
                        pair.Value.Unused = true;
                    }
                    pair.Value.IsOutDate = true;
                }
            }
        }

        private MarkItem GetMarkItem()
        {
            MarkItem item = null;
            foreach (var pair in markList)
            {
                if (pair.Value.Unused)
                {
                    item = pair.Value;
                    break;
                }
            }
            if (item == null)
            {
                var itemObj = GameObject.Instantiate(markModel, markRoot);
                var img = itemObj.GetComponent<Image>();
                var rtf = itemObj.GetComponent<RectTransform>();
                item =  new MarkItem() { MarkTf = itemObj, MarkRtf = rtf, Img = img };
            }
            else
            {
                markList.Remove(item.Angel);
            }

            item.Unused = false;
            return item;
        }
    }

    class MarkItem
    {
        public float Angel;
        public Image Img;
        public Transform MarkTf;
        public RectTransform MarkRtf;
        public bool IsOutDate;
        public bool Unused;
    }
}    
