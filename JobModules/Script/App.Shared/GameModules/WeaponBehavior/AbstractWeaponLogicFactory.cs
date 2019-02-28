using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IWeaponLogicComponentsFactory" />
    /// </summary>
    public interface IWeaponLogicComponentsFactory
    {
        IWeaponLogic CreateWeaponLogic(WeaponResConfigItem newCfg,
            WeaponConfig config,
            IWeaponSoundLogic soundLogic,
            IWeaponEffectLogic effectLogic);

        IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config);

        IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config);

        ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config);

        IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config);

        IThrowingFactory CreateThrowingFactory(WeaponResConfigItem newWeaponConfig, ThrowingConfig config);

        IFireActionLogic CreateFireActionLogic(WeaponResConfigItem config);

        IFireCheck CreateFireCheckLogic(FireModeLogicConfig config);

        IFireBulletCounter CreateFireBulletCounterLogic(FireCounterConfig config);

        IFireEffectFactory CreateFireEffectFactory(BulletConfig config);

        IAfterFire CreateAutoFireLogic(FireModeLogicConfig config);

        IFireCheck CreateSpecialReloadCheckLogic(CommonFireConfig config);

        IBulletFire CreateBulletFireLogic(BulletConfig config);

        IFireTriggger CreateFireCommandLogic(DefaultWeaponAbstractFireFireLogicConfig config);

        IBulletFire CreateShowFireInMap(DefaultWeaponAbstractFireFireLogicConfig config);
    }
}
