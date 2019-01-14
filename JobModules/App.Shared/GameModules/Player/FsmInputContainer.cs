using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.CharacterState;
using Core.Fsm;
using Core.Utils;
using Utils.Appearance;
using Utils.Utils;

namespace App.Shared.GameModules.Player
{
    public class FsmInputContainer: AdaptiveContainerImpl<IFsmInputCommand, FsmInputCommand>
    {
        
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmInputContainer));
        
        public FsmInputContainer(int initSize) : base(initSize)
        {
            _defaultGetItemCondition = DefaultGetItemCondition;
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < this.Length; ++i)
            {
                this[i].Reset();
            }
        }

        private bool DefaultGetItemCondition(IFsmInputCommand fsmInputCommand)
        {
            return fsmInputCommand.Type == FsmInput.None;
        }

        protected override int GetAvailable(Func<IFsmInputCommand, bool> getItemCondition)
        {
            int ret = 0;

            for (int i = 0; i < this.Length; ++i)
            {
                var inputCommand = this[i];
                if (getItemCondition(inputCommand))
                {
                    return ret;
                }
                ret++;
            }

            Logger.WarnFormat("resize FsmInputContainer list from {0} to {1}!!!", this.Length, ret + 1);
            
            Resize(ret + 1);
            var newCommand = new FsmInputCommand();
            newCommand.Reset();
            this[ret] = newCommand;

            return ret;
        }
    }
}
