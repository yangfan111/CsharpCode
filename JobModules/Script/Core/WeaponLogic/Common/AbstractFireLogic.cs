using Core.WeaponLogic.Attachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeaponConfigNs;

namespace Core.WeaponLogic.Common
{
    public abstract class AbstractFireLogic<TFireLogicConfig, T3> : AbstractAttachableWeaponLogic<TFireLogicConfig, T3> where TFireLogicConfig : ICopyableConfig<TFireLogicConfig>, new()
    {
        public AbstractFireLogic(TFireLogicConfig config):base(config)
        {

        }
        public virtual float GetFocusSpeed()
        {
            return 1;
        }

        public virtual float GetBreathFactor()
        {
            return 1;
        }

        public virtual float GetFocusFov()
        {
            return 0;
        }

        public virtual bool IsFovModified()
        {
            return false;
        }

        public virtual float GetReloadSpeed()
        {
            return 1;
        }
    }
}
