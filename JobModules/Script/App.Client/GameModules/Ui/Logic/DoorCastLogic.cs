using System;
using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components.SceneObject;
using App.Shared.Player;
using App.Shared.SceneTriggerObject;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using RuntimeDebugDraw;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Logic
{
    public class DoorCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DoorCastLogic));

        private MapObjectContext _doorContext;
        private IUserCmdGenerator _userCmdGenerator;
        private TriggerObjectManager _triggerObjectManager;
        private bool _enableAction;
        private int _doorObjId;
        private bool _open;

        public DoorCastLogic(MapObjectContext mapObjectContext, PlayerContext playerContext, IUserCmdGenerator cmdGenerator, float maxDistance) : base(playerContext, maxDistance)
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
           }
        }

        protected override void DoSetData(PointerData data)
        {
            _doorObjId = DoorCastData.EntityId(data.IdList);

            var doorObj = _triggerObjectManager.Get(ETriggerObjectType.Door, _doorObjId);
            if (doorObj == null)
            {
                Logger.Debug("Door trigger object is null.");
                return;
            }

            _enableAction = false;

            var player = _playerContext.flagSelfEntity;
            var go = doorObj;
            if (IsUntouchableOffGround(player, data.Position, go))
            {
                Logger.Debug("Door is untouchable to player.");
                return;
            }

            //make sure player-door have a correct relative position
            var direction = player.RootGo().transform.position - go.transform.position;
            direction.y = 0;
            var dot = Vector3.Dot(direction, go.transform.forward);

            if (!dot.Equals(0))
            {
                var mapObj = _doorContext.GetEntityWithEntityKey(new EntityKey(_doorObjId, (int) EEntityType.MapObject));
                if (mapObj == null || !mapObj.hasDoorData)
                {
                    if (DetectObstacle(go, dot<0))
                    {
                        SetObstacleTip();
                    }
                    else
                    {
                        Tip = ScriptLocalization.client_actiontip.opendoor;
                        _enableAction = true;
                    }
                }
                else
                {
                    var state = mapObj.doorData.State;
                    if (state == (int) DoorState.Closed)
                    {
                        if (DetectObstacle(go, dot<0))
                        {
                            SetObstacleTip();
                        }
                        else
                        {
                            Tip = ScriptLocalization.client_actiontip.opendoor;
                            _enableAction = true;
                        }
                    }
                    else if (state == (int) DoorState.OpenMax || state == (int) DoorState.OpenMin)
                    {
                        if ( (state==(int)DoorState.OpenMax && DetectObstacle(go, true))
                            || (state==(int)DoorState.OpenMin && DetectObstacle(go,false)))
                        {
                            SetObstacleTip();
                        }
                        else
                        {
                            Tip = ScriptLocalization.client_actiontip.closedoor;
                            _enableAction = true;
                        }
                    }
                    else
                    {
                        Logger.Debug("Door state is wrong.");
                    }
                }
            }
            else
            {
                Logger.Debug("Door operation angle failed.");
            }
        }

        private void SetObstacleTip()
        {
            Tip = ScriptLocalization.forbidOpenDoor;
            _enableAction = false;
        }
        
        private bool DetectObstacle(GameObject go, bool isPositiveDirect)
        {
            bool hasObstacle = false;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                if (child.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast))
                {
                    var forwardOfDoor = isPositiveDirect ? go.transform.forward : -go.transform.forward;
                    var width = Math.Max(child.localScale.x, child.localScale.z);
                    var thick = Math.Min(child.localScale.x, child.localScale.z);
                    
                    var extents = new Vector3(width- 2 * thick, child.localScale.y-thick, width - 2 * thick) / 2;
                    
                    var center = child.position;
                    center += forwardOfDoor.normalized * (width) / 2;
                    
                    if (Physics.CheckBox(center, extents, child.rotation, UnityLayers.DoorCastLayerMask))
                    {
                        hasObstacle = true;
                    }
                }
            }

            return hasObstacle;
        }
    }
}
