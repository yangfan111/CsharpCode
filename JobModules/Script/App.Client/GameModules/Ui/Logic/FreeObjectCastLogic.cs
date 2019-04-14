using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using Assets.Sources.Free.Utility;
using com.cpkf.yyjd.tools.util;
using Core.EntityComponent;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Logic
{
    public class FreeObjectCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FreeObjectCastLogic));
        private FreeMoveContext _freeMoveContext;
        private PlayerContext _playerContext;
        private IUserCmdGenerator _userCmdGenerator;
        private UnityEngine.Vector3 _centeroffset; 

        public FreeObjectCastLogic(
            FreeMoveContext freeMoveContext,
            PlayerContext playerContext,
            IUserCmdGenerator cmdGenerator,
            float distance) : base(playerContext, distance)
        {
            _userCmdGenerator = cmdGenerator;
            _freeMoveContext = freeMoveContext;
            _playerContext = playerContext;
        }

        public override void OnAction()
        {
            _userCmdGenerator.SetUserCmd((cmd) => cmd.IsTabDown = true);
        }

        protected override void DoSetData(PointerData data)
        {
            var player = _playerContext.flagSelfEntity;
            if(null == player)
            {
                Logger.Error("player self entity is null ");
            }
            var freeMoveEntity = _freeMoveContext.GetEntityWithEntityKey(new EntityKey(FreeObjectCastData.EntityId(data.IdList), (short)EEntityType.FreeMove));
            if(null == freeMoveEntity)
            {
                Logger.Error("free move entity is null !");
                return;
            }

            if (IsUntouchableOffGround(player, freeMoveEntity.position.Value, freeMoveEntity.unityGameObject.UnityObject))
            {
                Tip = "";
                return;
            }
            switch(freeMoveEntity.freeData.Cat)
            {
                case FreeEntityConstant.DeadBoxGroup:
                    if(freeMoveEntity.hasFreeData)
                    {
                        var freeDataValue = freeMoveEntity.freeData.Value;
                        if (StringUtil.IsNullOrEmpty(freeDataValue))
                        {
                            Logger.Error("dead box free data value is missing.");
                        }
                        else
                        {
                            Tip = string.Format(ScriptLocalization.client_actiontip.deadbox, freeDataValue);
                        }
                    }
                    else
                    {
                        Logger.Error("no free data attached to free move entity ");
                    }
                    break;
                case FreeEntityConstant.DropBoxGroup:
                    Tip = ScriptLocalization.client_actiontip.dropbox;
                    break;
                default:
                    Tip = "";
                    Logger.ErrorFormat("illegal category {0}", freeMoveEntity.freeData.Cat);
                    break;
            }
        }
    }
}
