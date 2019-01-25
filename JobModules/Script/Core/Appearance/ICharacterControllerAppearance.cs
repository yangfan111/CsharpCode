using Core.CharacterController;
using UnityEngine;

namespace Core.Appearance
{
    public interface ICharacterControllerAppearance
    {
        void SetCharacterController(ICharacterControllerContext controller);
        void SetCharacterRoot(GameObject characterRoot);

        void PlayerDead();
        void PlayerReborn();
        
        void SetCharacterControllerHeight(float height, bool baseOnFoot = true);
        float GetCharacterControllerHeight { get; }
        void SetCharacterControllerCenter(Vector3 value);
        Vector3 GetCharacterControllerCenter { get; }
        void SetCharacterControllerRadius(float value);
        float GetCharacterControllerRadius { get; }
    }
}