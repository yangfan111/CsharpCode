using System.Collections.Generic;
using App.Shared.Components.Ui;
using Core.CameraControl.NewMotor;
using Utils.Configuration;
using WeaponConfigNs;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class HurtedUiAdapter : UIAdapter,IHurtedUiAdapter
    {
        private Contexts _contexts;
        private PlayerContext _playerContext;
        private UiContext _uiContext;
        public HurtedUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _playerContext = contexts.player;
            _uiContext = contexts.ui;
        }

        public Dictionary<int, CrossHairHurtedData> HurtedDataList
        {
            get
            {
                return _contexts.ui.uI.HurtedDataList;
            }
        }

        public PlayerEntity GetPlayerEntity()
        {
            return _contexts.player.flagSelfEntity;
        }
    }
}
