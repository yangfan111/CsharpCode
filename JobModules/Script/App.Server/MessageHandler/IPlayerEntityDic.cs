using Core.Network;

namespace App.Server.MessageHandler
{
    public interface IPlayerEntityDic<TPlayer>
    {
        TPlayer GetPlayer(INetworkChannel channel);
    }
}