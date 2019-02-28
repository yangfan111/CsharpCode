using System;
using System.Collections.Generic;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Utils;

namespace Core.CharacterState.Action
{
    public class NewCommandImpl
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NewCommandImpl));
        
        private enum AnimationCallBackCommandType
        {
            Apply = 0,
            Interrupt
        }

        private struct AnimationCallBackCommand
        {
            public short CommandType;
            public int FsmType;
        }
        
        private class NewCommandContainer : AdaptiveContainerImpl<INewCommand, NewCommand>
        {
            private int _useCount;

            public NewCommandContainer(int initSize) : base(initSize)
            {
                _defaultGetItemCondition = DefaultGetItemCondition;
                Reset();
            }

            public void Reset()
            {
                _useCount = 0;
                for (int i = 0; i < this.Length; ++i)
                {
                    this[i].Reset();
                }
            }

            public void ResetItem(FsmInput type)
            {
                for (var i = 0; i < this.Length; ++i)
                {
                    if (this[i].Type == type)
                        this[i].Reset();
                }
            }

            private bool DefaultGetItemCondition(INewCommand newCommand)
            {
                return newCommand.Type == FsmInput.None;
            }

            protected override int GetAvailable(Func<INewCommand, bool> getItemCondition)
            {
                int ret = 0;
                int index = _useCount;
                while (index < this.Length)
                {
                    if (getItemCondition(this[index]))
                    {
                        ret = index;
                        _useCount = ret + 1;
                        return ret;
                    }

                    index++;
                }

                Logger.WarnFormat("resize NewCommandContainer list from {0} to {1}, useCount:{2}!!!", this.Length,
                    ret + 1, _useCount);
                ret = index;
                _useCount = ret + 1;
                Resize(ret + 1);
                var newCommand = new NewCommand();
                newCommand.Reset();
                this[ret] = newCommand;
                return ret;
            }
        }
        
        private interface INewCommand
        {
            FsmInput Type { get; set; }
            float AdditionalValue { get; set; }
            void Reset();
        }

        private class NewCommand : INewCommand
        {
            public NewCommand()
            {
                Reset();
            }

            public FsmInput Type { get; set; }
            public float AdditionalValue { get; set; }

            public void Reset()
            {
                Type = FsmInput.None;
                AdditionalValue = 0f;
            }
        }

        private class CallBackRegister
        {
            private readonly Dictionary<FsmInput, System.Action> _inputCallBack =
                new Dictionary<FsmInput, System.Action>(CommonIntEnumEqualityComparer<FsmInput>.Instance);

            private readonly Dictionary<FsmInput, System.Action> _callBackRemove =
                new Dictionary<FsmInput, System.Action>(CommonIntEnumEqualityComparer<FsmInput>.Instance);

            public void AddNewCallBack(FsmInput trigger, FsmInput removeCondition, System.Action callBack)
            {
                if (!_inputCallBack.ContainsKey(trigger))
                {
                    if (!_callBackRemove.ContainsKey(removeCondition))
                    {
                        _callBackRemove[removeCondition] = default(System.Action);
                    }

                    _callBackRemove[removeCondition] += () => _inputCallBack[trigger] = null;
                }

                _inputCallBack[trigger] = callBack;
            }

            public void TryInvokeCallBack(FsmInput type)
            {
                System.Action callBack;
                _inputCallBack.TryGetValue(type, out callBack);

                if (callBack != null)
                {
                    callBack.Invoke();
                    Logger.DebugFormat("Animation End Callback: {0}", type);
                }
            }

            public void TryRemoveCallBack(FsmInput type)
            {
                System.Action remove;
                _callBackRemove.TryGetValue(type, out remove);

                if (remove != null)
                    remove.Invoke();
            }
        }
        
        private static readonly int CommandsInitLen = 5;
        private NewCommandContainer commandsContainer = new NewCommandContainer(CommandsInitLen);
        private FsmOutputCache _directOutputs = new FsmOutputCache();
        private CallBackRegister _callBackRegister = new CallBackRegister();
        private List<FsmInput> _interruptInputs = new List<FsmInput>();
        private List<AnimationCallBackCommand> _animationCallBackCommand = new List<AnimationCallBackCommand>();
        
        protected void SetNewCommandFromFunctionCall(FsmInput type, float additionalValue = 0)
        {
            Logger.DebugFormat("Request Do Action : {0}   AdditionalValue  : {1}", type, additionalValue);
            var availableCommand = commandsContainer.GetAvailableItem();
            availableCommand.Type = type;
            availableCommand.AdditionalValue = additionalValue;
        }

        protected void SetNewCallbackFromFunctionCall(FsmInput trigger, FsmInput removeCondition, System.Action callBack)
        {
            if (callBack != null)
            {
                _callBackRegister.AddNewCallBack(trigger, removeCondition, callBack);
                Logger.DebugFormat("New callback from function call: {0}", trigger);
            }
        }

        protected void ApplyNewCommand(IAdaptiveContainer<IFsmInputCommand> commands, Action<FsmOutput> addOutput)
        {
            for (int i = 0; i < commandsContainer.Length; ++i)
            {
                var newCommand = commandsContainer[i];
                if (newCommand.Type != FsmInput.None)
                {
                    var item = commands.GetAvailableItem();
                    item.Type = newCommand.Type;
                    item.AdditioanlValue = newCommand.AdditionalValue;
                }
            }

            ClearAction(commands);
            commandsContainer.Reset();
            _directOutputs.Apply(addOutput);
        }

        public void TryAnimationBasedCallBack(IAdaptiveContainer<IFsmInputCommand> commands)
        {
            InterruptAnimationCallBack(commands);
            for (int i = 0; i < commands.Length; ++i)
            {
                var cmd = commands[i];
                if (cmd.Type != FsmInput.None)
                {
                    _callBackRegister.TryInvokeCallBack(cmd.Type);
                    _callBackRegister.TryRemoveCallBack(cmd.Type);
                    _animationCallBackCommand.Add(new AnimationCallBackCommand
                    {
                        CommandType = (int) AnimationCallBackCommandType.Apply,
                        FsmType = (int) cmd.Type
                    });
                }
            }
        }

        // 添加需要打断的 input
        public void AddInterruptInput(FsmInput input)
        {
            _interruptInputs.Add(input);
        }

        public virtual void ServerUpdate()
        {
            commandsContainer.Reset();
        }

        public void CollectAnimationCallback(Action<short, float> addCallback)
        {
            foreach (var backCommand in _animationCallBackCommand)
            {
                addCallback.Invoke(backCommand.CommandType, backCommand.FsmType);
            }
        }

        public void ClearAnimationCallback()
        {
            _animationCallBackCommand.Clear();
        }

        public void HandleAnimationCallback(List<KeyValuePair<short, float>> commands)
        {
            foreach (var keyValuePair in commands)
            {
                if (Mathf.Abs((keyValuePair.Value - (float) ((short) keyValuePair.Value))) > 0.1f)
                {
                    Logger.ErrorFormat("HandleAnimationCallback, float:{0} convert to int:{1} error",
                        keyValuePair.Value, (short) keyValuePair.Value);
                }

                if (keyValuePair.Key == (short) AnimationCallBackCommandType.Apply)
                {
                    var type = (FsmInput) ((short) keyValuePair.Value);
                    _callBackRegister.TryInvokeCallBack(type);
                    _callBackRegister.TryRemoveCallBack(type);
                }
                else if (keyValuePair.Key == (short) AnimationCallBackCommandType.Interrupt)
                {
                    var type = (FsmInput) ((short) keyValuePair.Value);
                    _callBackRegister.TryRemoveCallBack(type);
                }
            }
        }

        // 拦截需要销毁的input(打断不触发)
        private void InterruptAnimationCallBack(IAdaptiveContainer<IFsmInputCommand> commands)
        {
            if (_interruptInputs.Count <= 0) return;
            for (int i = 0; i < _interruptInputs.Count; ++i)
            {
                var input = _interruptInputs[i];
                for (int j = 0; j < commands.Length; ++j)
                {
                    var cmd = commands[j];
                    if (cmd.Type == input)
                    {
                        _callBackRegister.TryRemoveCallBack(cmd.Type);
                        _animationCallBackCommand.Add(new AnimationCallBackCommand
                        {
                            CommandType = (int) AnimationCallBackCommandType.Interrupt,
                            FsmType = (int) cmd.Type
                        });
                    }
                }
            }

            _interruptInputs.Clear();
        }

        /// <summary>
        /// 拦截一些action(不执行)
        /// </summary>
        /// <param name="commands"></param>
        private void ClearAction(IAdaptiveContainer<IFsmInputCommand> commands)
        {
            ClearActionByCmd(commands, FsmInput.Climb, FsmInput.Jump);
        }

        private void ClearActionByCmd(IAdaptiveContainer<IFsmInputCommand> commands, FsmInput cmd, FsmInput clearAction)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                if (commands[i].Type == cmd)
                {
                    ClearActionByCmdHelper(commands, clearAction);
                    return;
                }
            }
        }

        private void ClearActionByCmdHelper(IAdaptiveContainer<IFsmInputCommand> commands, FsmInput clearAction)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                if (commands[i].Type == clearAction)
                {
                    commands[i].Type = FsmInput.None;
                    commands[i].Handled = false;
                    return;
                }
            }
        }
        
        private static readonly FsmInput[] InterruptTypes = 
        {
            FsmInput.BuriedBomb,
            FsmInput.Reload,
            FsmInput.SpecialReload,
            FsmInput.Props
        };

        public void InterruptInputs()
        {
            for (var i = 0; i < InterruptTypes.Length; i++)
            {
                commandsContainer.ResetItem(InterruptTypes[i]);
            }
        }
    }
}