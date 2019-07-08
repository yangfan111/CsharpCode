using App.Shared.GameModules.Player;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Logic
{
    public class PlayerStateTipLogic : IPlayerStatTipLogic 
    {
        private enum ParachuteState
        {
            None,
            Start,//no tip
            WaitGlide,// jump tip
            Glide,// no tip
            WaitParachute,// parachute tip
        }
        private ParachuteState _parachuteState;
        private IUserCmdGenerator _userCmdGenerator;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerStateTipLogic)); 
        private PlayerContext _playerContext;
        public PlayerStateTipLogic(PlayerContext playerContext, IUserCmdGenerator cmdGenerator)
        {
            _playerContext = playerContext;
            _parachuteState = ParachuteState.Start;
            _userCmdGenerator = cmdGenerator;
        }

        public bool HasTipState()
        {
            UpdateState();
            bool hasTip = false;
            switch(_parachuteState)
            {
                case ParachuteState.None:
                case ParachuteState.Start:
                case ParachuteState.Glide:
                    break;
                default:
                    hasTip = true;
                    break;
            }
            if(MyPlayerEntity.IsOnVehicle() || PlayerStateUtil.HasPlayerState(EPlayerGameState.CanDefuse, MyPlayerEntity.gamePlay))
            {
                hasTip = true;
            }
            
            return hasTip;
        }

        private PlayerEntity MyPlayerEntity
        {
            get
            {
                var player = _playerContext.flagSelfEntity;
                if(null == player)
                {
                    Logger.WarnFormat("self entity in player context is null !");
                }
                return player;
            }
        }

        /// <summary>
        /// 需要先调用HasTipState判断是否有响应的状态
        /// </summary>
        public string StateTip
        {
            get
            {
                var tip = "";
                switch(_parachuteState)
                {
                    case ParachuteState.WaitGlide:
                        tip = ScriptLocalization.client_actiontip.airjump;
                        break;
                    case ParachuteState.WaitParachute:
                        tip = ScriptLocalization.client_actiontip.openparachute;
                        break;
                }

                if (string.IsNullOrEmpty(tip))
                {
                    if (PlayerStateUtil.HasPlayerState(EPlayerGameState.CanDefuse, MyPlayerEntity.gamePlay))
                    {
                        tip = "拆包";
                    }
                }
                return tip;
            }
        }

        public void Action()
        {
            Logger.DebugFormat("action in state {0}", _parachuteState);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.IsUseAction = true);
            switch(_parachuteState)
            {
                case ParachuteState.WaitGlide:
                    break;
                case ParachuteState.WaitParachute:
                    break;
            }
        }

        private bool CanGlide()
        {
            if(null == MyPlayerEntity)
            {
                return false;
            }
            if(!MyPlayerEntity.hasGamePlay)
            {
                return false;
            }
            var result = PlayerStateUtil.HasPlayerState(EPlayerGameState.CanJump, MyPlayerEntity.gamePlay);
            return result;
        }

        private bool CanParachute()
        {
            if(null == MyPlayerEntity)
            {
                return false;
            }
            if(MyPlayerEntity.hasStateInterface)
            {
                var result = MyPlayerEntity.stateInterface.State.GetActionState() == XmlConfig.ActionInConfig.Gliding;
                var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;
                result &= MyPlayerEntity.playerSkyMove.ParachuteTime > moveConfig.ParachuteTime;
 
//                Logger.DebugFormat("check parachute {0}, state is  {1}",
//                    result,
//                    MyPlayerEntity.stateInterface.State.GetActionState());
                return result;
            }
            else
            {
                Logger.Error("player entity has no state interface");
            }
            return false;
        }

        private void UpdateState()
        {
            if(!MyPlayerEntity.hasStateInterface)
            {
                return;
            }
            if(MyPlayerEntity.stateInterface.State.GetActionState() == XmlConfig.ActionInConfig.Parachuting)
            {
                _parachuteState = ParachuteState.None;
            }
            else if(CanParachute())
            {
                _parachuteState = ParachuteState.WaitParachute;
            }
            else if(MyPlayerEntity.stateInterface.State.GetActionState() == XmlConfig.ActionInConfig.Gliding)
            {
                _parachuteState = ParachuteState.Glide;
            }
            else if(CanGlide())
            {
                _parachuteState = ParachuteState.WaitGlide;
            }
        }
    }
}