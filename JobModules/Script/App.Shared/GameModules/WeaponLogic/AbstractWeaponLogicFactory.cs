using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IWeaponLogicComponentsFactory" />
    /// </summary>
    public interface IWeaponLogicComponentsFactory
    {
        IWeaponLogic CreateWeaponLogic(NewWeaponConfigItem newCfg,
            WeaponConfig config,
            IWeaponSoundLogic soundLogic,
            IWeaponEffectLogic effectLogic);

        IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config);

        IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config);

        ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config);

        IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config);

        IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config);

        IFireActionLogic CreateFireActionLogic(NewWeaponConfigItem config);

        IFireCheck CreateFireCheckLogic(FireModeLogicConfig config);

        IFireBulletCounter CreateFireBulletCounterLogic(FireCounterConfig config);

        IFireEffectFactory CreateFireEffectFactory(BulletConfig config);

        IAfterFire CreateAutoFireLogic(FireModeLogicConfig config);

        IFireCheck CreateSpecialReloadCheckLogic(CommonFireConfig config);

        IBulletFire CreateBulletFireLogic(BulletConfig config);

        IFireTriggger CreateFireCommandLogic(FireLogicConfig config);

        IBulletFire CreateShowFireInMap(FireLogicConfig config);
    }
}
