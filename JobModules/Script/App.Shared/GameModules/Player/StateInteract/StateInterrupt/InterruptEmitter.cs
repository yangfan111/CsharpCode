using System.Collections.Generic;
using Core;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class InterruptEmitter
    {
        private EPlayerState playerState;

//            private       Action<EInterruptType, Func<HashSet<EPlayerState>, bool>> interruptAndRecover;
//            private       Action<EInterruptType>                                    interruptSimple;
        private InterruptEventHandler handler;
        private EInterruptCmdType      cmdType;

        public InterruptEmitter(EPlayerState           playerState, EInterruptCmdType cmdType)
        {
            this.playerState      = playerState;
            this.cmdType          = cmdType;
            ////this.interruptEmitter = interruptEmitter;
        }
        public void SetHanlder(InterruptEventHandler emitter)
        {
            this.handler = emitter;

        }

        public bool Trigger(HashSet<EPlayerState> states)
        {
            if (CanTrigger(states))
            {
                if (cmdType == EInterruptCmdType.InterruptSimple)
                    handler.Interrupt();
                else
                    handler.Interrupt(CanRecover);
                return true;
            }
            return false; 
        }

        private bool CanTrigger(HashSet<EPlayerState> states)
        {
            return  states.Contains(playerState);
        }

        private bool CanRecover(HashSet<EPlayerState> states)
        {
            return !states.Contains(playerState);
        }
    }

}