using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Utils;
using Core.Appearance;
using Core.Configuration;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.Appearance
{
    class SightShift : IFirstPersonSight
    {
        private float _horizontalSpeed;
        private float _horizontalLimit;

        private float _verticalSpeed;
        private float _verticalLimitMax;
        private float _verticalLimitMin;
        private float _verticalPeriodTimeMax;
        private float _verticalPeriodTimeMin;

        private RandomnessSimulator _generator = new RandomnessSimulator();
        private IPossibilityResult _horizontalPossibility;

        #region Sync Properties

        private readonly IPredictedBreathState _state;

        private float HorizontalShift { get { return _state.SightHorizontalShift; } set { _state.SightHorizontalShift = value; } }
        private float VerticalShift { get { return _state.SightVerticalShift; } set { _state.SightVerticalShift = value; } }
        private float VerticalShiftRange { get { return _state.SightVerticalShiftRange; } set { _state.SightVerticalShiftRange = value; } }
        private int HorizontalShiftDirection { get { return _state.SightHorizontalShiftDirection; } set { _state.SightHorizontalShiftDirection = value; } }
        private int VerticalShiftDirection { get { return _state.SightVerticalShiftDirection; } set { _state.SightVerticalShiftDirection = value; } }
        private int RemainVerticalPeriodTime { get { return _state.SightRemainVerticalPeriodTime; } set { _state.SightRemainVerticalPeriodTime = value; } }
        private int RandomSeed { get { return _state.RandomSeed; } set { _state.RandomSeed = value; } }

        #endregion

        public float Buff { get { return _holdBreath ? 0.1f : (_prone ? 0.6f : _attachmentBuff); } }
        private bool _holdBreath;
        private bool _prone;
        private float _attachmentBuff = 1;

        public SightShift(IPredictedBreathState state)
        {
            _state = state;

            _horizontalSpeed = SingletonManager.Get<CharacterStateConfigManager>().SightShiftHorizontalSpeed;
            _horizontalLimit = SingletonManager.Get<CharacterStateConfigManager>().SightShiftHorizontalLimit;
            _verticalSpeed = SingletonManager.Get<CharacterStateConfigManager>().SightShiftVerticalSpeed;
            _verticalLimitMax = SingletonManager.Get<CharacterStateConfigManager>().SightShiftVerticalLimitMax;
            _verticalLimitMin = SingletonManager.Get<CharacterStateConfigManager>().SightShiftVerticalLimitMin;
            _verticalPeriodTimeMax = SingletonManager.Get<CharacterStateConfigManager>().SightShiftVerticalPeriodTimeMax;
            _verticalPeriodTimeMin = SingletonManager.Get<CharacterStateConfigManager>().SightShiftVerticalPeriodTimeMin;

            _horizontalPossibility = _generator.GetSimplePossibilityExpression(0.25f);

            Clear();
            HorizontalShiftDirection = 1;
        }

        // interval in ms
        public void Update(int interval)
        {
            _generator.Reset(RandomSeed);

            var randomNumber = _generator.Random();
            var intervalInSecond = interval * 0.001f;

            // Vertical
            // pause period
            if (RemainVerticalPeriodTime > 0)
            {
                // decrease pause time
                RemainVerticalPeriodTime -= interval;
                if (RemainVerticalPeriodTime <= 0)
                {
                    // new shift period
                    VerticalShiftRange = Mathf.Lerp(_verticalLimitMin, _verticalLimitMax, (float)randomNumber);
                }
            }
            else
            {
                if (VerticalShiftRange == 0)
                {
                    VerticalShiftRange = Mathf.Lerp(_verticalLimitMin, _verticalLimitMax, (float)randomNumber);
                }
                VerticalShift += _verticalSpeed * intervalInSecond * VerticalShiftDirection;
                // shift period
                if (VerticalShiftDirection > 0)
                {
                    // reach up boundary
                    if (VerticalShift >= VerticalShiftRange)
                    {
                        VerticalShiftDirection = -1;
                        VerticalShift = VerticalShiftRange;
                    }
                }
                else
                {
                    // reach down boundary
                    if (VerticalShift <= 0)
                    {
                        VerticalShift = 0;

                        RemainVerticalPeriodTime = (int)Mathf.Ceil(Mathf.Lerp(_verticalPeriodTimeMin, _verticalPeriodTimeMax, (float)randomNumber));
                        // ready for another period
                        VerticalShiftRange = 0;
                        VerticalShiftDirection = 1;
                    }
                }
            }

            // Horizontal
            bool directionChanged = false;
            HorizontalShift += _horizontalSpeed * intervalInSecond * HorizontalShiftDirection;
            if (HorizontalShift >= _horizontalLimit)
            {
                HorizontalShift = _horizontalLimit;
                HorizontalShiftDirection = -HorizontalShiftDirection;
                directionChanged = true;
            }
            if (HorizontalShift <= -_horizontalLimit)
            {
                HorizontalShift = -_horizontalLimit;
                HorizontalShiftDirection = -HorizontalShiftDirection;
                directionChanged = true;
            }
            if (!directionChanged && _horizontalPossibility.Result())
            {
                HorizontalShiftDirection = -HorizontalShiftDirection;
            }

            RandomSeed = (int)Mathf.Lerp(0, 65536, (float)randomNumber);
        }

        public void Clear()
        {
            // 继承水平偏移的方向，而非每次都从同一移动方向开始
            HorizontalShift = 0;
            VerticalShift = 0;
            VerticalShiftRange = 0;
            VerticalShiftDirection = 1;
            RemainVerticalPeriodTime = 0;
        }

        public void SetHoldBreath(bool value)
        {
            _holdBreath = value;
        }

        public void SetProne(bool value)
        {
            _prone = value;
        }

        // 可能的同步问题
        public void SetAttachmentFactor(float value)
        {
            _attachmentBuff = value;
        }
    }

    interface IPossibilityResult
    {
        bool Result();
        void SetRandomNumber(double number);
    }

    class RandomnessSimulator
    {
        private CSharpRandom _generator = new CSharpRandom();
        private double _randomNumber;

        private List<IPossibilityResult> _possibilityGenerator = new List<IPossibilityResult>();

        public void Reset(int seed)
        {
            _generator.Reset(seed);
        }

        public double Random()
        {
            _randomNumber = _generator.NextDouble();
            foreach (var v in _possibilityGenerator)
            {
                v.SetRandomNumber(_randomNumber);
            }

            return _randomNumber;
        }

        public IPossibilityResult GetSimplePossibilityExpression(float possibility)
        {
            var ret = new SimplePossibility(possibility);
            _possibilityGenerator.Add(ret);
            return ret;
        }

        class SimplePossibility : IPossibilityResult 
        {
            private double _randomNumber;
            private float _basePossibility;

            public SimplePossibility(float possibility)
            {
                _basePossibility = possibility;
            }

            public void SetRandomNumber(double number)
            {
                _randomNumber = number;
            }

            public bool Result()
            {
                return _randomNumber < _basePossibility;
            }
        }
    }
}
