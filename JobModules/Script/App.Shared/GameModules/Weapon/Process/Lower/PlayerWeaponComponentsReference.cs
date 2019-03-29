using App.Shared.Components.Player;

namespace App.Shared.GameModules
{

    public class PlayerWeaponComponentsReference : PlayerComponentsReference
    {
        public PlayerWeaponComponentsReference(PlayerEntity in_entity) : base(in_entity)
        {
        }

        public PlayerWeaponBagSetComponent BagSet
        {
            get { return entity.playerWeaponBagSet; }
        }
        public PlayerWeaponUpdateComponent WeaponUpdate
        {
            get { return entity.playerWeaponUpdate; }
        }

        public PlayerWeaponAuxiliaryComponent WeaponAux
        {
            get
            {
                if (entity.hasPlayerWeaponAuxiliary)
                    return entity.playerWeaponAuxiliary;
                entity.AddPlayerWeaponAuxiliary();
                return entity.playerWeaponAuxiliary;
            }
        }

        public PlayerWeaponCustomizeComponent Customize
        {
            get { return entity.playerWeaponCustomize; }
        }
        public GrenadeCacheDataComponent GrenadeCache
        {
            get { return entity.grenadeCacheData;}
        }


    }

}
