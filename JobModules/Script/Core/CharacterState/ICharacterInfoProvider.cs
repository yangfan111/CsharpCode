using XmlConfig;

namespace Core.CharacterState
{

    public interface ICharacterSpeedInfo
    {
        float GetJumpSpeed();
    }

    public interface ICharacterBoundsInfo
    {
        CharacterControllerCapsule GetStandCapsule();
        CharacterControllerCapsule GetCrouchCapsule();
        CharacterControllerCapsule GetProneCapsule();
        float GetBigJumpHeight();
        float GetSlopeLimit();
        float GetStepOffset();
    }
    
    public interface ICharacterInfoProvider:ICharacterSpeedInfo,ICharacterBoundsInfo
    {
        
    }
    
    public interface ICharacterInfoProviderContext : ICharacterInfoProvider
    {
        void SetCurrentType(int type);
    }
}