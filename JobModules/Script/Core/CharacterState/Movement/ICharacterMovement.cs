using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Core.CharacterState.Movement
{
    public interface ICharacterMovement: IGetMovementState
    {

        bool IsForth { get; }
        bool IsBack { get; }
        bool IsLeft { get; }
        bool IsRight { get; }
        bool IsUp { get; }
        bool IsDown { get; }

        float HorizontalValue { get; }
        float VerticalValue { get; }
        float UpDownValue { get; }

        void UpdateAxis(float horizontalValue, float verticalValue, float upDownValue);
    }

    public interface ICharacterMovementInConfig
    {
        bool InTransition();
        float TransitionRemainTime();
        float TransitionTime();
        MovementInConfig CurrentMovement();
        MovementInConfig NextMovement();
    }
}
