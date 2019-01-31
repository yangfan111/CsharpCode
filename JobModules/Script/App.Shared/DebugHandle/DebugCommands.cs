namespace App.Shared.DebugHandle
{
    public class DebugCommand
    {
        public string Command { get; set; }
        public string[] Args { get; set; }

        public DebugCommand(string command)
        {
            Command = command;
            Args = new string[0];
        }
        public DebugCommand(string command, string[] args)
        {
            Command = command;
            Args = args;
        }
    }

    public class DebugCommands
    {
        public const string SetFrameRate = "SFR";
        public const string GetQuality = "GetQuality";
        public const string GetQualityList = "GetQualityList";
        public const string SetQuality = "SetQuality";

        public const string Quality = "Quality";

        public const string ShowDrawHitBoxOnBullet = "ShowDrawHitBoxOnBullet";
        public const string HideDrawHitBoxOnBullet = "HideDrawHitBoxOnBullet";
        public const string ShowDrawHitBoxOnFrame = "ShowDrawHitBoxOnFrame";
        public const string HideDrawHitBoxOnFrame = "HideDrawHitBoxOnFrame";
        public const string EnableDrawBullet = "ShowBullet";
        public const string DisableDrawBullet = "HideBullet";
        public const string KillMe = "Kill";
        public const string ChangeHp = "hp";
        public const string TestFrame = "testFrame";

        public const string ShowVehicleDebugInfo = "SVD";
        public const string SetDynamicPrediction = "SDP";
        public const string SetVehicleHp = "SVH";
        public const string SetVehicleFuel = "SVF";
        public const string SetVehicleInput = "SVI";
        public const string DragCar = "DCar";
        public const string ShowExplosionRange = "ShowExplosionRange";
        public const string HideExplosionRange = "HideExplosionRange";
        public const string EnableVehicleCollisionDamage = "EVC";
        public const string EnableVehicleCollisionDebug = "EVD";
        public const string SetVehicleDynamicPrediction = "SVP";
        public const string ShowClientVehicle = "ClientVehicle";
        public const string ShowServerVehicle = "ServerVehicle";
        public const string ResetVehicle = "ResetVehicle";
        public const string EnableVehicleCull = "VCull";

        public const string SetCurBullet = "sb";
        public const string SetReservedBullet = "srb";

        public const string SetWeapon = "sw";
        public const string DropWeapon = "dw";
        public const string TestWeaponAssemble = "twa";

        public const string SetAttachment = "sa";
        public const string ClearAttachment = "ca";
        public const string SwitchAttachment = "swa";
        public const string ShowAvaliablePartType = "showpt";
        public const string ReloadConfig = "rc";

        public const string SetEquip = "se";

        public const string ShowConfig = "showconfig";
        public const string CreateSceneObject = "sobj";
        public const string ClearSceneObject = "clrsobj";
        public const string ShowArtTools = "art";
        public const string ListDoorEntity = "listDoorEntity";

        public const string ShowTerrainTrace = "stt";
        public const string PrintEntity = "printEntity";
        public const string ListEntity = "listEntity";
        public const string CountEntity = "countEntity";
        
        public const string ShowAniInfo = "ShowAniInfo";
        public const string ShowBox = "ShowBox";
        public const string ShowGround = "ShowGround";


        public const string ClientMove = "m";
        public const string DebugTime = "dt";

        public const string QualitySetting = "qs";
        public const string Dump = "dump";
        public const string Culling = "culling";
        public const string Terrain = "terrain";
        public const string SetFps = "setfps";
        public const string TreeDistance = "tree";
        public const string LodBias = "lod";
        public const string TestMap = "tt";

        public const string UIList = "uils";
        public const string HideUI = "hideui";
        public const string ShowUI = "showui";

        public const string EnableProfiler = "enprof";

        public const string HeatMapPause = "heatmappause";
        public const string HeatMapRestart = "heatmaprestart";
        public const string HeatMapStopAndExit = "heatmapstop&exit";
        public const string HeatMapStop = "heatmapstop";
        public const string HeatMapPoints = "heatmappoints";
        public const string HeatMapScenes = "heatmapscenes";
        public const string EnableRecordProfiler = "enrp";

        public const string EnableFlagImmutability = "efi";

        public const string SetMaxQuality = "SetMaxHighQuality";
        public const string GetMaxQuality = "GetMaxHighQuality";

        public const string EnableMinRendererSet = "enableminrenderset";
        public const string DisableMinRendererSet = "disableminrenderset";

        public const string WaterReflectUseCam = "waterreflectusecam";
        public static string Event = "event";
        public static string FilterPlayer = "filterPlayer";

        public const string ForbidSystem = "stopsys";
        public const string PermitSystem = "startsys";
        public const string ShowSystem = "sys";    }

    public interface IDebugCommandHandler
    {
        string OnDebugMessage(DebugCommand message);
    }
}