using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Appearance;

namespace Core.Appearance
{
    public interface ICharacterFirstPersonAppearance
    {
        void SetFirstPersonCharacter(GameObject obj);
        IFirstPersonSight SightShift { get; }
        void Update();

        void SetFirstPersonHeight(float value);
        void SetFirstPersonForwardOffset(float value);
        
    }
}
