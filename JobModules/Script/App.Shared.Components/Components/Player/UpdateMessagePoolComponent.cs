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
        [DontInitilize] public IUpdateMessagePool UpdateMessagePool {
            get { return _updateMessagePool;}
        }
        [DontInitilize] public int LastestExecuteUserCmdSeq;
        private IUpdateMessagePool _updateMessagePool;

        public void Reset()
        {
            LastestExecuteUserCmdSeq = -1;
            if (_updateMessagePool == null)
            {
                _updateMessagePool = new UpdateMessagePool();
                
            }
            else
            {
                _updateMessagePool.Dispose();
            }
        }
    }
}