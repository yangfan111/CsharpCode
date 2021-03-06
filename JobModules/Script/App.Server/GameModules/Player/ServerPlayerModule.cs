﻿using App.Server.GameModules.GamePlay.player;
using App.Server.GameModules.Player;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.ResourceLoad;
using Core.GameModule.Module;

namespace App.Server
{
    public class ServerPlayerModule : GameModule
    {
        public ServerPlayerModule(Contexts contexts)
        {
            AddSystem(new ServerRemoteEventInitSystem(contexts));
            AddSystem(new PlayerEntityInitSystem(contexts));
            AddSystem(new ServerPlayerCameraInitSystem(contexts.player));

            AddSystem(new PlayerChangeRoleSystem(contexts));
            AddSystem(new PlayerResourceLoadSystem(contexts));

            AddSystem(new PlayerDebugDrawSystem(contexts));
            AddSystem(new ServerPlayerWeaponInitSystem(contexts.player, contexts.session.commonSession));
            AddSystem(new PlayerEquipPickAndDropSystem());
            AddSystem(new ServerBulletInfoCollectSystem(contexts.bullet,contexts.player));
        }
    }
}
