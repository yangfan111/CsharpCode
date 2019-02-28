using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Core.GameModule.System;
using Entitas;

namespace App.Client.GameModules.Ui
{
    public class UiPlayerDataInitSystem : ReactiveEntityInitSystem<PlayerEntity>
    {

        public UiPlayerDataInitSystem(Contexts contexts) : base(contexts.player)
        {

        }
     
        //目前没有观战  先关闭
        public override void SingleExecute(PlayerEntity entity)
        {
//            if(weaponBagStateAdapter != null) weaponBagStateAdapter.Player = entity;
//            if(weaponBagTipStateAdapter != null) weaponBagTipStateAdapter.Player = entity;
//            if (stateUiAdapter != null) stateUiAdapter.PlayerEntity = entity;
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.isFlagSelf;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.PlayerInfo.Added());
        }
    }
}
