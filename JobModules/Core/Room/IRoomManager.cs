using System.Collections.Generic;
using Core.Network;

namespace Core.Room
{
    public interface IRoomManager
    {
        bool RequestPlayerInfo(string messageToken, INetworkChannel channel);
        bool RequestRoomInfo(string messageToken, INetworkChannel channel);
        void SetPlayerStageRunning(string messageToken, INetworkChannel channel);
        void AddRoom(IRoom room);
        IRoom GetNewRoom();

        void AddPlayerInfo(string token, IPlayerInfo room);
        void RemovePlayerInfo(string token);
        bool HasPlayerInfo(string token);
        void Update();
        void LateUpdate();

        List<IPlayerInfo> GetRobotPlayerInfos();
      
    }
}