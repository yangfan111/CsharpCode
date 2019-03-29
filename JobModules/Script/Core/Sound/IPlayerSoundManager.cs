using XmlConfig;

namespace Core.Sound
{
    public interface IPlayerSoundManager
    {
        int Play(EPlayerSoundType type);
        int Play(int id);
        void Stop(ref int id);
        int PlayOnce(EPlayerSoundType soundType);
        int PlayOnce(int id);
    }
}