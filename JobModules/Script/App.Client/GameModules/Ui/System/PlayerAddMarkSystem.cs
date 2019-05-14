using App.Shared.Util;
using Assets.App.Client.GameModules.Ui;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.System
{
    public class PlayerAddMarkSystem : AbstractUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAddMarkSystem));

        private Contexts _contexts;

        public PlayerAddMarkSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return true;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            if (!cmd.IsAddMark)
                return;
            var mapdata = _contexts.ui.map;
            if (mapdata == null)
                return;
            Vector2 markPos = Vector2.zero;
            if (mapdata.CurPlayer != null) markPos = mapdata.CurPlayer.Pos.ShiftedUIVector2();

            PlayerAddMarkUtility.SendMarkMessage(_contexts, markPos);
        }
    }
}