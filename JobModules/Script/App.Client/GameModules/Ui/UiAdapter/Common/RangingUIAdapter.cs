using System;
using App.Shared;
using App.Client.GameModules.Ui.UiAdapter;
using Core.Free;
using Free.framework;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class RangingUiAdapter : UIAdapter, IRangingUiAdapter
    {
        private Contexts _contexts;
        public RangingUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }


        private RangingInfo rangeInfo = null;
        public RangingInfo RangeInfo
        {
            get
            {
                return rangeInfo;
            }

            set
            {
                rangeInfo = value;
            }
        }


        public PlayerEntity GetPlayerEntity()
        {
            return _contexts.player.flagSelfEntity;
        }
    }
}