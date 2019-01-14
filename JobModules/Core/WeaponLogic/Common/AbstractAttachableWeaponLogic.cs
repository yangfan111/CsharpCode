
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.Common
{
    public abstract class AbstractAttachableWeaponLogic<T1,  T3> where T1 : ICopyableConfig<T1>, new()
    {
        protected T1 _baseConfig;
        protected T1 _config;

        public AbstractAttachableWeaponLogic(T1 config)
        {
            _baseConfig = config; 
            _config = new T1();
            config.CopyTo(_config);
        }

        public void ApplyModifier( T3 arg)
        {
           
            Apply(_baseConfig, _config, arg);
        }
        public abstract void Apply(T1 baseConfig, T1 output, T3 arg);

    }
}
