using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.CastObjectUtil;
using App.Client.GameModules.SceneObject;
using App.Shared;
using App.Shared.Components.SceneObject;
using App.Shared.Player;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Logic
{
    public class DoorCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DoorCastLogic));
        private MapObjectContext _doorContext;
        private PlayerContext _playerContext;
        private IUserCmdGenerator _userCmdGenerator;
        private TriggerObjectManager _triggerObjectManager;
        private bool _enableAction;
        private int _doorObjId;
        private bool _open;

        public DoorCastLogic(
            MapObjectContext mapObjectContext,
            PlayerContext playerContext,
            IUserCmdGenerator cmdGenerator,
            float maxDistance) : base(playerContext, maxDistance)
        {
            _doorContext = mapObjectContext;
            _playerContext = playerContext;
            _userCmdGenerator = cmdGenerator;
            _triggerObjectManager = SingletonManager.Get<TriggerObjectManager>();
        }

        public override void OnAction()
        {
            if (_enableAction)
            {
                _userCmdGenerator.SetUserCmd((cmd) => cmd.IsUseAction = true);
                _userCmdGenerator.SetUserCmd((cmd) => cmd.UseEntityId = _doorObjId);
                _userCmdGenerator.SetUserCmd((cmd) => cmd.UseType = (int)EUseActionType.Door);
                var player = _playerContext.flagSelfEntity;
                //if(null != player)
                //{
                //    if(_open)
                //    {
                //        player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.OpenDoor);
                //    }
                //    else
                //    {
                //        player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.CloseDoor);
                //    }
                //}
                //else
                //{
                //    Logger.Error("self entity in player context is null");
                //}
           }
        }

        protected override void DoSetData(PointerData data)
        {
            _doorObjId = DoorCastData.EntityId(data.IdList);

            var doorObj = _triggerObjectManager.Get(ETriggerObjectType.Door, _doorObjId);
            if (doorObj == null)
            {
                return;
            }

            _enableAction = false;

            var player = _playerContext.flagSelfEntity;
            var go = doorObj;
            if (IsUntouchableOffGround(player, data.Position, go))
            {
                return;
            }

            //make sure player-door have a correct relative position
            var direction = player.RootGo().transform.position - go.transform.position;
            direction.y = 0;
            var dot = Vector3.Dot(direction, go.transform.forward);

            if (!dot.Equals(0))
            {
//                var mapObj = MapObjectUtility.GetMapObjByRawId(_doorObjId, (int) ETriggerObjectType.Door);
                var mapObj = _doorContext.GetEntityWithEntityKey(new EntityKey(_doorObjId, (int) EEntityType.MapObject));
                if (mapObj == null)
                {
                    Tip = ScriptLocalization.client_actiontip.opendoor;
                    _enableAction = true;
                }
                else
                {
                    var state = mapObj.doorData.State;
                    if (state == (int) DoorState.Closed)
                    {
                        Tip = ScriptLocalization.client_actiontip.opendoor;
                        _enableAction = true;
                    }
                    else if (state == (int) DoorState.OpenMax || state == (int) DoorState.OpenMin)
                    {
                        Tip = ScriptLocalization.client_actiontip.closedoor;
                        _enableAction = true;
                    }
                }
            }
        }
    }
}
