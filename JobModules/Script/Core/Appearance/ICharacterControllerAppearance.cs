using Core.CharacterController;
using UnityEngine;

namespace Core.Appearance
{
    public interface ICharacterControllerAppearance
    {
        void SetCharacterController(ICharacterControllerContext controller);
        void SetCharacterRoot(GameObject characterRoot);
        void SetThirdModel(GameObject model);

        void PlayerDead(bool isSelf = true);
        void PlayerReborn();
        
        void SetCharacterControllerHeight(float height, bool updateCapsule, bool baseOnFoot = true);
        float GetCharacterControllerHeight { get; }
        void SetCharacterControllerCenter(Vector3 value, bool updateCapsule);
        Vector3 GetCharacterControllerCenter { get; }
        void SetCharacterControllerRadius(float value, bool updateCapsule);
        float GetCharacterControllerRadius { get; }
    }
}