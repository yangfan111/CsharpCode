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
        [DontInitilize] public IUpdateMessagePool UpdateMessagePool;
        [DontInitilize] public int LastestExecuteUserCmdSeq;
        public void Reset()
        {
            LastestExecuteUserCmdSeq = -1;
            if (UpdateMessagePool == null)
            {
                UpdateMessagePool = new UpdateMessagePool();
                
            }
        }
    }
}