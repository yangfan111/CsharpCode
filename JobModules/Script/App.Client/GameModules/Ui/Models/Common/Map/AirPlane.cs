using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Ui.Map;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class AirPlane
    {
        private IUiResourcesLoader loader;
        //空投点 通用
        private Dictionary<string, Sprite> dropAnimSpriteDic = new Dictionary<string, Sprite>();
        public int spriteSum = 13;

        private Transform tran;
        private Transform airPlane;
        private Transform airDrop;

        private int lastPlaneType = -1;
        private bool isNeedChangeSprite;
        private float lastKongTouDirection;

        private List<Transform> airDropList = null;


        public AirPlane(Transform tran, IUiResourcesLoader loader)
        {
            this.loader = loader;
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            airPlane = tran.Find("airPlane");
            airDrop = tran.Find("kongtouModel");
            UIUtils.SetActive(airDrop, false);

            PreParedKTouSprite();
        }

        private void PreParedKTouSprite()
        {
            for (int i = 1; i <= spriteSum; i++)
            {
                var name = GetSpriteNameByNum(i);
                loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, name,
                    (sprite) =>
                    {
                        dropAnimSpriteDic.Add(name, sprite);
                    });
            }
        }

        public void HideAirDrop()
        {
            if (!isNeedChangeSprite) return;
            isNeedChangeSprite = false;
            foreach (var dropTf in airDropList)
            {
                UIUtils.SetActive(dropTf, false);
            }
        }

        public void ShowAirDrop(List<MapFixedVector2> dropList, float rate)
        {
            if (isNeedChangeSprite) return;
            isNeedChangeSprite = true;
            if (airDropList == null) airDropList = new List<Transform>();
            int i = 0;
            for (; i < dropList.Count; i++)
            {
                Transform dropTf = null;
                if (airDropList.Count <= i)
                {
                    dropTf = GameObject.Instantiate(airDrop, tran, true);
                    airDropList.Add(dropTf);
                }
                else
                {
                    dropTf = airDropList[i];
                }
                // 设置空投点的位置
                UIUtils.SetActive(dropTf, true);
                Vector2 kTouPosByPixel = (dropList[i].ShiftedUIVector2()) * rate;
                dropTf.GetComponent<RectTransform>().anchoredPosition = kTouPosByPixel;
            }
            for (; i < airDropList.Count; i++)
            {
                Transform dropTf = airDropList[i];
                UIUtils.SetActive(dropTf, false);
            }
        }

        public void Update(float interval, float rate, List<MapFixedVector2> airDropList, AirPlaneData planeData)
        {
            if (planeData.Type == 0)  //目前无飞机
            {
                if (planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, false);
                    HideAirDrop();
                    lastPlaneType = planeData.Type;
                }
            }
            else if (planeData.Type == 1) //空投机
            {
                if (planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, true);
                    lastPlaneType = planeData.Type;
                }

                //更新飞机位置
                Vector2 planePosByPixel = (planeData.Pos.ShiftedUIVector2()) * rate;
                airPlane.GetComponent<RectTransform>().anchoredPosition = planePosByPixel;

                //更新飞机方向
                float delataAngel = planeData.Direction - lastKongTouDirection;
                airPlane.transform.RotateAround(tran.position, UnityEngine.Vector3.forward, -delataAngel);
                lastKongTouDirection = planeData.Direction;

                //更新空投点
                ShowAirDrop(airDropList, rate);
            }
            else if (planeData.Type == 2)  //运输机
            {
                if (planeData.Type != lastPlaneType)
                {
                    UIUtils.SetActive(airPlane, true);
                    HideAirDrop();
                    lastPlaneType = planeData.Type;
                }

                //更新飞机位置
                Vector2 planePosByPixel = (planeData.Pos.ShiftedUIVector2()) * rate;
                airPlane.GetComponent<RectTransform>().anchoredPosition = planePosByPixel;

                //更新飞机方向
                float delataAngel = planeData.Direction - lastKongTouDirection;
                airPlane.transform.RotateAround(tran.position, UnityEngine.Vector3.forward, -delataAngel);
                lastKongTouDirection = planeData.Direction;
            }

            UpdateKTouImg(interval);
        }

        public float intervalTime = 0.03f;
        private float sumTime;
        private int imgIndex;

        private string GetSpriteNameByNum(int i)  /*i >=1  <= 13*/
        {
            var name = "000";
            if (i < 10)
            {
                name = name + "0" + i.ToString();
            }
            else
            {
                name = name + i.ToString();
            }
            return name;
        }
        private Sprite GetSpriteByName(string name)
        {
            if (dropAnimSpriteDic.ContainsKey(name))
            {
                return dropAnimSpriteDic[name];
            }
            else
                return null;
        }
        public Sprite GetSpriteByNum(int i)
        {
            return GetSpriteByName(GetSpriteNameByNum(i));
        }
        public int CaculateSprteIndex(int index)
        {
            int temIndex = -1;
            temIndex = Mathf.Abs((index + 1) % (spriteSum + 1));
            return temIndex;
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
                if (sumTime > intervalTime)
                {
                    sumTime = 0;
                    imgIndex = CaculateSprteIndex(imgIndex);     //图片1到13 （从小到大）  就是下标0到12
                    foreach (var item in airDropList)
                    {
                        var tran = item;
                        Image img = tran.GetComponent<Image>();
                        var temperSprite = GetSpriteByNum(imgIndex);
                        if (temperSprite != null)
                        {
                            img.sprite = temperSprite;
                        }
                    }
                }
            }
        }
    }
}