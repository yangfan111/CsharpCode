using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class SafeMiniDis
    {
        private Transform tran;
        private RectTransform rectTransform;
        private Transform line;
        private RectTransform lineRT;
        private RawImage lineImage;
        private ActiveSetter tranActiveSetter;

        private float lastWidth;

        public SafeMiniDis(Transform tran)
        {
            this.tran = tran;
            tranActiveSetter = new ActiveSetter(tran.gameObject);
            tranActiveSetter.Active = true;
            rectTransform = tran.GetComponent<RectTransform>();
            line = tran.Find("line");
            lineRT = line.GetComponent<RectTransform>();
            lineImage = line.GetComponent<RawImage>();
        }

        public void Hide()
        {
            tranActiveSetter.Active = false;
        }

        public void Update(DuQuanInfo safeDuquan, Vector2 selfPlayPos, float playItemModelWidth, float rate, float miniMapRepresentWHByRice)
        {
            var salfCenter = safeDuquan.Center.ShiftedUIVector2();
            // 安全区和当前玩家的连线的方向
            if (safeDuquan.Level != 0 && safeDuquan.Radius > 0 && Vector2.Distance(selfPlayPos, salfCenter) > safeDuquan.Radius)  //安全区外
            {
                tranActiveSetter.Active = true;
                {
                    Vector2 fromVector = new Vector2(0, 1);
                    Vector3 temper = salfCenter - selfPlayPos;
                    Vector2 toVector = new Vector3(temper.x * -1, temper.y, temper.z);  // 以玩家为中心的坐标系 转化unity的3d坐标系
                    float angle = Vector2.Angle(fromVector, toVector) - 180; //求出两向量之间的夹角  
                    Vector3 normal = Vector3.Cross(fromVector, toVector);//叉乘求出法线向量  
                    if (normal.z < 0)
                    {
                        angle = 360 - angle;
                    }
                    line.localEulerAngles = new UnityEngine.Vector3(0, 0, -angle % 360);

                    //控制连线的长度
                    var width = temper.magnitude * rate - safeDuquan.Radius * rate;
                    var showWidth = miniMapRepresentWHByRice * rate;
                    var rect = lineImage.uvRect;
                    if (width > showWidth)
                    {
                        if (lastWidth > width)
                        {
                            rect.y -= (lastWidth - width) / 16;
                        }
                        else if (lastWidth < width)
                        {
                            rect.y -= (lastWidth - width) / 16;
                        }

                        rect.y = rect.y % 1;
                        lastWidth = width;
                        width = showWidth;
                    }
                    else
                    {
                        rect.height = width / 16;
                    }
                    lineRT.sizeDelta = new Vector2(2, width);
                    lineImage.uvRect = rect;

                   
                    //控制pivot
                    //                    lineRT.pivot = new Vector2(0.5f, - playItemModelWidth / (2 * width));
                    lineRT.pivot = new Vector2(1f,1f);
                    lineRT.anchoredPosition = selfPlayPos * rate;
                }
            }
            else
            {
                tranActiveSetter.Active = false;
            }
        }
    }
}