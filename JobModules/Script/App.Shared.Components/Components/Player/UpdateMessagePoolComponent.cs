using System;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    [Serializable]
    public class UpdateMessagePoolComponent:IComponent,IResetableComponent
    {
        private ServerUpdateMessagePool value;

        public ServerUpdateMessagePool Value
        {
            get
            {
                if(value == null)
                    value  =new ServerUpdateMessagePool();
                return value;
            }
        }
        
        [DontInitilize] public int LastestExecuteUserCmdSeq;

        public void Reset()
        {
            LastestExecuteUserCmdSeq = -1;
            if (value == null)
            {
                value = new ServerUpdateMessagePool();
                
            }
            else
            {
                value.Dispose();
            }
        }
    }
}