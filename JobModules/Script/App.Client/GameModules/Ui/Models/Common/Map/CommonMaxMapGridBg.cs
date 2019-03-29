using System;
using App.Client.GameModules.Ui.Common.MaxMap;
using App.Client.GameModules.Ui.Utils;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using App.Shared.Components.Ui;
using App.Shared.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public partial class CommonMaxMap
    {
        #region Grid
        private Transform lineModel;
        private Transform lineRoot;
        private UIUtils.SimplePool gridPool = null;
        private int hundred = 0;
        private int thousand = 0;
        private GridCommon gridUtil = null;

        private void InitGrids()
        {
            gridPool = new UIUtils.SimplePool(lineModel, lineRoot);
            UIUtils.SetActive(lineModel, false);
            gridUtil = new GridCommon();
            gridUtil.SetAction(UpdateGridCallBack);
            thousand = gridUtil.gridThousandInterval;
            hundred = gridUtil.gridHunderdInterval;
        }
        private void UpdateGrids()
        {
            if (lineModel == null || lineRoot == null)
                return;

            if (MapLevel.Min.Equals(adapter.MapLevel))
            {
                lineRoot.gameObject.SetActive(false);
            }
            else
            {
                lineRoot.gameObject.SetActive(true);
                if (lastMaskCenterPInMapByRice == maskCenterPInMapByRice)
                    return;
                else
                    lastMaskCenterPInMapByRice = maskCenterPInMapByRice;

                gridPool.DespawnAllGo();
                gridUtil.UpdateGrid((int)MaxMapRealWHByRice, (int)MaxMapRepresentWHByRice, maskCenterPInMapByRice);
            }
        }
        private void UpdateGridCallBack(int index, bool vertical)
        {
            var tempValue = index % 10;
            if (tempValue == 0)   //千米级别的线条才创建
            {
                var tran = gridPool.SpawnGo();
                RefreshLineModel(tran, index, vertical);
            }
        }

        private void RefreshLineModel(Transform tran, int temperIndex /*0 79*/, bool vertical)
        {
            //刷新线条位置和角度
            RefreshLinePos(temperIndex, tran, vertical);

            //刷新线条颜色
            RefreshLineColor(temperIndex, tran);

            //刷新线条名字
            RefreshLineName(temperIndex, tran, vertical);
        }
        private void RefreshLinePos(int temperIndex, Transform tran, bool vertical)
        {
            var thousandIndex = temperIndex / 10;
            var hundredIndex = (temperIndex % 10);
            var RT = tran.GetComponent<RectTransform>();
            if (RT)
            {
                if (vertical)
                {
                    var adjustedValue = Mathf.Round((temperIndex * hundred - maskCenterPInMapByRice.x) * rate);
                    Vector2 delatPosByPixel = new Vector2(adjustedValue, 0f);
                    RT.anchoredPosition = delatPosByPixel;
                    tran.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, 0f);
                }
                else
                {
                    var adjustedValue = Mathf.Round((temperIndex * hundred - maskCenterPInMapByRice.y) * rate);
                    Vector2 delatPosByPixel = new Vector2(0, adjustedValue);
                    RT.anchoredPosition = delatPosByPixel;
                    tran.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, 90f);
                }
            }
        }
        private void RefreshLineColor(int temperIndex, Transform tran)
        {
            var thousandIndex = temperIndex / 10;
            var hundredIndex = (temperIndex % 10);
            var imgControl = tran.GetComponent<Image>();
            var RT = tran.GetComponent<RectTransform>(); 
            var beisu = Math.Log((MaxMapRealWHByRice / MaxMapRepresentWHByRice), 2);
            if (imgControl)
            {
                if (hundredIndex != 0)    //百米线   
                {
                    imgControl.color = Color.white;
                    imgControl.color = new Color(imgControl.color.r, imgControl.color.g, imgControl.color.b, (float)beisu * 0.05f);

                    //设置长度
                    RT.sizeDelta = new Vector2(1.5f, maxMapMaskWByPixel);
                }
                else
                {
                    imgControl.color = Color.black;
                    float alpha = 0.4f + (int)beisu * 0.05f;
                    imgControl.color = new Color(imgControl.color.r, imgControl.color.g, imgControl.color.b, alpha);

                    //设置长度
                    RT.sizeDelta = new Vector2(3f, maxMapMaskWByPixel);
                }
            }
        }
        private void RefreshLineName(int temperIndex, Transform tran, bool vertical)
        {
            var thousandIndex = temperIndex / 10;
            var hundredIndex = (temperIndex % 10);

            var name = tran.Find("name");
            var nameText = name.GetComponent<Text>();
            var nameNum = tran.Find("nameNum");
            var nameNumText = nameNum.GetComponent<Text>();
            if (name && nameText && nameNum && nameNumText)
            {
                //刷新名字位置
                if(vertical)
                {
                    name.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);
                }
                else
                {
                    name.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10, 0);
                }

                //刷新名字文本
                nameNumText.text = "";
                if (hundredIndex == 0)   //千米线
                {
                    if (vertical)
                    {
                        nameText.text = gridUtil.GetVerticalStr((int)thousandIndex);
                        nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 1f);   //透明度
                    }
                    else
                    {
                        nameText.text = gridUtil.GetHorizontalStr((int)thousandIndex);
                        nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 1f);  //透明度
                    }
                }
                else
                {
                    nameText.text = "";
                }
            }
        }
        #endregion

        #region MapBg
        private Transform mapBg;
        private Transform maxMapMaskControl = null;
        private RectTransform mapBgRectTran;
        private RectTransform maxMapMaskControlRect;
        private Vector2 dragStart = Vector2.zero; //拖拽变量 保存鼠标在mapbg的位置和 锚点的位置
        private Vector2 dragingPos = Vector2.zero; //拖拽变量 保存鼠标在mapbg的位置和 锚点的位置
        private float dragSpeed = 1f;
        private Vector2 lastMaskCenterPInMapByRice = new Vector2(-1, -1);

        void InitMapBg()
        {
            if (mapBg == null || mapBgRectTran == null)
                return;
            
            //设置大小
            mapBgRectTran.sizeDelta = new Vector2(rate * MaxMapRealWHByRice, rate * MaxMapRealWHByRice);
            //设置位置
            mapBgRectTran.anchoredPosition = Vector2.zero;
            //设置maskControl中心店的 实际地图坐标 米
            maskCenterPInMapByRice = new Vector2(MaxMapRealWHByRice / 2, MaxMapRealWHByRice / 2);

            //根据地图Id 设置mapBg的图片
            var mapBgImg = mapBg.GetComponent<Image>();
            var data = adapter;
            if (data != null)
            {
                var mapId = data.MapId;
                switch (mapId)
                {
                    case 1:
                        {
                            Loader.RetriveSpriteAsync(SingletonManager.Get<MapConfigManager>().SceneParameters.MapLargeBundlename, SingletonManager.Get<MapConfigManager>().SceneParameters.MapLarge, (sprite) =>
                            {
                                mapBgImg.sprite = sprite;
                            });
                        }
                        break;
                }
            }
        }
        void UpdataMapBg()
        {
            if (!isGameObjectCreated || mapBg == null || mapBgRectTran == null || maxMapMaskControlRect == null)
                return;
           
            Vector2 lastHitPInMapImgByPixel = Vector2.zero;   //鼠标位置在地图control上的 像素位置
            Vector2 lastHitPInMaskByPixel = Vector2.zero;     //鼠标位置在遮罩control上的 像素位置 
            Vector2 hitPoint = Vector2.zero;
            Vector2 hitPoint1 = Vector2.zero;
            Vector2 startPoint = Vector2.zero;
            Vector2 endPoint = Vector2.zero;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(maxMapMaskControlRect, Input.mousePosition, uiCamera, out hitPoint1))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBgRectTran, Input.mousePosition, uiCamera, out hitPoint))
                {
                    lastHitPInMapImgByPixel = hitPoint;
                    lastHitPInMaskByPixel = hitPoint1;

                    //记录mask中心点对应的 真实地图的坐标 米   
                    var temPos = new Vector2(MaxMapRealWHByRice/2, MaxMapRealWHByRice/2) + lastHitPInMapImgByPixel / lastRate;
                    maskCenterPInMapByRice = temPos - lastHitPInMaskByPixel / rate;

                    //设置大小
                    mapBgRectTran.sizeDelta = new Vector2(rate * MaxMapRealWHByRice, rate * MaxMapRealWHByRice);

                    //设置位置
                    startPoint = hitPoint;
                    endPoint = rate / lastRate * startPoint;
                    mapBgRectTran.anchoredPosition += (startPoint - endPoint);
                                        
                    //校正
                    AdjustMapBgLocation();
                }
            }
        }

        private void AdjustMapBgLocation()
        {
            var border = Math.Abs((mapBgRectTran.rect.width - maxMapMaskControlRect.rect.width) / 2);

            var newPos = new Vector2();
            newPos = mapBgRectTran.anchoredPosition;
            var lastPos = mapBgRectTran.anchoredPosition;

            if (newPos.x > border)
            {
                newPos.x = border;
            }
            else if (newPos.x < -border)
            {
                newPos.x = -border;
            }

            if (newPos.y > border)
            {
                newPos.y = border;
            }
            else if (newPos.y < -border)
            {
                newPos.y = -border;
            }

            mapBgRectTran.anchoredPosition = newPos;
            maskCenterPInMapByRice = maskCenterPInMapByRice - (newPos - lastPos) / rate;

        }
        #endregion

        #region MapBgEventTrigger
        private void OnPointerDown(BaseEventData eventData)
        {
            var data = eventData as PointerEventData;
            Vector2 mouseDown = data.position;    
            Vector2 mouseUguiPos = new Vector2();
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(maxMapMaskControlRect, mouseDown, null, out mouseUguiPos);
            if (isRect)  
            {
                dragStart = mouseUguiPos;                   //计算图片位置和鼠标点在mask上的的差值
            }
        }
        private void OnPointerUp(BaseEventData eventData)
        {
            dragStart = Vector2.zero;
        }

        private void OnDrag(BaseEventData eventData)
        {
            //在缩放模式下 才能拖拽
            if(MaxMapRepresentWHByRice != MaxMapRealWHByRice && dragStart != Vector2.zero)
            {
                var data = eventData as PointerEventData;
                Vector2 mouseDrag = data.position;   //当鼠标拖动时的屏幕坐标
                Vector2 uguiPos = new Vector2();
                bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(maxMapMaskControlRect, mouseDrag, null, out uguiPos);
                if (isRect)
                {
                    dragingPos = uguiPos;
                    SetMapBgLocationByDrag(dragingPos - dragStart);
                    dragStart = dragingPos;
                }
                //Debug.Log("OnDrag");
                Cursor.visible = false;
            }
        }
        private void OnEndDrag(BaseEventData eventData)
        {
            //Debug.Log("OnEndDrag");
            dragStart = Vector2.zero;
            Cursor.visible = true;
        }

        /**
         *  传入一个像素单位的offset   移动mapBg这个ui的位置 并且更新maskcontrol的中心点的的位置（米）
         *  当offset为正时  mapbg向右移动 中心店坐标变小 反之
         */
        private void SetMapBgLocationByDrag(Vector2 offset /*像素单位*/)
        {
            //Debug.Log("ddd " + offset.ToString());
            var border = Math.Abs((mapBgRectTran.rect.width - maxMapMaskControlRect.rect.width) / 2);
            var lastPos = mapBgRectTran.anchoredPosition;
            var newPos = lastPos + offset * dragSpeed; //*2 是提高 拖动速度效果而已

            if (Math.Abs(newPos.x) <= border && Math.Abs(newPos.y) <= border)
            {
                mapBgRectTran.anchoredPosition = newPos;
                maskCenterPInMapByRice = maskCenterPInMapByRice - (offset * dragSpeed) / rate;  
            }
            else 
            {
                if(newPos.x > border)
                {
                    newPos.x = border;
                }
                else if(newPos.x < -border)
                {
                    newPos.x = -border;
                }

                if (newPos.y > border)
                {
                    newPos.y = border;
                }
                else if(newPos.y < - border)
                {
                    newPos.y = -border;
                }

                mapBgRectTran.anchoredPosition = newPos;
                maskCenterPInMapByRice = maskCenterPInMapByRice - (newPos - lastPos) / rate;
            }

            //Debug.Log("kkk " + maskCenterPInMapByRice.ToString());
        }

        #endregion
    }
}    
