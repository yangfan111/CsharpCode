using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using UnityEngine;
using Utils.CharacterState;

namespace Core.Fsm
{
    public enum FsmOutputType
    {
        Bool,
        Float,
        Int,

        LayerWeight,
        FirstPersonHeight,
        FirstPersonForwardOffset,

        FirstPersonSight,

        Peek,

        CharacterControllerHeight,
        CharacterControllerJumpHeight,
        CharacterControllerRadius,
        InterruptAction,
        ChangeDiveSensitivity,
        
        RebindAnimator,
        End
    }

    public class FsmOutput
    {
        public const float PeekLeftDegree = -1f;
        public const float PeekRightDegree = 1f;
        public const float NoPeekDegree = 0;

        public bool Valid;
        public string Target;
        public int TargetHash;
        public FsmOutputType Type;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public CharacterView View;
        public bool UpdateImmediate;

        public static FsmOutput Cache = new FsmOutput();

        public void CopyFrom(FsmOutput source)
        {
            Valid = source.Valid;
            Target = source.Target;
            TargetHash = source.TargetHash;
            Type = source.Type;
            BoolValue = source.BoolValue;
            IntValue = source.IntValue;
            FloatValue = source.FloatValue;
            View = source.View;
            UpdateImmediate = source.UpdateImmediate;
        }

        #region SetValue

        // BOOL变量作为状态迁移触发用,分为两种情况,一是设置完成后必须立即清除,二是设置完成后可以等待某个条件来清除
        // 例如：在合理帧率范围内，Fire/Injury需要设置后清除，否则可能会出现多次Fire/Injury；而Reload可以在动画播放后再清除
        public void SetValue(int hash, string name, bool value, CharacterView view, bool updateImmediate)
        {
            Type = FsmOutputType.Bool;
            Target = name;
            TargetHash = hash;
            BoolValue = value;
            View = view;
            UpdateImmediate = updateImmediate;
        }

        public void SetValue(int hash, string name, int value, CharacterView view)
        {
            Type = FsmOutputType.Int;
            Target = name;
            TargetHash = hash;
            IntValue = value;
            View = view;
        }

        public void SetValue(int hash, string name, float value, CharacterView view)
        {
            Type = FsmOutputType.Float;
            Target = name;
            TargetHash = hash;
            FloatValue = value;
            View = view;
        }

        #endregion

        public void SetLayerWeight(int index, float weight, CharacterView view)
        {
            Type = FsmOutputType.LayerWeight;
            IntValue = index;
            FloatValue = weight;
            View = view;
        }

        public void SetValue(FsmOutputType type, float value)
        {
            Type = type;
            FloatValue = value;
        }
    }

    public class FsmOutputCache
    {
        private List<FsmOutput> _cache = new List<FsmOutput>();
        private int _availableOutputs;

        public void CacheFsmOutput(int hash, string name, bool value, CharacterView view)
        {
            var output = GetAvailableOutput();
            output.SetValue(hash, name, value, view, false);
        }

        public void CacheFsmOutput(int hash, string name, int value, CharacterView view)
        {
            var output = GetAvailableOutput();
            output.SetValue(hash, name, value, view);
        }

        public void CacheFsmOutput(int hash, string name, float value, CharacterView view)
        {
            var output = GetAvailableOutput();
            output.SetValue(hash, name, value, view);
        }

        public void CacheFsmOutput(int index, float weight, CharacterView view)
        {
            var output = GetAvailableOutput();
            output.SetLayerWeight(index, weight, view);
        }

        public void CacheFsmOutput(FsmOutputType type, float value)
        {
            var output = GetAvailableOutput();
            output.SetValue(type, value);
        }

        public void Apply(Action<FsmOutput> exporter)
        {
            for (int i = 0; i < _availableOutputs; i++)
            {
                exporter(_cache[i]);
            }
            _availableOutputs = 0;
        }

        private FsmOutput GetAvailableOutput()
        {
            if (_availableOutputs >= _cache.Count)
            {
                _cache.Add(new FsmOutput());
            }

            return _cache[_availableOutputs++];
        }
    }
}
