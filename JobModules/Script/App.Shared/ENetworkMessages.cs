namespace App.Shared
{
    public enum EClient2ServerMessage
    {
        Login = 3,
        UserCmd,
        LocalDisconnect,
        LocalLogin,
        SimulationTimeSync,
        VehicleCmd,
        /// <summary>
        /// id 字段，拾取的时候是entityid，丢弃的时候是inventoryid
        /// </summary>
        PickEquipment,
        DropEquipment,
        PutOnEquipment,
        ClothChange,
        DebugCommand,
        FreeEvent,
        VehicleEvent,
        TriggerObjectEvent,
        Ping,
        UpdateMsg,
        FireInfo,
        DebugScriptInfo,
        GameOver,
        Max,
    }

    public enum EServer2ClientMessage
    {
        Snapshot = EClient2ServerMessage.Max + 1,
        LoginSucc,
        PlayerInfo,
        SimulationTimeSync,
        UdpId,
        FreeData,
        Statistics,
        Ping,
        DamageInfo,
        UpdateAck,
        FireInfoAck,
        DebugMessage,
        ClearScene,
        GameOver,
        HeartBeat,
        Max
    }

}