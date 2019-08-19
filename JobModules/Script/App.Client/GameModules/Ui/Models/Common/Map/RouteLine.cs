using App.Client.GameModules.Ui.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class RouteLine
    {
        private Transform tran;
        private RectTransform rectTransform;
        private Transform line;
        private RectTransform lineRtf;
        private ActiveSetter tranActiveSetter;

        public RouteLine(Transform tran)
        {
            this.tran = tran;
            tranActiveSetter = new ActiveSetter(tran.gameObject);
            tranActiveSetter.Active = true;
            rectTransform = tran.GetComponent<RectTransform>();
            line = tran.Find("routeLine");
            lineRtf = line.GetComponent<RectTransform>();
        }

        public void Updata(bool isShowRouteLine, Vector2 startPosByRice, Vector2 endPosByRice, float rate)
        {
            if (!isShowRouteLine)
            {
                tranActiveSetter.Active = false;
            }
            else
            {
                tranActiveSetter.Active = true;

                var startPosByPixel = (startPosByRice) * rate;
                var endPosByPixel = (endPosByRice) * rate;

                //更新角度
                Vector2 from = new Vector2(0, 1);   //因为prefab上 默认图片是向右方向
                Vector2 temper = endPosByPixel - startPosByPixel;
                Vector2 to = new Vector3(temper.x * -1, temper.y);  // 以玩家为中心的坐标系 转化unity的3d坐标系
                {
                    float angle = Vector2.Angle(from, to); //求出两向量之间的夹角  
                    Vector3 normal = Vector3.Cross(from, to);//叉乘求出法线向量  
                    if (normal.z < 0)
                    {
                        angle = 360 - angle;
                    }
                    lineRtf.localEulerAngles = new Vector3(0, 0, -angle);
                }

                //更新位置
                Vector2 middlePos = (startPosByPixel + endPosByPixel) / 2;
                lineRtf.anchoredPosition = middlePos;

                //更新大小
                float height = Vector2.Distance(startPosByPixel, endPosByPixel) + 24;//24是头尾两端的圈圈中点位置矫正
                lineRtf.sizeDelta = new Vector2(lineRtf.sizeDelta.x, height);
            }
        }
    }
}