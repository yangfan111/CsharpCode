using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Logic
{
    public class MapObjectCastLogic:AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MapObjectCastLogic));
        private MapObjectContext _mapObjectContext;
        private ClientSessionObjectsComponent _sessionObjectsComponent;
        private IUserCmdGenerator _cmdGenerator;
        
        public MapObjectCastLogic(
            PlayerContext playerContext,
            MapObjectContext mapObjectContext,
            ClientSessionObjectsComponent sessionObjectsComponent,
            IUserCmdGenerator cmdGenerator,
            float maxDistance):base(playerContext, maxDistance)
        {
            _mapObjectContext = mapObjectContext;
            _sessionObjectsComponent = sessionObjectsComponent;
            _cmdGenerator = cmdGenerator;
        }
        
        private PointerData _pointerData;


        public override void OnAction()
        {
            return;
        }

        protected override void DoSetData(PointerData data)
        {
            return;
        }
    }
}