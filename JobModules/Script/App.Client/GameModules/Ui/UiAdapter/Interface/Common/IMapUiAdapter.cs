using Entitas;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;
using Core.Ui.Map;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IMapUiAdapter : IMiniMapUiAdapter
    {
        string ChannelName { get; }
        string RoomName { get; }
        int PlayerCount { get; }
        int PlayerCapacity { get; }
        EUICampType MyCamp { get; }
        string GetWinConditionDescription();
        string GetModeDescription();
    }
}
