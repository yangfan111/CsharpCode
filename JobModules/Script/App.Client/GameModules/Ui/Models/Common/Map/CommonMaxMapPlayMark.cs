using App.Client.GameModules.Ui.Common.MaxMap;
using UnityEngine;
using DG.Tweening;
using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public partial class CommonMaxMap
    {
        private Transform playItemRoot = null;
        private Transform playItemModel = null;
        private UnityEngine.Vector2 playRefePosByPixel = new UnityEngine.Vector2(0, 0);   //像素
        private UnityEngine.Vector2 PlayRefePosByRice = new UnityEngine.Vector2(0, 0);       //米

        private Transform markRoot = null;
        private Transform markModel = null;
        private Vector2 markRefePosByPixel = Vector2.zero;  //标记的参考点 像素
        private Vector2 markRefePosByRice = Vector2.zero; //标记的参考点  米  是当前人物的实际位置

        private Tween iconTween = null;
        float playItemModelWidth = 0;

        UIUtils.SimplePool playPool = null;
        UIUtils.SimplePool markPool = null;
        PlayMarkCommn playMarkUtil = null;

        #region PlayerItem        
        private void InitPlayerItemList()
        {
            if (playItemModel && playItemRoot)
            {
                UIUtils.SetActive(playItemModel, false);
                playPool = new UIUtils.SimplePool(playItemModel, playItemRoot);

                if (playMarkUtil == null)
                    playMarkUtil = new PlayMarkCommn(adapter);
                playMarkUtil.SetPlayAction((info) =>
                {
                    var tran = playPool.SpawnGo();
                    var playData = (MiniMapTeamPlayInfo)info;
                    RefreshPlayTeamItem(tran, playData);
                });
            }
        }
        private void UpdatePlayerItemList()
        {
            if (playItemRoot == null || playItemModel == null || playPool == null)
                return;

            playPool.DespawnAllGo();
            playMarkUtil.UpdatePlayList();           
        }
        private void RefreshPlayTeamItem(Transform tran, MiniMapTeamPlayInfo data)
        {
            //控制位置和方向              
            {
                var direction = tran.Find("direction");
                if (direction == null) return;
                var directRT = direction.GetComponent<RectTransform>();
                var tranRT = tran.GetComponent<RectTransform>();

                var offset = new Vector2(tranRT.sizeDelta.y + directRT.sizeDelta.y, tranRT.sizeDelta.y + directRT.sizeDelta.y) / (2 * rate);
                var result = UIUtils.MathUtil.IsInSquare(PlayRefePosByRice, MaxMapRepresentWHByRice, MaxMapRepresentWHByRice, offset, true, data.Pos);
                tran.GetComponent<RectTransform>().anchoredPosition = playRefePosByPixel + result.ContactPoint * rate;

                if (!result.IsContact)
                {
                    UIUtils.SetActive(direction, false);
                    UIUtils.SetActive(tran, true);
                }
                else
                {
                    UIUtils.SetActive(tran, false);
                    UIUtils.SetActive(direction, false);
//                    {
//                        Vector2 fromVector = new Vector2(0, 1);
//                        Vector2 toVector = tran.GetComponent<RectTransform>().anchoredPosition.normalized;
//
//                        float angle = UnityEngine.Vector2.Angle(fromVector, toVector); //求出两向量之间的夹角  
//                        UnityEngine.Vector3 normal = UnityEngine.Vector3.Cross(fromVector, toVector);//叉乘求出法线向量  
//                        if (normal.z < 0)
//                        {
//                            angle = 360 - angle;
//                        }
//                        direction.localEulerAngles = new UnityEngine.Vector3(0, 0, (angle + 180) % 360);
//                    }
                }
            }

            //编号和编号背景色
            playMarkUtil.UpdatePlayNumAndColor(tran, ref PlayRefePosByRice, data, false);

            //控制朝向
            playMarkUtil.UpdatePlayFaceDirection(tran, data);       
        }
        #endregion


        #region Mark
        private void InitMarkItemList()
        {
            if (markRoot && markModel)
            {
                if (playMarkUtil == null)
                    playMarkUtil = new PlayMarkCommn(adapter);
                playMarkUtil.SetMarkAction((info) =>
                {
                    var tran = markPool.SpawnGo();
                    var markData = (MiniMapPlayMarkInfo)info;
                    RefreshMarkItem(tran, markData);
                });

                UIUtils.SetActive(markModel, false);
                markPool = new UIUtils.SimplePool(markModel, markRoot);
            }
        }
        private void UpdateMarkItemList()
        {
            if (markModel == null || markRoot == null || markPool == null)
                return;
            
            markPool.DespawnAllGo();
            playMarkUtil.UpdateMarkList();
        }
        private void RefreshMarkItem(Transform tran, MiniMapPlayMarkInfo data)
        {
            playMarkUtil.UpdateMarkItemCommon(markPool, tran, rate, data, markRefePosByRice, MaxMapRepresentWHByRice, markRefePosByPixel);            
        }
        #endregion
    }
}    
