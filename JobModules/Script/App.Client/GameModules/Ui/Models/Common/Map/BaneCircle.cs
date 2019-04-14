using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class BaneCircle
    {
        private float lineWidth = 1.5f;                 //毒圈、安全区的线条宽度

        private Transform tran;
        private RectTransform rectTransform;
        private Image img;
        private Material material;

        public BaneCircle(Transform tran, bool isBane, bool isMiniMap = true)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            img = tran.GetComponent<Image>();
            material = img.material;

            UnityEngine.Color boundColor = UnityEngine.Color.white;
            if (isBane)
            {
                boundColor = new Color(91 / 255f, 255 / 255f, 20 / 255f, 0.58f);
            }
            else
            {
                if (isMiniMap)
                    boundColor = new Color(boundColor.r, boundColor.g, boundColor.b, 0.58f);
            }
            

            material.SetColor("_BoundColor", boundColor);
            material.SetFloat("_ComponentWidth", rectTransform.rect.width);
        }

        public void Update(DuQuanInfo duQuanInfo, Vector2 selfPlayPos, float rate, float windowWidthByRice)
        {
            Vector2 referPosByPixel = Vector2.zero;

            var duquanPos = duQuanInfo.Center.ShiftedUIVector2();
            if (duQuanInfo.Level == 0 || duQuanInfo.Radius == 0 || UnityEngine.Vector2.Distance(selfPlayPos, duquanPos) > 1.414f * windowWidthByRice / 2 + duQuanInfo.Radius) //不在地图视野内
            {
                UIUtils.SetActive(tran, false);
            }
            else
            {
                UIUtils.SetActive(tran, true);
                
                //设置大小
                float beishu = (duQuanInfo.Radius * rate) / (rectTransform.rect.width / 2);
                float beishuDaosu = (rectTransform.rect.width / 2) / (duQuanInfo.Radius * rate);
                var tilingX = Mathf.Max(beishuDaosu, 0);
                float tilingY = tilingX;
                material.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));

                //设置位置    
                Vector2 startPoint = referPosByPixel + (duquanPos - selfPlayPos) * rate;
                var halfW = rectTransform.rect.width / 2;
                Vector2 endPoint = referPosByPixel - new Vector2(halfW, halfW) + new Vector2(beishu * halfW, beishu * halfW);
                var deltaX = (endPoint.x - startPoint.x) / (beishu * rectTransform.rect.width);
                var deltaY = (endPoint.y - startPoint.y) / (beishu * rectTransform.rect.width);
                var safeDelat = new Vector2(deltaX, deltaY);
                material.SetTextureOffset("_MainTex", safeDelat);
                material.SetFloat("_BoundWidth", lineWidth * beishuDaosu);
            }
        }
    }
}