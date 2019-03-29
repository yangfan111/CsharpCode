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

        public void Update(List<MiniMapTeamPlayInfo> TeamInfos, float rate)
        {
            if (TeamInfos.Count > 0)
            {
                for (int i = 0; i < TeamInfos.Count; i++)
                {
                    if (TeamInfos[i].MarkList != null && TeamInfos[i].MarkList.Count > 0)
                    {
                        foreach (var markInfo in TeamInfos[i].MarkList)//目前只能有一个标记点，如果有多个不能用palyerid作为key
                        {
                            RefreshMarkItem(TeamInfos[i], markInfo, rate);
                        }
                    }
                    else
                    {
                        MapMarkItem markItem;
                        markItemDic.TryGetValue(TeamInfos[i].PlayerId, out markItem);
                        if (markItem != null) markItem.SetActive(false);
                    }
                }
            }
        }
        private void RefreshMarkItem(MiniMapTeamPlayInfo data, MiniMapPlayMarkInfo markData, float rate)
        {
            MapMarkItem markItem;
            markItemDic.TryGetValue(data.PlayerId, out markItem);
            if (markItem == null)
            {
                markItem = new MapMarkItem(GameObject.Instantiate(markModel, tran, true));
                markItemDic[data.PlayerId] = markItem;
            }
            markItem.SetActive(true);
            markItem.Update(data, rate, markData);
        }
    }

    public class MapMarkItem
    {
        private Transform tran;
        private RectTransform rectTransform;
        private Image img;

        public bool IsOutDate;

        public MapMarkItem(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            img = tran.GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            UIUtils.SetActive(tran, active);
        }

        private MiniMapPlayStatue _statue;
        private Color _color;
        private float _rate;
        private Vector2 _pos;
        public void Update(MiniMapTeamPlayInfo teamPlayInfo, float rate, MiniMapPlayMarkInfo data)
        {
            if (teamPlayInfo.Statue.Equals(_statue) && teamPlayInfo.Color.Equals(_color) && rate.Equals(_rate) && data.Pos.Equals(_pos)) return;
            _statue = teamPlayInfo.Statue;
            _color = teamPlayInfo.Color;
            _pos = data.Pos;
            _rate = rate;

            if (_statue == MiniMapPlayStatue.DEAD) //死亡
            {
                UIUtils.SetActive(tran, false);
            }
            else
            {
                UIUtils.SetActive(tran, true);
//                var temperSprite = SpriteComon.GetInstance().GetSpriteByName("fix_00");
//                if (temperSprite != null && temperSprite != img.sprite)
//                    img.sprite = temperSprite;
                img.color = _color;

                var pos = data.Pos * rate;
                rectTransform.anchoredPosition = pos;                  //更新标记位置
            }
        }

        
    }
}