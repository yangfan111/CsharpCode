using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.para.exp;
using App.Shared.FreeFramework.framework.ai.move;

namespace com.wd.free.ai
{
    [Serializable]
    public class OrderAiAction : AbstractGameAction
    {
        public int repeat;
        public List<IGameAction> actions;
        // 每帧都做的动作，如收集数据
        public IGameAction frame;

        public IParaCondition condition;

        private int index;
        private bool initialed;

        private int currentCount;

        public override void DoAction(IEventArgs args)
        {
            Initial();

            if (index >= 0 && index < actions.Count)
            {
                IGameAction action = actions[index];
                args.FreeContext.AiSuccess = false;

                action.Act(args);

                if(frame != null)
                {
                    frame.Act(args);
                }

                if (args.FreeContext.AiSuccess)
                {
                    index++;
                }

                args.FreeContext.AiSuccess = false;

                if (index == actions.Count)
                {
                    currentCount++;

                    if (currentCount >= repeat && repeat > 0)
                    {
                        args.FreeContext.AiSuccess = true;
                    }
                    else
                    {
                        index = 0;
                    }
                }

                if (!args.FreeContext.AiSuccess && condition != null && condition.Meet(args))
                {
                    args.FreeContext.AiSuccess = true;
                }
            }
        }

        public override void Reset(IEventArgs args)
        {
            index = 0;
            currentCount = 0;
            for(int i = 0; i < actions.Count; i++)
            {
                actions[i].Reset(args);
            }
        }

        private void Initial()
        {
            if (!initialed)
            {
                initialed = true;
                index = 0;

                for (int i = 0; i < actions.Count; i++)
                {
                    if (actions[i] is WaitTimeAiAction)
                    {
                        continue;
                    }
                    if (actions[i] is OneTimeAiAction)
                    {
                        continue;
                    }
                    if (actions[i] is OrderAiAction)
                    {
                        continue;
                    }
                    if (actions[i] is MoveToAiAction)
                    {
                        continue;
                    }
                    if (actions[i] is FaceToAiAction)
                    {
                        continue;
                    }

                    OneTimeAiAction oneTime = new OneTimeAiAction();
                    oneTime.action = actions[i];
                    actions[i] = oneTime;
                }
            }

        }
    }
}
