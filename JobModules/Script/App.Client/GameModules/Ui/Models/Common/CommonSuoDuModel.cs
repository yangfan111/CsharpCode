using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using Core.SpatialPartition;
using Core.Ui.Map;
using UnityEngine.UI;
using Core.Utils;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonSuoDuModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonSuoDuModel));
        ISuoDuUiAdapter adapter = null;
        private bool isGameObjectCreated = false;
        private DuQuanInfo curDuquan = new DuQuanInfo(-1,new MapFixedVector2(Vector2.zero), 0, 0,0);
        private DuQuanInfo safeArea = new DuQuanInfo(-1, new MapFixedVector2(Vector2.zero), 0, 0, 0);
        private DuQuanInfo rawDuquan = new DuQuanInfo(-1, new MapFixedVector2(Vector2.zero), 0, 0, 0);
        private UnityEngine.Vector2 curPlayPos = new UnityEngine.Vector2(0, 0);

        //滑动条相关变量
        Vector2 intersectPointWithRaw = Vector2.zero;           //毒圈的交点
        Vector2 intersectPointWithSafe = Vector2.zero;          //安全区交点
        Vector2 intersectPointWithMoving = Vector2.zero;        //移动状态毒圈的交点

        private Transform root = null;
        private RectTransform playerImgRt = null;
        private RectTransform suoduRt = null;

        private bool isChangeColor = false;
        private float changeColorTimeInterval = 0.2f;
        private float changColorTime = 0;

        //时间相关变量
        private float waitTime = 0;
        private float startTime = 0;

        private CommonSuoDuViewModel _viewModel = new CommonSuoDuViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonSuoDuModel(ISuoDuUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
        }
        public override void Update(float interval)
        {
            if (!isVisible) return;
            RefreshGui(interval);
        }

        private void InitGui()
        {
            playerImgRt = FindChildGo("PlayerImg").GetComponent<RectTransform>();
            suoduRt = FindChildGo("SuoDu").GetComponent<RectTransform>();
            root = FindChildGo("root");
        }

        private void RefreshGui(float interval)
        {
            if (adapter != null && isGameObjectCreated)
            {
                if (safeArea.Level != adapter.NextDuquan.Level)
                {
                    var tempData = adapter.CurDuquan;
                    rawDuquan.SetValue(tempData.Level, tempData.Center, tempData.Radius, tempData.WaitTime, tempData.MoveTime);
                    waitTime = rawDuquan.WaitTime;
                    startTime = Time.time;
                }

                //设置当前数据
                curDuquan = adapter.CurDuquan;
                safeArea = adapter.NextDuquan;
//                curPlayPos.x = adapter.CurPosition.x;
//                curPlayPos.y = adapter.CurPosition.z;
                curPlayPos = adapter.CurPosition.WorldVector3().To2D();

                if (curDuquan.Level != adapter.OffLineNum && safeArea.Level != adapter.OffLineNum && rawDuquan.Level != adapter.OffLineNum
                    && curDuquan.Radius != 0 && safeArea.Radius != 0  && rawDuquan.Radius != 0)
                {
                    root.gameObject.SetActive(true);
                    RefreshSlider(interval);
                }
                else
                {
                    root.gameObject.SetActive(false);
                }
            }
        }

        private void RefreshSlider(float interval)
        {
            GetContactPoints(curPlayPos, curDuquan.Center.WorldVector2(), curDuquan.Radius, safeArea.Center.WorldVector2(), safeArea.Radius, rawDuquan.Center.WorldVector2(), rawDuquan.Radius);
            RefreshSliderProcess(interval);
            RefreshSliderTime(interval);
        }

        private void GetContactPoints(Vector2 curPlayPos, Vector2 duQuanPoint, float duQuanRadius, Vector2 safePointer, float safeRadius, Vector2 rawDuquanPos, float rawDuquanRadius)
        {
            //求出与毒圈的交点
            List<Vector2> points = GetIntersectPoint(curPlayPos, safePointer, rawDuquanPos, rawDuquanRadius);
            int i = FilterSameDirecVector2(curPlayPos, safePointer, points);
            if (i < points.Count)
            {
                intersectPointWithRaw = points[i];
            }

            // 求出安全区的交点
            points.Clear();
            points = GetIntersectPoint(curPlayPos, safePointer, safePointer, safeRadius);
            i = FilterSameDirecVector2(curPlayPos, safePointer, points);
            if (i < points.Count)
            {
                intersectPointWithSafe = points[i];
            }

            //求出移动毒圈的交点
            points.Clear();
            points = GetIntersectPoint(curPlayPos, safePointer, duQuanPoint, duQuanRadius);
            i = FilterSameDirecVector2(curPlayPos, safePointer, points);
            if (i < points.Count)
            {
                intersectPointWithMoving = points[i];
            }
        }
        private List<Vector2> GetIntersectPoint(Vector2 linePoint1, Vector2 linePoint2, Vector2 circlePointe, float cirleRadius)
        {
            List<Vector2> temList = new List<Vector2>();
            Vector2 temJiaoDian1 = Vector2.zero;
            Vector2 temJiaoDian2 = Vector2.zero;

            if (linePoint1.x == linePoint2.x)
            {
                var temper = (float)Math.Sqrt((cirleRadius * cirleRadius) - (circlePointe.x - linePoint1.x) * (circlePointe.x - linePoint1.x));
                temJiaoDian1.x = linePoint1.x;
                temJiaoDian1.y = circlePointe.y + temper;
                temJiaoDian2.x = linePoint1.x;
                temJiaoDian2.y = circlePointe.y - temper;
            }
            else if(linePoint1.y == linePoint2.y)
            {
                var temper = (float)Math.Sqrt((cirleRadius * cirleRadius) - (circlePointe.y - linePoint1.y) * (circlePointe.y - linePoint1.y));
                temJiaoDian1.x = circlePointe.x - temper;
                temJiaoDian1.y = linePoint1.y;
                temJiaoDian2.x = circlePointe.x + temper;
                temJiaoDian2.y = linePoint1.y;
            }
            else
            {
                //此时直线不垂直也不水平 
                /*
                    x = ky + b
                    k = (x2 - x1) / (y2 - y1);
                    b = x1 - y1(x2 - x1) /(y2 - y1)

                    ex^2 + fx+ g =0   //圆方程转换以后生成的一元二次表达式
                    e = (k^2 + 1)
                    f = [ 2k(b - m) - 2n]
                    g = n^2 + (b - m)^2 - r^2                                 
                */

                // 求与当前毒圈的交点  
                float y2y1 = linePoint2.y - linePoint1.y;
                float k = (linePoint2.x - linePoint1.x) / y2y1;
                float b = linePoint1.x - linePoint1.y * (linePoint2.x - linePoint1.x) / (linePoint2.y - linePoint1.y);

                float e = k * k + 1;
                float f = 2 * k * (b - circlePointe.x) - 2 * circlePointe.y;
                float g = circlePointe.y * circlePointe.y + (b - circlePointe.x) * (b - circlePointe.x) - cirleRadius * cirleRadius;

                float temperIntersectY = (float)(-f + Math.Sqrt(f * f - 4 * e * g)) / (2 * e);
                float temperIntesectX = k * temperIntersectY + b;
                float temperIntersectY2 = (float)(-f - Math.Sqrt(f * f - 4 * e * g)) / (2 * e);
                float temperIntesectX2 = k * temperIntersectY2 + b;

                temJiaoDian1.x = temperIntesectX;
                temJiaoDian1.y = temperIntersectY;
                temJiaoDian2.x = temperIntesectX2;
                temJiaoDian2.y = temperIntersectY2;
            }

            temList.Add(temJiaoDian1);
            temList.Add(temJiaoDian2);
            return temList;
        }
        private int FilterSameDirecVector2(Vector2 startPoint, Vector2 endPoint, List<Vector2> points)
        {       
            for(int i = 0; i < points.Count; i++)
            {
                float result = UnityEngine.Vector2.Dot(startPoint - endPoint, points[i] - endPoint); //求出两向量之间的夹角  
                if (result > 0)
                    return i;
            }
            return 0;
        }
      
        private void RefreshSliderProcess(float interval)
        {
            float movingDuquanToSafe = Vector2.Distance(intersectPointWithMoving, intersectPointWithSafe);
            float rawDuquanToSafe = Vector2.Distance(intersectPointWithRaw, intersectPointWithSafe);
            float playToSafe = Vector2.Distance(curPlayPos, intersectPointWithSafe);
            float playToSafeCenter = Vector2.Distance(curPlayPos, safeArea.Center.WorldVector2());

            //更新小人的位置
            if (playToSafeCenter <= safeArea.Radius)    //安全区内
            {
                playerImgRt.anchoredPosition = new Vector2(0 + 3, playerImgRt.anchoredPosition.y);                   //放在终点
                _viewModel.PlayerImgColor = UnityEngine.Color.white;
                isChangeColor = false;
            }
            else if (playToSafeCenter > safeArea.Radius && playToSafe < movingDuquanToSafe)   //安全区和当前毒圈之间
            {
                var adjustedValue = Mathf.Max(-suoduRt.sizeDelta.x + 16, -(playToSafe / rawDuquanToSafe) * suoduRt.sizeDelta.x);
                playerImgRt.anchoredPosition = new Vector2(adjustedValue, playerImgRt.anchoredPosition.y);
                _viewModel.PlayerImgColor = UnityEngine.Color.white;
                isChangeColor = false;
            }
            else if (playToSafe >= movingDuquanToSafe && playToSafe < rawDuquanToSafe)   //原始毒圈以内 移动毒圈以外
            {
                var adjustedValue = Mathf.Max(-suoduRt.sizeDelta.x + 16, -(playToSafe / rawDuquanToSafe) * suoduRt.sizeDelta.x);
                playerImgRt.anchoredPosition = new Vector2(adjustedValue, playerImgRt.anchoredPosition.y);             
                isChangeColor = true;
            }
            else if(playToSafe >= rawDuquanToSafe)     //原始毒圈以外
            {
                playerImgRt.anchoredPosition = new Vector2(-suoduRt.sizeDelta.x + 16, playerImgRt.anchoredPosition.y);     //放在起点
                isChangeColor = true;
            }

            if(isChangeColor)
            {
                changColorTime += interval;
                if(changColorTime > changeColorTimeInterval)
                {
                    changColorTime = 0;
                    if(_viewModel.PlayerImgColor == UnityEngine.Color.red)
                    {
                        _viewModel.PlayerImgColor = UnityEngine.Color.white;
                    }
                    else if(_viewModel.PlayerImgColor == UnityEngine.Color.white)
                    {
                        _viewModel.PlayerImgColor = UnityEngine.Color.red;
                    }
                    else
                    {
                        _viewModel.PlayerImgColor = UnityEngine.Color.red;
                    }
                }
            }

            //更新进度条的比例  当前毒圈/原始毒圈
            _viewModel.SuoDuValue = Math.Min(1, Vector2.Distance(intersectPointWithMoving, intersectPointWithRaw) / rawDuquanToSafe);
            Logger.DebugFormat("suodu rate: \"{0}\" ", _viewModel.SuoDuValue);
        }

        private void RefreshSliderTime(float interval)
        {
            waitTime -= Time.time - startTime;
            startTime = Time.time;
            if (waitTime > 0)
            {
                _viewModel.TimeGameObjectActiveSelf = true;

                int minter = (int)waitTime / 60;
                int second = (int)waitTime % 60;
                if (minter > 0)
                {
                    _viewModel.TimeText = minter.ToString() + ":" + second.ToString();
                }
                else
                {
                    _viewModel.TimeText = second.ToString();
                }
            }
            else
            {
                _viewModel.TimeText = "0";
                _viewModel.TimeGameObjectActiveSelf = false;
                waitTime = 0;
            }
        }
    }
}    
