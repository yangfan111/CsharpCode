using App.Client.GameModules.Ui.UiAdapter;
using Core.Enums;
using UIComponent.UI;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Group
{
    public abstract class AbstractRecordInfo : UIItem
    {

        protected override void SetView()
        {
            base.SetView();
            UpdateInfoView();
        }

        public void UpdateInfoView()
        {
            if (!_viewInitialized) return;
            var data = Data as GroupRecordViewData;

            UpdateGroupShow(data);
            var needShow = data.NeedShow && !data.IsTitle;
            if (!needShow) return;
            UpdateSingleItemData(data);
            UpdateSingleItemIcon(data);
            UpdateSingleItemMySelf(data);
            UpdateSingleItemBadge(data);
        }

        protected int curBadgeId;

        protected abstract void UpdateSingleItemBadge(GroupRecordViewData data);

        protected AssetInfo GetBadgeAssetInfo(int id)
        {
            return SingletonManager.Get<CardConfigManager>().GetAssetInfoById(id);
        }

        protected void UpdateGroupShow(GroupRecordViewData data)
        {
            RealUpdateGroupShow(data);
            if (data.IsTitle)
            {
                InitTitleText(data.CanResque);
                UpdateSizeByFatherRoot();
            }
        }

        protected abstract void RealUpdateGroupShow(GroupRecordViewData data);
        protected abstract void InitTitleText(bool canResque);

        protected void UpdateSizeByFatherRoot()
        {
            var rootRtf = ViewInstance.transform as RectTransform;
            rootRtf.sizeDelta = (rootRtf.parent.transform as RectTransform).rect.size;
        }

        protected Color origColor = new Color32(0xff, 0xff, 0xff, 255);
        protected Color myselfColor = new Color32(0xfd, 0xcd, 0x50, 255);
        protected Color deadColor = new Color32(0x82, 0x82, 0x82, 255);

        protected void UpdateSingleItemMySelf(GroupRecordViewData viewData)
        {
            var data = viewData.BattleData;
            if (data.IsMySelf)
            {
                UpdateColor(myselfColor);
            }
            else if (data.IsDead)
            {
                UpdateColor(deadColor);
            }
            else
            {
                UpdateColor(origColor);
            }
        }

        protected abstract void UpdateColor(Color color);

        protected abstract void UpdateSingleItemIcon(GroupRecordViewData viewData);

        protected bool IsTitle(EUIGameTitleType titleType, int title)
        {
            int type = 1 << (int) titleType;
            return (title & type) == type;
        }

        protected string twoDataFormat = "{0}/{1}";

        protected abstract void UpdateSingleItemData(GroupRecordViewData viewData);
    }
}