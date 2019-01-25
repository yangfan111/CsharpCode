using Core.Appearance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.Appearance
{
    public class FirstPersonAppearanceManager : ICharacterFirstPersonAppearance
    {
        private readonly SightShift _sightShift;
        private GameObject _characterP1;
        private readonly IPredictedAppearanceState _state;

        public FirstPersonAppearanceManager(IPredictedAppearanceState state)
        {
            _state = state;
            if (_state != null)
            {
                _state.FirstPersonHeight = AnimatorParametersHash.FirstPersonStandCameraHeight;
                _state.FirstPersonForwardOffset = AnimatorParametersHash.FirstPersonStandCameraForwardOffset;
                _sightShift = new SightShift(_state);
            }
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterP1 = obj;
        }

        public IFirstPersonSight SightShift { get { return _sightShift; } }

        public void SetFirstPersonHeight(float value)
        {
            _state.FirstPersonHeight = value;
        }

        public void SetFirstPersonForwardOffset(float value)
        {
            _state.FirstPersonForwardOffset = value;
        }


        public void Update()
        {
            var lastPosition = _characterP1.transform.localPosition;
            lastPosition.y = _state.FirstPersonHeight;
            lastPosition.z = _state.FirstPersonForwardOffset;
            _characterP1.transform.localPosition = lastPosition;
        }
    }
}
