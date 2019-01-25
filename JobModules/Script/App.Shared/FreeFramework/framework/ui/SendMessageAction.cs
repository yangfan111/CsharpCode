using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.util;
using System;
using Core.Free;
using Free.framework;
using Free.framework.ui;
using UnityEngine;
using Core.Utils;
using com.wd.free.debug;

namespace gameplay.gamerule.free.ui
{
    [System.Serializable]
    public abstract class SendMessageAction : AbstractGameAction
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SendMessageAction));

        private const long serialVersionUID = 1697675183988052641L;

        public const int SCOPE_PLYAER = 1;

        public const int SCOPE_TEAMATE = 2;

        public const int SCOPE_OBSERVER = 3;

        public const int SCOPE_ALL = 4;

        protected string scope;

        protected string player;

        [System.NonSerialized]
        public int playerId;

        [System.NonSerialized]
        protected static SimpleProto builder = FreePool.Allocate();

        public static IFreeMessageSender sender;

        public virtual string GetMessageDesc()
        {
            return this.GetType().Name;
        }

        public override void DoAction(IEventArgs args)
        {
            builder = FreePool.Allocate();

            BuildMessage(args);

            //if (builder.Key == 52)
            //{
            //          Debug.Log(builder.ToString());
            //}


            if (FreeLog.IsEnable())
            {
                Log(args);
            }

            if (sender != null)
            {
                sender.SendMessage(args, builder, FreeUtil.ReplaceInt(scope, args), FreeUtil.ReplaceVar(player, args));
            }
        }

        protected abstract void BuildMessage(IEventArgs args);

        private void Log(IEventArgs args)
        {
            string s = string.Empty;
            int realScope = FreeUtil.ReplaceInt(scope, args);
            switch (realScope)
            {
                case SCOPE_ALL:
                    {
                        s = "全部玩家";
                        break;
                    }

                case SCOPE_OBSERVER:
                    {
                        s = "观察者";
                        break;
                    }

                case SCOPE_PLYAER:
                    {
                        s = "玩家" + player;
                        break;
                    }

                case SCOPE_TEAMATE:
                    {
                        s = "玩家" + player + "的队友";
                        break;
                    }
            }
            FreeLog.Message(string.Format("{0} \n{1}\n范围:{2}", FreeMessageConstant.GetMessageDesc(builder.Key), builder, s), args);
        }

        public virtual void SetScope(int scope)
        {
            this.scope = scope.ToString();
        }

        public virtual string GetPlayer()
        {
            return player;
        }

        public virtual void SetPlayer(string player)
        {
            this.player = player;
        }
    }
}
