using App.Shared;
using UserInputManager.Lib;
using App.Client.CastObjectUtil;
using App.Shared.Components.Player;
using Core.Enums;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using I2.Loc;

namespace App.Client.GameModules.Ui.Logic
{
    public class PlayerCastLogic : AbstractCastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerCastLogic));
        private PlayerContext _playerContext;
        private IUserCmdGenerator _userCmdGenerator;
        private int _playerId;

        public PlayerCastLogic(
            PlayerContext playerContext,
            IUserCmdGenerator cmdGenerator,
            float maxDistance) : base(playerContext, maxDistance)
        {
            _playerContext = playerContext;
            _userCmdGenerator = cmdGenerator;
        }

        public override void OnAction()
        {
            _userCmdGenerator.SetUserCmd((cmd) => cmd.IsUseAction = true);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.UseEntityId = _playerId);
            _userCmdGenerator.SetUserCmd((cmd) => cmd.UseType = (int)EUseActionType.Player);
        }

        protected override void DoSetData(PointerData data)
        {
            _playerId = PlayerCastData.EntityId(data.IdList);
            var playerEntity = _playerContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(_playerId, (short)EEntityType.Player));
            if (null != playerEntity && playerEntity.hasGamePlay)
            {
                var player = _playerContext.flagSelfEntity;

                if (player.gamePlay.IsLifeState(EPlayerLifeState.Alive) && !player.gamePlay.IsSave                      //自己是活着
                    && playerEntity.playerInfo.TeamId == player.playerInfo.TeamId && !playerEntity.gamePlay.IsBeSave    //是队友
                    && playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dying)                                        //队友是受伤状态
                    && Vector3.Distance(player.position.Value, playerEntity.position.Value) <= SharedConfig.MaxSaveDistance  //距离
                    )
                {
                    Tip = string.Format(ScriptLocalization.client_actiontip.saveplayer, playerEntity.playerInfo.PlayerName);
                }
            }
        }
    }
}
