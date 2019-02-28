using System;
using Core.Network;

namespace Core.Room
{

    public interface IRoom : IDisposable
    {
        IRoomId RoomId { get; }
        bool LoginPlayer(IPlayerInfo playerInfo, INetworkChannel channel);
        void Start();
        void Update(int interval);
        void LateUpdate();
        void SetHallRoom(IHallRoom hallRoom);
        void SendSnapshot();
        void CompensationSnapshot();
        void RunFreeGameRule();
        void GameOver(bool forceExit);
        void SetGameMode(int mode);
        bool SendLoginSucc(IPlayerInfo playerInfo, INetworkChannel channel);
        void SetPlayerStageRunning(IPlayerInfo playerInfo, INetworkChannel channel);
        PlayerInfo PlayerJoin(long hallRoomId, object roomPlayerInfo, out int errorCode);
        bool IsDiposed { get; }
        bool IsGameOver { get; }
    }
}