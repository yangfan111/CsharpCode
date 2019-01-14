using Core.Network;

namespace Core.Room
{
    public interface IRoom
    {
        IRoomId RoomId { get; }
        bool LoginPlayer(IPlayerInfo playerInfo, INetworkChannel channel);
        void Update(int interval);
        void LateUpdate();
        void SetHallRoom(IHallRoom hallRoom);
        void Reset();
        void SendSnapshot();
        void CompensationSnapshot();
        void RunFreeGameRule();
        void GameOver();
        void SetGameMode(int mode);
        void SetTeamCapacity(int capacity);
        bool SendLoginSucc(IPlayerInfo playerInfo, INetworkChannel channel);
        void SetPlayerStageRunning(IPlayerInfo playerInfo, INetworkChannel channel);
    }
}