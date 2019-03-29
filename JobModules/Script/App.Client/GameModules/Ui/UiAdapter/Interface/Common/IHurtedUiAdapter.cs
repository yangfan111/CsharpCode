using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IHurtedUiAdapter : IAbstractUiAdapter
    {
        Dictionary<int, CrossHairHurtedData> HurtedDataList { get; }
        PlayerEntity GetPlayerEntity();
    }
}
