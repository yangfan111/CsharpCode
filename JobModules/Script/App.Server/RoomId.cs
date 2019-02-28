using Core.Room;

namespace App.Server
{
    public struct RoomId : IRoomId
    {
        public RoomId(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}