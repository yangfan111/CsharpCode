using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using UIComponent.UI;
using UnityEngine;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common.Tip
{
    public class WeaponPropertyBarItem : UIItem
    {
        private float barWidth;

        private WeaponPropertyBarItemViewModel viewModel = new WeaponPropertyBarItemViewModel();

        protected override IUiViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

        protected override void SetView()
        {
            barWidth = viewModel.ProgressBgSizeDelta.x;
            var itemData = Data as ContrastPropertyItemData;

            viewModel.ProgressBgActiveSelf = !itemData.IsBullet;
            viewModel.CurrentBarImageActiveSelf = !itemData.IsBullet;
            viewModel.MissBarImageActiveSelf = !itemData.IsBullet;
            viewModel.AddBarImageActiveSelf = !itemData.IsBullet;
            viewModel.DownImageActiveSelf = !itemData.IsBullet;
            viewModel.UpImageActiveSelf = !itemData.IsBullet;

            viewModel.PropertyNameText = itemData.Name;

            if (itemData.IsBullet)
            {
                viewModel.NumberText = itemData.Bullet + "/" + itemData.MaxBullet;
                if (itemData.DiffValue > 0)
                {
                    viewModel.UpImageActiveSelf = true;
                }
                else if (itemData.DiffValue < 0)
                {
                    viewModel.DownImageActiveSelf = true;
                }
            }
            else
            {
                viewModel.NumberText = "";

                if (itemData.DiffValue > 0)
                {
                    viewModel.MissBarImageActiveSelf = false;
                    viewModel.AddBarImageActiveSelf = true;
                    viewModel.DownImageActiveSelf = false;
                    viewModel.UpImageActiveSelf = true;

                    viewModel.CurrentBarImageSizeDelta = new Vector2(barWidth * (itemData.Value - itemData.DiffValue) / WeaponProperty.UpperLimit, viewModel.CurrentBarImageSizeDelta.y);

                    var pos = viewModel.CurrentBarImageAnchoredPosition;
                    viewModel.AddBarImageAnchoredPosition = new Vector2(pos.x + viewModel.CurrentBarImageSizeDelta.x, pos.y);
                    viewModel.AddBarImageSizeDelta = new Vector2(barWidth * itemData.DiffValue / WeaponProperty.UpperLimit, viewModel.CurrentBarImageSizeDelta.y);

                }
                else if (itemData.DiffValue < 0)
                {
                    viewModel.MissBarImageActiveSelf = true;
                    viewModel.AddBarImageActiveSelf = false;
                    viewModel.DownImageActiveSelf = true;
                    viewModel.UpImageActiveSelf = false;

                    viewModel.CurrentBarImageSizeDelta = new Vector2(barWidth * (itemData.Value) / WeaponProperty.UpperLimit, viewModel.CurrentBarImageSizeDelta.y);
                    var pos = viewModel.CurrentBarImageAnchoredPosition;
                    viewModel.MissBarImageAnchoredPosition = new Vector2(pos.x + viewModel.CurrentBarImageSizeDelta.x, pos.y);
                    viewModel.MissBarImageSizeDelta = new Vector2(-barWidth * itemData.DiffValue / WeaponProperty.UpperLimit, viewModel.CurrentBarImageSizeDelta.y);
                }
                else
                {
                    viewModel.CurrentBarImageSizeDelta = new Vector2(barWidth * (itemData.Value) / WeaponProperty.UpperLimit, viewModel.CurrentBarImageSizeDelta.y);
                    viewModel.MissBarImageActiveSelf = false;
                    viewModel.AddBarImageActiveSelf = false;
                    viewModel.DownImageActiveSelf = false;
                    viewModel.UpImageActiveSelf = false;
                }
            }

        }


    }
}
