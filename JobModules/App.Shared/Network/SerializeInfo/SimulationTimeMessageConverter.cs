using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Core.Prediction.VehiclePrediction.TimeSync;


namespace App.Shared.Network
{
    public class SimulationTimeMessageConverter
    {

        public static SimulationTimeMessage FromProtoBuf(SimulationTimeMessageItem item)
        {
            var msg = new SimulationTimeMessage()
            {
                ClientSimulationTime = item.ClientSimulationTime,
                ServerSimulationTime = item.ServerSimulationTime
            };

            return msg;
        }

        public static SimulationTimeMessageItem ToProtoBuf(SimulationTimeMessage msg)
        {
            var item = SimulationTimeMessageItem.Allocate();

            item.ClientSimulationTime = msg.ClientSimulationTime;
            item.ServerSimulationTime = msg.ServerSimulationTime;
            

            return item;
        }
    }
}
