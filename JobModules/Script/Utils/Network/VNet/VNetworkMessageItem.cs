using Core.Network.ENet;

namespace VNet
{
    internal class VNetworkMessageItem: NetworkMessageItem
    {
        public VNetworkMessageItem(int messageType, object messageBody, int channel) : base(messageType, messageBody)
        {
            this.Channel = channel;
        }

        public int Channel { get; set; }
    }
}
