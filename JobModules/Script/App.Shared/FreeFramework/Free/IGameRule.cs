using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Core.Prediction.UserPrediction.Cmd;
using Free.framework;
using App.Shared.GameModules.Bullet;
using Core.Free;
using Core.WeaponLogic;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay
{
    public interface IGameRule : IFreeRule
    {
        void GameStart(Contexts room);

        void Update(Contexts room, int interval);

        void GameEnd(Contexts room);

        int GameStage { get; }

        int EnterStatus { get; }

        Contexts Contexts { get; }

        void PlayerPressCmd(Contexts room, PlayerEntity player, IUserCmd cmd);

        void PlayerEnter(Contexts room, PlayerEntity player);

        void PlayerLeave(Contexts room, PlayerEntity player);

        float HandleDamage(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage);

        void KillPlayer(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage);

        void HandleFreeEvent(Contexts room, PlayerEntity player, SimpleProto message);

        //TODO 修改
       // void HandleWeaponState(Contexts room, PlayerEntity player, IPlayerWeaponState state);
        void HandleWeaponState(Contexts room, PlayerEntity player);

        void HandleWeaponFire(Contexts room, PlayerEntity player, WeaponResConfigItem info);
    }
}
