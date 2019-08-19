using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.Utils.MiniMaxMapCommon;
using App.Shared.Components.Ui;
using Core.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapMark
    {
        private Transform tran;
        private Transform markModel = null;
        private Dictionary<long, MapMarkItem> markItemDic;

        public MapMark(Transform tran)
        {
            this.tran = tran;
            markModel = tran.Find("markItem");
            markItemDic = new Dictionary<long, MapMarkItem>();
        }

        public void Update(Dictionary<long, MiniMapPlayMarkInfo>  markInfos, float rate)
        {
            foreach (var markItem in markItemDic)
            {
                if (markInfos == null || !markInfos.ContainsKey(markItem.Key))
                {
                    markItem.Value.SetActive(false);
                }
            }
            if (markInfos == null) return;

            foreach (var markPair in markInfos)
            {
                var markInfo = markPair.Value;
                RefreshMarkItem(markPair.Key, markInfo, rate);
            }
        }
        private void RefreshMarkItem(long playerId, MiniMapPlayMarkInfo markData, float rate)
        {
            MapMarkItem markItem;
            markItemDic.TryGetValue(playerId, out markItem);
            if (markItem == null)
            {
                markItem = new MapMarkItem(GameObject.Instantiate(markModel, tran, true));
                markItemDic[playerId] = markItem;
            }
            markItem.SetActive(true);
            markItem.Update(rate, markData);
        }
    }

    public class MapMarkItem
    {
        private Transform tran;
        private RectTransform rectTransform;
        private Image img;
        private ActiveSetter tranActiveSetter;

        public bool IsOutDate;

        public MapMarkItem(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            img = tran.GetComponent<Image>();
            tranActiveSetter = new ActiveSetter(tran.gameObject);
        }

        public void SetActive(bool active)
        {
            UIUtils.SetActive(tran, active);
        }

        private MiniMapPlayStatue _statue;
        private Color _color;
        private float _rate;
        private Vector2 _pos;
        public void Update(float rate, MiniMapPlayMarkInfo data)
        {
            if (data.Color == _color && rate == _rate && data.Pos == _pos) return;
//            _statue = teamPlayInfo.Statue;
            _color = data.Color;
            _pos = data.Pos;
            _rate = rate;

            //            if (_statue == MiniMapPlayStatue.DEAD) //死亡
            //            {
            //                UIUtils.SetActive(tran, false);
            //            }
            //            else
            //            {
            tranActiveSetter.Active = true;
//                var temperSprite = SpriteComon.GetInstance().GetSpriteByName("fix_00");
//                if (temperSprite != null && temperSprite != img.sprite)
//                    img.sprite = temperSprite;
                img.color = _color;

                var pos = data.Pos * rate;
                rectTransform.anchoredPosition = pos;                  //更新标记位置
//            }
        }

        
    }
}