using System.Collections.Generic;
using Core.Ui;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UserInputManager.Lib;


namespace App.Shared.Components.Ui
{
    public static class UiDataAdapterComponentExs
    {
//        static IPlayerStateUiAdapter PlayerState(this UiDataAdapterComponent c)
//        {
//            return (IPlayerStateUiAdapter)c.UiAdapters[0];
//        }
//
//        static void Init(this UiDataAdapterComponent c)
//        {
//
//        }
    }
    [Ui, Unique]
    public class UISessionComponent : IComponent
    {
        [DontInitilize] public Dictionary<string, IUiAdapter> UiAdapters;

        [DontInitilize] public List<string> CreateUi;
        [DontInitilize] public Dictionary<string, bool> UiState;

        [DontInitilize] public Dictionary<UiGroup, List<IUiGroupController>> UiGroup;
        [DontInitilize] public List<UiGroup> HideGroup;
        [DontInitilize] public List<KeyHandler> OpenUKeyhandlerList;
    }
}