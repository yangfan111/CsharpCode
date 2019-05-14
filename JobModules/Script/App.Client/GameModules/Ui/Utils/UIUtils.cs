using DG.Tweening;
using System;
using System.Collections.Generic;
using Assets.App.Client.GameModules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Utils
{
    public class UIUtils
    {
        public static Tween CallTween(float startValue, float endValue, Action<float> upddateFunc, Action<float> complateFunc, float during)
        {
            float temperPos = startValue;
            Tween t = DOTween.To(() => temperPos, x => temperPos = x, endValue, during);
            t.OnUpdate(() =>
            {
                if(upddateFunc != null)
                    upddateFunc((float)temperPos);

            }).OnComplete(() =>
            {
                if(complateFunc != null)
                    complateFunc((float)temperPos);
            });
            return t;
        }

        public class SimplePool
        {
            private Transform model;
            private Transform parent;
            private List<Transform> poolList = new List<Transform>();
            private HashSet<Transform> usingList = new HashSet<Transform>();  //提高性能 原来是list

            public SimplePool(Transform _model, Transform _parent)
            {
                this.model = _model;
                this.parent = _parent;
            }

            public Transform SpawnGo()
            {
                Transform tran = null;
                if (poolList.Count > 0)
                {
                    tran = poolList[0];
                    poolList.RemoveAt(0);
                }
                else
                {
                    tran = GameObject.Instantiate(model, parent, true);
                }
                tran.SetAsLastSibling();
                usingList.Add(tran);
                UIUtils.SetActive(tran, true);
                return tran;
            }
            public void DespawnGo(Transform go)
            {
                //go.gameObject.SetActive(false);
                if(go.transform.position != new Vector3(5000, 5000, 0))  //提高性能
                    go.transform.position = new Vector3(5000, 5000, 0);
                if (usingList.Contains(go))
                {
                    usingList.Remove(go);
                }
                poolList.Add(go);
            }
            public void DespawnAllGo()
            {
                foreach (var go in usingList)
                {
                    //go.gameObject.SetActive(false);  
                    if (go.transform.localPosition != new Vector3(5000, 5000, 0))  //提高性能
                        go.transform.localPosition = new Vector3(5000, 5000, 0);
                    poolList.Add(go);
                }
                usingList.Clear();
            }

            public HashSet<Transform> GetUsingList()
            {
                return usingList;
            }          
        }
         
        public class MathUtil
        {
            public struct ContactResult
            {
                bool isContact;
                Vector2 contactPoint;

                public bool IsContact
                {
                    get { return isContact; }
                    set { isContact = value; }
                }

                public Vector2 ContactPoint
                {
                    get { return contactPoint; }
                    set { contactPoint = value; }
                }

                public ContactResult(bool isContact, Vector2 contactPoint)
                {
                    this.isContact = isContact;         //在正方形内则为false  否则为true表示有交点
                    this.contactPoint = contactPoint;   //像素位置  正方形内返回当前位置 否则返回与正方形的交点位置
                }
            }

            /*
             * 判断一个点是否在正方形内 如果不在就就去该点和正方形的交点 否则返回当前点
             * 所有参数都是 以米 作为实际单位 正方形中心、宽度、高度、一个偏移相对于正方形的四条边的偏移、 检测点， 比例 offset = true 表示向内偏移 = false 表示想外偏移
             */
            public static ContactResult IsInSquare(Vector2 centerPoint, float centerWidth, float centerHeight, Vector2 offset, bool offsetDirect, Vector2 checkedPoint)
            {
                float deltaX = checkedPoint.x - centerPoint.x;
                float deltaY = checkedPoint.y - centerPoint.y;
                var rectSize = Vector2.zero;
                if (offsetDirect)
                {
                    if ((Math.Abs(deltaX) <= Math.Abs(centerWidth / 2 - offset.x)) && (Math.Abs(deltaY) <= Math.Abs(centerHeight / 2 - offset.y)))
                    {
                        var point = checkedPoint - centerPoint;
                        return new ContactResult(false, point);
                    }
                    else
                    {
                        var point = CaculateIntersectPoint(centerPoint, (centerWidth / 2 - offset.x), (centerHeight / 2 - offset.y), checkedPoint);
                        return new ContactResult(true, point - centerPoint);
                    }
                }
                else
                {
                    if ((Math.Abs(deltaX) <= Math.Abs(centerWidth / 2 + offset.x)) && (Math.Abs(deltaY) <= Math.Abs(centerHeight / 2 + offset.y)))
                    {
                        var point = checkedPoint - centerPoint;
                        return new ContactResult(true, point);
                    }
                    else
                    {
                        var point = CaculateIntersectPoint(centerPoint, (centerWidth / 2 + offset.x), (centerHeight / 2 + offset.y), checkedPoint);
                        return new ContactResult(false, point - centerPoint);
                    }
                }
            }

            public static Vector2 CaculateIntersectPoint(Vector2 centerPoint, float halfWidth, float halfHeight, Vector2 checkedPoint)
            {
                //计算当前小队队员位置和小地图中处的位置的连线 与小地图正方形的交点 单位米
                if (centerPoint.x == checkedPoint.x)         //垂直
                {
                    if (checkedPoint.y > centerPoint.y)
                    {
                        return new Vector2(centerPoint.x, centerPoint.y + halfHeight);
                    }
                    else if (checkedPoint.y < centerPoint.y)
                    {
                        return new Vector2(centerPoint.x, centerPoint.y - halfHeight);
                    }
                }
                else if (centerPoint.y == checkedPoint.y)    //水平
                {
                    if (checkedPoint.x > centerPoint.x)
                    {
                        return new Vector2(centerPoint.x + halfWidth, centerPoint.y);
                    }
                    else if (checkedPoint.x < centerPoint.x)
                    {
                        return new Vector2(centerPoint.x - halfWidth, centerPoint.y);
                    }
                }
                else
                {
                    /*
                    *  y = kx + b
                    *  k = y2 -y1 / x2 -x1  
                    *  b = y2 - k * x2                
                    **/
                    float k = (checkedPoint.y - centerPoint.y) / (checkedPoint.x - centerPoint.x);
                    float b = checkedPoint.y - k * checkedPoint.x;
                    Vector2 checkingVec = new Vector2(checkedPoint.x - centerPoint.x, checkedPoint.y - centerPoint.y);

                    //与正方形的右边相交
                    Vector2 standardVec = new Vector2(halfWidth, 0);
                    if (Vector2.Angle(standardVec, checkingVec) <= 45)  //45 因为设计时小地图的UI的宽高是一样的
                    {
                        float x = centerPoint.x + halfWidth;
                        float y = k * x + b;
                        return new Vector2(x, y);
                    }

                    //与正方形的上边相交
                    standardVec = new Vector2(0, halfHeight);
                    if (Vector2.Angle(standardVec, checkingVec) <= 45)
                    {
                        float y = centerPoint.y + halfHeight;
                        float x = (y - b) / k;
                        return new Vector2(x, y);
                    }

                    //与正方形左边相交
                    standardVec = new Vector2(-halfWidth, 0);
                    if (Vector2.Angle(standardVec, checkingVec) <= 45)
                    {
                        float x = centerPoint.x - halfWidth;
                        float y = k * x + b;
                        return new Vector2(x, y);
                    }

                    //与正方形下边相交
                    standardVec = new Vector2(0, -halfHeight);
                    if (Vector2.Angle(standardVec, checkingVec) <= 45)
                    {
                        float y = centerPoint.y - halfHeight;
                        float x = (y - b) / k;
                        return new Vector2(x, y);
                    }
                }
                return Vector2.zero;
            }
            //按照unity的三维坐标系 计算的 from -》 to 的逆时针方向的夹角
            public static float GetAngle_360(UnityEngine.Vector3 fromVector, UnityEngine.Vector3 toVector)
            {
                float angle = UnityEngine.Vector3.Angle(fromVector, toVector); //求出两向量之间的夹角  
                UnityEngine.Vector3 normal = UnityEngine.Vector3.Cross(fromVector, toVector);//叉乘求出法线向量  
                if (normal.z > 0)  //顺时针方向角度
                {
                    angle = 360 - angle;
                }
                return angle;
            }
        }

        public static void SetActive(GameObject go, bool isActive)
        {
            if (go.activeSelf != isActive)
                go.SetActive(isActive);
        }

        public static void SetActive(Transform go, bool isActive)
        {
            SetActive(go.gameObject,isActive);
        }

        public static void SetEnable(Behaviour behaviour, bool isActive)
        {
            if (behaviour.enabled != isActive)
            {
                behaviour.enabled = isActive;
            }
        }
        /// <summary>
        /// 父节点为显示的最大区域
        /// </summary>
        /// <param name="image"></param>
        public static void SetImageSuitable(Image image)
        {
            if (image == null)
            {
                return;
            }
            float spriteWidth = 0;
            float spriteHeight = 0;
            if (image.sprite != null && image.sprite.texture != null)
            {
                spriteWidth = image.sprite.texture.width;
                spriteHeight = image.sprite.texture.height;
            }
            else
            {
                return;
            }

            AspectRatioFitter arf = image.GetComponent<AspectRatioFitter>();
            if (arf == null)
            {
                arf = image.gameObject.AddComponent<AspectRatioFitter>();
            }
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.aspectRatio = spriteWidth / spriteHeight;
        }

        public static Vector2 WorldPosToRect(Vector3 targetPos, RectTransform rect)
        {
            var cam = Camera.main;
            return WorldPosToRect(targetPos, cam, rect);
        }

        public static Vector2 WorldPosToRect(Vector3 targetPos, Camera camera, RectTransform rect)
        {
            Vector2 position = camera.WorldToScreenPoint(targetPos);
            Vector2 result = Vector2.zero;
            if (UiCommon.UIManager.GetRootRenderMode().Equals(RenderMode.ScreenSpaceCamera))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, position, UiCommon.UIManager.UICamera,
                    out result);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, position, null, out result);
            }

            return result;
        }

        public static bool InView(Vector3 pos)
        {
            var cam = Camera.main;
            Vector2 viewPos = cam.WorldToViewportPoint(pos);
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
