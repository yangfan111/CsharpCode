using System;
using Sharpen;

namespace com.wd.free.exception
{
    public enum EPriority
    {
        Ignore = 100,
        Influential = 200,
        Important = 300,
        Serious = 400,
    }

    public enum EFreeException
    {
        Action,
        Effect,
        Ignore = (int)EPriority.Ignore, // 特效，动作等
        Config,
        Influential = (int)EPriority.Influential, // 配置
        Logic,
        Important = (int)EPriority.Important, // 逻辑
        Hp,
        Damage,
        Dead,
        Trigger,
        Serious = (int)EPriority.Serious, // 血量
    }

    [System.Serializable]
	public class GameConfigExpception : Exception
	{
		private const long serialVersionUID = 4509393008035571056L;

		public GameConfigExpception()
			: base()
		{
		}

		public GameConfigExpception(string msg)
			: base(msg)
		{
		}

		public GameConfigExpception(Exception t)
			: base(t.Message)
		{
		}
	}
}
