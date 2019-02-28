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

        public RouteLine(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            line = tran.Find("routeLine");
            lineRtf = line.GetComponent<RectTransform>();
        }

        public void Updata(bool isShowRouteLine, Vector2 startPosByRice, Vector2 endPosByRice, float rate)
        {
            if (!isShowRouteLine)
            {
                UIUtils.SetActive(tran, false);
            }
            else
            {
                UIUtils.SetActive(tran, true);

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
                float height = Vector2.Distance(startPosByPixel, endPosByPixel);
                lineRtf.sizeDelta = new Vector2(lineRtf.sizeDelta.x, height);
            }
        }
    }
}