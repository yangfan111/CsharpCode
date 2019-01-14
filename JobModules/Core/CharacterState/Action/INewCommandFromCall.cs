using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Utils.Appearance;

namespace Core.CharacterState.Action
{
    public interface INewCommandFromCall
    {
        /// <summary>
        /// 动画播放完成的回调
        /// </summary>
        /// <param name="commands"></param>
        void TryAnimationBasedCallBack(IAdaptiveContainer<IFsmInputCommand> commands);
        /// <summary>
        /// 打断动画,因此不执行动画播放完成的回调
        /// </summary>
        /// <param name="commands"></param>
        void AddInterruptInput(FsmInput input);

        void ServerUpdate();
        
        void CollectAnimationCallback(Action<short, float> addCallback);

        void ClearAnimationCallback();

        void HandleAnimationCallback(List<KeyValuePair<short, float>> commands);

    }
}
