using System.Collections.Generic;
using App.Client.GameModules.GamePlay.Free.UI;
using App.Protobuf;
using Assets.Sources.Free.Effect;
using Free.framework;

namespace Assets.Sources.Free.UI
{
    public class FreeUIUtil
    {
        private IList<IFreeComponent> _components;
        private IList<IShowStyle> _showStyles;
        private IList<IFreeEffect> _effects;
		
        private static FreeUIUtil _instance;
		
        public  FreeUIUtil()
        {
            _components = new List<IFreeComponent> ();
            _showStyles = new List<IShowStyle> ();
            _effects = new List<IFreeEffect> ();

            _components.Add(new FreeImageComponent());
            _components.Add(new FreeTextComponent());
            _components.Add(new FreeNumberComponent());
            _components.Add(new FreeListComponent());
            _components.Add(new FreeRaderComponent());
            _components.Add(new FreeExpComponent());
            _components.Add(new FreeSmallMapComponent());
            _components.Add(new FreePrefabComponent());
//            this._components.Add(new FreeRotationImageComponent());
//
            this._showStyles.Add(new ShowSimpleStyle());
//
            this._effects.Add(new FreeSingleEffect());
//            this._effects.Add(new FreeTwoEffect());
            this._effects.Add(new FreeParticleEffect());
        }

        public static FreeUIUtil GetInstance(){
            if (_instance == null) _instance = new FreeUIUtil();
            return _instance;
        }

        public IShowStyle GetShowStyle( SimpleProto sp)
        {
            for (var index = 0; index < _showStyles.Count; index++)
            {
                var style = _showStyles[index];
                var ns = style.Parse(sp);
                if (ns != null)
                    return ns;
            }
            return null;
        }
		
        public IFreeEffect GetEffect(int type)
        {
            for (var index = 0; index < _effects.Count; index++)
            {
                var effect = _effects[index];
                if (effect.Type == type)
                    return effect.Clone();
            }

            return null;
        }
		
        public IFreeComponent GetComponent(int type)
        {
            for (var index = 0; index < _components.Count; index++)
            {
                var po = _components[index];
                if (po.Type == type)
                    return po.Clone();
            }

            return null;
        }
		
    }
    
}
