using App.Shared.Components.Ui;
using System;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IFocusMaskUIAdapter : IAbstractUiAdapter
    {
        bool IsShowCrossHair { get; }
    }
}
