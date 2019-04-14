using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Common.MaxMap;
using App.Client.GameModules.Ui.Utils;
using Core.Ui.Map;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public partial class CommonMaxMap
    {      
        private Transform airPlane;
        private RectTransform airPlaneRT;
        private Transform kTouModel;
        private Transform kTouRoot;

        private float sumTime = 0;
        private int imgIndex = 0;
        private bool isNeedChangeSprite = false;

        private float lastKongTouDirection = 0;  // 控投机上次的方向
        private float lastYunShuDirection = 0;   // 运输机上次的方向
        private int lastPlaneType = -1; //上一次飞机的类型
        UIUtils.SimplePool airPlanePool = null;

        private void InitAirPlane()
        {
            airPlaneRT = airPlane.GetComponent<RectTransform>();
            airPlanePool = new UIUtils.SimplePool(kTouModel, kTouRoot);
        }

        private void UpateAirPlane(float interval)
        {
            if (kTouModel == null || kTouRoot == null)
                return;

            UIUtils.SetActive(kTouModel, false);
            var planeData = adapter.PlaneData;
            if (planeData.Type == 0)  //目前无飞机
            {
                if(planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, false);
                    airPlanePool.DespawnAllGo();
                    lastPlaneType = planeData.Type;
                }
                isNeedChangeSprite = false;
            }
            else if (planeData.Type == 1) //空投机
            {
                if (planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, true);
                    lastPlaneType = planeData.Type;
                }

                //更新飞机位置
                Vector2 planePosByPixel = (planeData.Pos.ShiftedUIVector2() - maskCenterPInMapByRice) * rate;
                airPlaneRT.anchoredPosition = planePosByPixel;

                //更新飞机方向
                float delataAngel = planeData.Direction - lastKongTouDirection;
                airPlane.transform.RotateAround(kTouRoot.transform.position, UnityEngine.Vector3.forward, -delataAngel);
                lastKongTouDirection = planeData.Direction;

                airPlanePool.DespawnAllGo();

                List<MapFixedVector2> list = adapter.KongTouList();
                foreach (var item in list)                      //更新空投点
                {
                    var tran = airPlanePool.SpawnGo();
                    //设置空投点的位置
                    Vector2 kTouPosByPixel = (item.ShiftedUIVector2() - maskCenterPInMapByRice) * rate;
                    tran.GetComponent<RectTransform>().anchoredPosition = kTouPosByPixel;
                }
                isNeedChangeSprite = true;
            }
            else if(planeData.Type == 2)  //运输机
            {
                if (planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, true);
                    airPlanePool.DespawnAllGo();
                    lastPlaneType = planeData.Type;
                }

                //更新飞机位置
                Vector2 planePosByPixel = (planeData.Pos.ShiftedUIVector2() - maskCenterPInMapByRice) * rate;
                airPlaneRT.anchoredPosition = planePosByPixel;

                //更新飞机方向
                float delataAngel = planeData.Direction - lastKongTouDirection;
                airPlane.transform.RotateAround(kTouRoot.transform.position, UnityEngine.Vector3.forward, -delataAngel);
                lastKongTouDirection = planeData.Direction;
                isNeedChangeSprite = false;
            }
            
            UpdateKTouImg(interval);
        }

        private void UpdateKTouImg(float interval)
        {
            if (isNeedChangeSprite == false)
            {
                imgIndex = 0;
                return;
            }
            else
            {
                sumTime = sumTime + interval;
                if (sumTime > AirPlaneCommon.GetInstance().intervalTime)
                {
                    sumTime = 0;
                    imgIndex = AirPlaneCommon.GetInstance().CaculateSprteIndex(imgIndex);     //图片1到13 （从小到大）  就是下标0到12
                    foreach (var item in airPlanePool.GetUsingList())     
                    {
                        var tran = item;
                        Image img = tran.GetComponent<Image>();
                        var temperSprite = AirPlaneCommon.GetInstance().GetSpriteByNum(imgIndex);
                        if (temperSprite != null)
                        {
                            img.sprite = temperSprite;
                        }
                    }                   
                }
            }
        }




        #region 飞机航线
        private RectTransform routeLineGo = null;
        private void InitRouteLine()
        {
            RouteLineCommon.GetInstance().SetMiniMapAdapter(adapter);
        }

        private void UpdateRouteLine()
        {
            RouteLineCommon.GetInstance().UpdataRouteLineCommon(maskCenterPInMapByRice, routeLineGo, rate);
        }
        #endregion
    }
}    
