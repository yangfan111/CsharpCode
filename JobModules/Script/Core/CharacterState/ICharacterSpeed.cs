using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture;
using UnityEngine;

namespace Core.CharacterState
{
    public interface ICharacterSpeed
    {
        Vector3 GetSpeed(Vector3 lastVel, float deltaTime, float buff = 0);
        Vector3 GetSpeedOffset(float buff = 0);
        // 人物速度增加
        void SetSpeedAffect(float affect);
        float SpeedRatio();
    }
}
