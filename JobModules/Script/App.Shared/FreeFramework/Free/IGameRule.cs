using App.Shared.GameModules.Attack;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Free.framework;
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

        void HandleWeaponState(Contexts room, PlayerEntity player, int weaponId);

        void HandleWeaponFire(Contexts room, PlayerEntity player, WeaponResConfigItem info);
    }
}
