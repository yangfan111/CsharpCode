using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class BombArea
    {
        private float lineWidth = 1.5f;                 //毒圈、安全区的线条宽度

        private Transform tran;
        private RectTransform rectTransform;
        private Image img;

        public BombArea(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            img = tran.GetComponent<Image>();
        }

        public void Update(BombAreaInfo curBombAreaInfo, Vector2 selfPlayPos, float rate, float windowWidthByRice, bool isMiniMap = true)
        {
            Vector2 referPosByPixel = Vector2.zero;

            var temperVec = new Vector2(curBombAreaInfo.Center.x, curBombAreaInfo.Center.z);
            if (curBombAreaInfo.Radius.Equals(0) && curBombAreaInfo.Num.Equals(-1) && Vector2.Distance(selfPlayPos, temperVec) > 1.414f * windowWidthByRice / 2 + curBombAreaInfo.Radius) //不在地图视野内
            {
                UIUtils.SetActive(tran, false);
            }
            else
            {
                UIUtils.SetActive(tran, true);
                var material = img.material;
                //设置大小
                float beishu = (curBombAreaInfo.Radius * rate) / (rectTransform.rect.width / 2);
                float beishuDaosu = (rectTransform.rect.width / 2) / (curBombAreaInfo.Radius * rate);
                var tilingX = Mathf.Max(beishuDaosu, 0);
                float tilingY = tilingX;
                material.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));

                //设置位置    
                Vector2 startPoint = referPosByPixel + (temperVec - selfPlayPos) * rate;
                var halfW = rectTransform.rect.width / 2;
                Vector2 endPoint = referPosByPixel - new Vector2(halfW, halfW) + new Vector2(beishu * halfW, beishu * halfW);
                var deltaX = (endPoint.x - startPoint.x) / (beishu * rectTransform.rect.width);
                var deltaY = (endPoint.y - startPoint.y) / (beishu * rectTransform.rect.width);
                material.SetTextureOffset("_MainTex", new Vector2(deltaX, deltaY));

                UnityEngine.Color bColor = Color.white;
                if (isMiniMap)
                    bColor = new Color(255 / 255f, 9 / 255f, 5 / 255f, 102 / 255f);
                else
                    bColor = new Color(255 / 255f, 9 / 255f, 5 / 255f, 102 / 255f);
                material.SetColor("_BoundColor", bColor);
                material.SetFloat("_BoundWidth", rectTransform.rect.width);
                material.SetFloat("_ComponentWidth", rectTransform.rect.width);
            }
        }
    }
}