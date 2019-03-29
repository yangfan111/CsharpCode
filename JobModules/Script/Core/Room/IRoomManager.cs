using System.Collections.Generic;
using Core.Network;

namespace Core.Room
{
    public interface IRoomListener
    {
        void OnRoomCreated(IRoom room);
        void OnRoomDispose(IRoom room);
    }

    public interface IRoomManager
    {
        void CreateRoomIfNotExist();
        bool RequestPlayerInfo(string messageToken, INetworkChannel channel);
        bool RequestRoomInfo(string messageToken, INetworkChannel channel);
        void SetPlayerStageRunning(string messageToken, INetworkChannel channel);
        void AddRoom(IRoom room);
        IRoom GetNewRoom();
        IRoom GetRoom(int roomId);

        void AddPlayerInfo(string token, IPlayerInfo room);
        void RemovePlayerInfo(string token);
        bool HasPlayerInfo(string token);
        void Update();
        void LateUpdate();

        List<IPlayerInfo> GetRobotPlayerInfos();
      
    }
}