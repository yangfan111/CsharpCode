using System;
using App.Client.GameModules.Ui.Common.MaxMap;
using UnityEngine;
using App.Client.GameModules.Ui.Utils;

namespace App.Client.GameModules.Ui.Models.Common
{
    public partial class CommonMaxMap
    {
        private Vector2 duquanRefePosByPixel = new Vector2(0, 0);           //像素
        private Vector2 duquanRefePosByRice = new Vector2(0, 0);            //米

        private Transform curDuquanRoot = null;
        private Transform safeDuquanRoot = null;
        private Transform miniDisTran = null;
        private Transform curBombAreaRoot = null;
        private DuquanCommon duquanUtil = null;

        private void InitDuQuanAndSafe()
        {
            duquanUtil = new DuquanCommon(adapter);
            duquanUtil.SetActions(UpdateSafe, UpdateDuquan, UpdataMiniDis, UpdateBombArea);
        }
        private void UpdateDuQuanAndSafe()
        {
            duquanUtil.UpdateDuQuanAndSafe(curDuquanRoot, safeDuquanRoot, miniDisTran, curBombAreaRoot);
        }

        private void UpdateSafe()
        {
            duquanUtil.UpdateSafeCommon(false, safeDuquanRoot, duquanRefePosByRice, duquanRefePosByPixel, MaxMapRepresentWHByRice, rate);
        }

        private void UpdateDuquan()
        {
            duquanUtil.UpateDuquanCommon(false, curDuquanRoot, duquanRefePosByRice, duquanRefePosByPixel, MaxMapRepresentWHByRice, rate);           
        }

        private void UpdateBombArea()
        {
            duquanUtil.UpdateBombAreaCommon(false, curBombAreaRoot, duquanRefePosByRice, duquanRefePosByPixel, MaxMapRepresentWHByRice, rate);                    
        }

        private void UpdataMiniDis()
        {
            Vector2 curPlayPos = adapter.CurPlayerPos;
            var safeDuquan = duquanUtil.safeDuquan;

            if (Vector2.Distance(curPlayPos, safeDuquan.Center) > safeDuquan.Radius) //安全区外
            {
                UIUtils.SetActive(miniDisTran, true);

                //计算当前玩家 +  安全区中心点  相对于lineRoot 控件中心的像素位置
                var curPlayPosByPixel = (curPlayPos - duquanRefePosByRice ) * rate;
                var safePosByPixel = (safeDuquan.Center - duquanRefePosByRice) * rate;
               
                //设置方向
                Vector2 from = new Vector2(0, 1);
                Vector2 temper = safePosByPixel - curPlayPosByPixel;
                Vector2 to = new Vector3(temper.x * -1, temper.y);  // 以玩家为中心的坐标系 转化unity的3d坐标系
                {
                    float angle = Vector2.Angle(from, to); //求出两向量之间的夹角  
                    Vector3 normal = Vector3.Cross(from, to);//叉乘求出法线向量  
                    if (normal.z < 0)
                    {
                        angle = 360 - angle;
                    }
                    miniDisTran.localEulerAngles = new Vector3(0, 0, -angle);
                }
                
                //设置长度
                var height = Vector2.Distance(curPlayPosByPixel, safePosByPixel) - safeDuquan.Radius * rate - playItemModelWidth / 2;
                height = Math.Max(height, 2);
                miniDisTran.GetComponent<RectTransform>().sizeDelta = new Vector2(2, height);

                //设置位置
                Vector2 offset = temper.normalized * safeDuquan.Radius * rate;
                var contactPoint = safePosByPixel - offset;               
                Vector2 pos = (contactPoint + curPlayPosByPixel + ((playItemModelWidth / 2) * temper.normalized)) / 2;            
                miniDisTran.GetComponent<RectTransform>().anchoredPosition = pos;
            }
            else
            {
                UIUtils.SetActive(miniDisTran, false);
            }
        }
    }
}    
