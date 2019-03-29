using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class SafeMiniDis
    {
        private Transform tran;
        private RectTransform rectTransform;
        private Transform line;
        private RectTransform lineRT;
        public SafeMiniDis(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            line = tran.Find("line");
            lineRT = line.GetComponent<RectTransform>();
        }

        public void Update(DuQuanInfo safeDuquan, Vector2 selfPlayPos, float playItemModelWidth, float rate, float miniMapRepresentWHByRice)
        {
            // 安全区和当前玩家的连线的方向
            if (safeDuquan.Level != 0 && safeDuquan.Radius > 0 && Vector2.Distance(selfPlayPos, safeDuquan.Center) > safeDuquan.Radius)  //安全区外
            {
                UIUtils.SetActive(tran, true);
                {
                    Vector2 fromVector = new Vector2(0, 1);
                    Vector3 temper = safeDuquan.Center - selfPlayPos;
                    Vector2 toVector = new Vector3(temper.x * -1, temper.y, temper.z);  // 以玩家为中心的坐标系 转化unity的3d坐标系
                    float angle = Vector2.Angle(fromVector, toVector); //求出两向量之间的夹角  
                    Vector3 normal = Vector3.Cross(fromVector, toVector);//叉乘求出法线向量  
                    if (normal.z < 0)
                    {
                        angle = 360 - angle;
                    }
                    line.localEulerAngles = new UnityEngine.Vector3(0, 0, -angle % 360);

                    //控制连线的长度
                    var width = temper.magnitude * rate - safeDuquan.Radius * rate - playItemModelWidth / 2;
                    var showWidth = miniMapRepresentWHByRice * rate;
                    if (width > showWidth) width = showWidth;
                    lineRT.sizeDelta = new Vector2(2, width);

                    //控制pivot
                    lineRT.pivot = new Vector2(0.5f, - playItemModelWidth / (2 * width));
                    lineRT.anchoredPosition = selfPlayPos * rate;
                }
            }
            else
            {
                UIUtils.SetActive(tran, false);
            }
        }
    }
}