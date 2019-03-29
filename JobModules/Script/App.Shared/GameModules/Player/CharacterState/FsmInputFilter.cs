using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using Utils.Appearance;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterState
{
    // 处理以下命令
    // PeekLeft -- 向左侧身
    // PeekRight -- 向右侧身
    // NoPeek -- 不侧身
    // Walk -- 静走
    // Run -- 奔跑
    // Sprint -- 冲刺

    interface IFsmInputFilter
    {
        bool Active { get; }
        void SetCurrentState(FilterState state);
        void Filter(IAdaptiveContainer<IFsmInputCommand> commands);
    }

    public struct FilterState
    {
        public PostureInConfig Posture;

        public bool IsEqual(FilterState rhs)
        {
            return Posture == rhs.Posture;
        }
    }

    abstract class AbstractStateFilter : IFsmInputFilter
    {
        public bool Active { get; private set; }

        public abstract void Filter(IAdaptiveContainer<IFsmInputCommand> commands);

        public void SetCurrentState(FilterState state)
        {
            Active = KeyState.IsEqual(state);
        }

        protected abstract FilterState KeyState { get; }
    }

    class DiveStateFilter:AbstractStateFilter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DiveStateFilter));
        private static readonly FilterState _keyState = new FilterState { Posture = PostureInConfig.Dive };

        private static Dictionary<FsmInput, FsmInput> SimpleMap = new Dictionary<FsmInput, FsmInput>(CommonIntEnumEqualityComparer<FsmInput>.Instance)
        {
            { FsmInput.Sprint,              FsmInput.None },
            { FsmInput.Run,                FsmInput.None },
            { FsmInput.Walk,                FsmInput.None },
            { FsmInput.Idle,                FsmInput.None },
        };

        /// <summary>
        /// 如果满足1,把2设为none
        /// </summary>
        private static Dictionary<FsmInput, FsmInput> TransferMap = new Dictionary<FsmInput, FsmInput>(CommonIntEnumEqualityComparer<FsmInput>.Instance)
        {
            {FsmInput.DiveIdle, FsmInput.Idle }
        }; 


        public override void Filter(IAdaptiveContainer<IFsmInputCommand> commands)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                Filter(commands[i]);
            }

            for (int i = 0; i < commands.Length; ++i)
            {
                if (TransferMap.ContainsKey(commands[i].Type))
                {
                    commands[i].Type = TransferMap[commands[i].Type];
                }
            }
        }

        private void Filter(IFsmInputCommand command)
        {
            FsmInput mappedType;
            if (SimpleMap.TryGetValue(command.Type, out mappedType))
            {
                command.Type = mappedType;
            }
        }

        protected override FilterState KeyState
        {
            get { return _keyState; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    class ProneStateFilter : AbstractStateFilter
    {
        private static readonly FilterState _key = new FilterState { Posture = PostureInConfig.Prone };

        protected override FilterState KeyState { get { return _key; } }

        public override void Filter(IAdaptiveContainer<IFsmInputCommand> commands)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                Filter(commands[i]);
            }
        }

        private static Dictionary<FsmInput, FsmInput> SimpleMap = new Dictionary<FsmInput, FsmInput>(CommonIntEnumEqualityComparer<FsmInput>.Instance)
        {
            { FsmInput.Sprint,              FsmInput.Run },
            { FsmInput.Walk,                FsmInput.Run },
            { FsmInput.PeekLeft,            FsmInput.NoPeek },
            { FsmInput.PeekRight,           FsmInput.NoPeek }
        };

        private void Filter(IFsmInputCommand command)
        {
            FsmInput mappedType;
            if (SimpleMap.TryGetValue(command.Type, out mappedType))
            {
                command.Type = mappedType;
            }
        }
    }
}
