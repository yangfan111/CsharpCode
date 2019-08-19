using Core.Components;
using Core.Utils;
using Utils.CharacterState;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// 最终改变WeaponRuntimeDataComponent.LastSpreadX/LastSpreadY
    /// </summary>
    public class RifleSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        
        {
            // var appearance = controller.RelatedAppearence;
            // var weaponObject = appearance.IsFirstPerson
            //                 ? appearance.GetWeaponP1InHand()
            //                 : appearance.GetWeaponP3InHand();
            // DebugUtil.MyLog(weaponObject.transform.position.ToString("f4"));
            RifleSpreadLogicConfig config = attackProxy.WeaponConfigAssy.S_RifleSpreadLogicCfg;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, attackProxy);
            FireSpreadFormula.ApplyRifleFinalSpread(spreadScaleFactor ,config.SpreadScale,attackProxy.RuntimeComponent);
        }

    }
}
