using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.SceneManagement;
using App.Protobuf;

using App.Shared.DebugHandle;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Event;
using App.Shared.SceneManagement;
using Core.Components;
using UnityEngine;
using Wenzil.Console;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.Step;
using Core.MyProfiler;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Utils.Configuration;
using Console = System.Console;
using ArtPlugins;
using Core.Event;
using Utils.Singleton;

namespace App.Shared.Scripts
{
    public class DeveloperConsoleCommands : MonoBehaviour
    {
        public IDebugCommandHandler _handler;
        public IEcsDebugHelper _EcsDebugHelper;
        public Contexts _contexts;
        public LoggerAdapter logger = new LoggerAdapter(typeof(DeveloperConsoleCommands));

        void Start()
        {
            ConsoleCommandsDatabase.RegisterCommand("SFR",
                SetFrameRate,
                description: "Set Application Frame Rate",
                usage: "SFR [s/c] FrameRate"
            );
            ConsoleCommandsDatabase.RegisterCommand("SetQuality",
                SetQuality,
                description: "Set Client Quality",
                usage: "1.Quality\n" +
                       "2.Quality all\n" +
                       "3.Quality QualityLevelIndex");

            ConsoleCommandsDatabase.RegisterCommand("Quality",
                SetCustomQuality,
                description: "Set Custom Quality",
                usage: "Quality level\n" +
                       "level 1 2 3 4 5");

            ConsoleCommandsDatabase.RegisterCommand("HitBox",
                ShowDrawHitBoxOnBullet,
                description: "Show/Hide HitBox on Hit/Frame",
                usage: "HitBox [show/hide] [bullet/frame]");

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.EnableDrawBullet,
                ShowBullet,
                description: "start draw bullet move line",
                usage: "ShowBullet"
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.DisableDrawBullet,
                HideBullet,
                description: "stop draw bullet move line",
                usage: "HideBullet"
            );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.KillMe,
                KillMe,
                description: "(Role)KillMe: Kill yourself",
                usage: DebugCommands.KillMe);

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ChangeHp,
                ChangeHp,
                description: "(Role)ChangeHp: ",
                usage: DebugCommands.ChangeHp + " num");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.DragCar,
                DragCar,
                description: "(Vehicle)DragCar: Drag a neighboring car",
                usage: DebugCommands.DragCar);

            ConsoleCommandsDatabase.RegisterCommand("explo",
                ShowExplosionRange,
                description: "(Vehicle)Explosion: Change Vehicle Explosion ",
                usage: "explo [Show/Hide] range");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowVehicleDebugInfo,
                ShowVehicleDebugInfo,
                description: "(Vehicle) [Show/Hide] Vehicle Debug Info",
                usage: DebugCommands.ShowVehicleDebugInfo);

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetVehicleHp,
                SetVehicleHp,
                description: "(Vehicle)SetVehicleHp: Set Vehicle Hp",
                usage: DebugCommands.SetVehicleHp + " VehicleId Hp");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableVehicleCollisionDamage,
                EnableVehicleCollisionDamage,
                description: "(Vehicle)EnableVehicleCollisionDamage: Enable Vehicle Collision Damage",
                usage: DebugCommands.EnableVehicleCollisionDamage + " [0/1]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableVehicleCollisionDebug,
                EnableVehicleCollisionDebug,
                description: "(Vehicle)EnableVehicleCollisionDamage: Enable Vehicle Collision Debug",
                usage: DebugCommands.EnableVehicleCollisionDebug + " [0/1]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetVehicleDynamicPrediction,
                SetVehicleDynamicPrediction,
                description: "(Vehicle) SetVehicleDynamicPrediction",
                usage: DebugCommands.SetVehicleDynamicPrediction + " [0/1]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowClientVehicle,
                ShowClientVehicle,
                description: "ShowClientVehicle",
                usage: DebugCommands.ShowClientVehicle);

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowServerVehicle,
                ShowServerVehicle,
                description: "ShowClientVehicle",
                usage: DebugCommands.ShowServerVehicle);

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ResetVehicle,
                ResetVehicle,
                description: "ResetVehicle",
                usage: DebugCommands.ResetVehicle + " [VehicleId]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableVehicleCull,
                EnableVehicleCull,
                description: "Enable/Disable Vehcile Culling",
                usage: DebugCommands.EnableVehicleCull + "[IsServer(0/1)] [Enable (0/1)]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.TestFrame,
                TestFrame,
                description: "测试全图帧率",
                usage: "testFrame 0,0,100,10 表示从0，0点开始到1000，1000结束，步长为100米");


            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapRestart,
                StartSampler,
                description: "开启数据采集",
                usage: "startSampler");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapPause,
                StopSampler,
                description: "关闭数据采集",
                usage: "stopSampler");


            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetVehicleFuel,
                SetVehicleFuel,
                description: "(Vehicle)SetVehicleFuel: Set Vehicle Fuel Volume",
                usage: DebugCommands.SetVehicleFuel + " VehicleId FuelVolume");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetVehicleInput,
                SetVehicleInput,
                description: "(Vehicle)SetVehicleInput: Set Vehicle Input T: Throttle, S: Steer, H: HandBrake, B Brake",
                usage: "1." + DebugCommands.SetVehicleInput + " VehicleId [T/S/H/B] value\n" +
                       "2." + DebugCommands.SetVehicleInput + " VehicleId ThrottleInput SteerInput\n" +
                       "3." + DebugCommands.SetVehicleInput +
                       " VehicleId ThrottleInput SteerInput HandBrakeInput BrakeInput\n");

            ConsoleCommandsDatabase.RegisterCommand("ls",
                ListEntites,
                description: "List the Players and Vehicles",
                usage: "ls [Player|Vehicle]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetCurBullet,
                SetCurrentBulet,
                description: "(Weapon)SetBullet: Set current weapon's bullet to assigned number",
                usage: DebugCommands.SetCurBullet + " number ");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetReservedBullet,
                SetReservedBullet,
                description: "(Weapon)SetReservedBullet:Set reserved weapon's bullet to assigned number",
                usage: DebugCommands.SetReservedBullet + " number ");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetEquip,
                SetEquip,
                description: "SetEquipment : set equipment to assigned item",
                usage: DebugCommands.SetReservedBullet + " ItemId");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetWeapon,
                SetWeapon,
                description: "设置当前或指定位置的武器",
                usage: DebugCommands.SetWeapon + " ItemID");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.DropWeapon,
                DropWeapon,
                description: "删除当前或指定位置的武器",
                usage: DebugCommands.SetWeapon + " ItemID");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.TestWeaponAssemble,
                TestWeaponAssemble,
                description: "此时武器组装逻辑",
                usage: DebugCommands.TestWeaponAssemble);

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.SetAttachment,
                SetAttachment,
                description: "设置当前或指定位置武器的配件",
                usage: DebugCommands.SetAttachment + " attachmentId"
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ClearAttachment,
                ClearAttachment,
                description: "删除当前或指定位置武器的配件",
                usage: DebugCommands.ClearAttachment
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ShowAvaliablePartType,
                ShowAvaliablePartType,
                description: "显示当前武器的可用配件类型列表",
                usage: DebugCommands.ShowAvaliablePartType
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.SwitchAttachment,
                SwitchAttachment,
                description: "交换指定两个位置的对应配件",
                usage: DebugCommands.SwitchAttachment
            );



            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ShowConfig,
                ShowConfig,
                description: "ShowConfig: show xml content eg attachment",
                usage: DebugCommands.ShowConfig + " configName "
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.CreateSceneObject,
                CreateSceneObj,
                description: "在附近生成一个物件",
                usage: DebugCommands.CreateSceneObject + " itemId "
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ReloadConfig,
                ReloadConfig,
                description: "重新载入配置",
                usage: DebugCommands.ReloadConfig
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ClearSceneObject,
                ClearSceneObj,
                description: "ClearSceneObject: clear all scene objects ",
                usage: DebugCommands.ClearSceneObject
            );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ListDoorEntity,
                ListDoorEntity,
                description: "List All Scene Entities of Doors in the scene",
                usage: DebugCommands.ListDoorEntity);

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ShowArtTools,
                ShowArtTools,
                description: "显示TA工具",
                usage: DebugCommands.ShowArtTools
            );

            ConsoleCommandsDatabase.RegisterCommand(
                DebugCommands.ShowTerrainTrace,
                ShowTerrainTrace,
                description: "显示Terrain输出测试信息",
                usage: DebugCommands.ShowTerrainTrace
            );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.PrintEntity,
                PrintEntitas,
                description: "显示entita的数据 entityid entityType",
                usage: DebugCommands.PrintEntity
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ListEntity,
                ListEntitas,
                description: "显示entita列表",
                usage: DebugCommands.ListEntity
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.CountEntity,
                CountEntity,
                description: "显示entitag个数",
                usage: DebugCommands.CountEntity
                );
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowAniInfo,
                ShowAniInfo,
                description: "(Role)show animator info",
                usage: DebugCommands.ShowAniInfo);

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ClientMove,
                 ClientMove,
                 description: "在离线模式下移动",
                 usage: DebugCommands.ClientMove
                 );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.DebugTime,
                DebugTime,
                description: "打印时间",
                usage: DebugCommands.DebugTime
                );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.Dump,
                Dump,
                description: "输出客户端相关信息",
                usage: DebugCommands.Dump
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.Culling,
                Culling,
                description: "切换裁剪使能状态",
                usage: DebugCommands.Culling
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.Terrain,
                TerrainSetting,
                description: "切换地形状态",
                usage: DebugCommands.Terrain
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.QualitySetting,
                QualitySetting,
                description: "输出客户端全局配置信息",
                usage: DebugCommands.QualitySetting
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetFps,
                SetFps,
                description: "设置FPS cmd 命令 ui UI",
                usage: DebugCommands.SetFps
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.LodBias,
                LodBias,
                description: "显示和调整LodBias",
                usage: DebugCommands.LodBias + "value");
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.TestMap,
               TestMap,
               description: "测试大地图",
               usage: DebugCommands.TestMap + "value");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.TreeDistance,
                 TreeDistance,
                 description: "显示和设置树木距离,以及草的密度",
                 usage: DebugCommands.ClientMove
                 );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.UIList,
                PrintUIList,
                 description: "UI列表",
                 usage: DebugCommands.UIList
                 );

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowUI,
                ShowUi,
                description: "显示UI",
                usage: DebugCommands.ShowUI
                );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HideUI,
                HideUi,
                description: "隐藏UI",
                usage: DebugCommands.HideUI
             );
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableProfiler,
                EnableProfiler,
                description: "Enable/Disable Profiler",
                usage: "1.Show Client Profiler State:" + DebugCommands.EnableProfiler + "\n" +
                       "2.Enable/Disable Client Profiler:" + DebugCommands.EnableProfiler + "-c [1/0] + [-g 1/0]\n" +
                       "3.Enable/Disable Server Profiler:" + DebugCommands.EnableProfiler + "-s [1/0]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableRecordProfiler,
                EnableRecordProfiler,
                description: "Enable/Disable Record Profiler",
                usage: "1.Show Client Record Profiler State:" + DebugCommands.EnableRecordProfiler + "\n" +
                       "2. Enable/Disable Record Client Profiler:" + DebugCommands.EnableRecordProfiler + "[0/1]\n" +
                       "3.Enable/Disable Record Server/Client Profiler:" + DebugCommands.EnableRecordProfiler + "[c/s] [1/0]");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableFlagImmutability,
                EnableFlagImmutability,
                description: "Enable/Disable EnableFlagImmutability",
                usage: "1.Show Client EnableFlagImmutability:" + DebugCommands.EnableRecordProfiler + "\n" +
                       "2. Enable/Disable EnableFlagImmutability:" + DebugCommands.EnableRecordProfiler + "[0/1]\n");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapStopAndExit,
                HeatMapStopAndExit,
                description: "Force stop heatmap sampler and exit exe , only used in special client exe,\nparams mean whether to upload sampler data to website",
                usage: string.Format("{0} false/true", DebugCommands.HeatMapStopAndExit));

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapStop,
                HeatMapStop,
                description: "force stop heatmap but doesn't exit exe, only used in special client exe,\n params mean whether to upload sampler data to website",
                usage: string.Format("{0} false/true", DebugCommands.HeatMapStop));

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapPoints,
                HeatMapPoints,
                description: "allow to sample some points, only used in special client exe",
                usage: string.Format("heatmappoints [(x1,y1,z1),(x2,y2,z2),...] false/true"));

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.HeatMapScenes,
                HeatMapScenes,
                description: "allow to sample some scenes, only used in special client exe",
                usage: string.Format("heatmapscenes [(x1,y1),(x2,y2),...] false/true"));

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.EnableMinRendererSet,
                EnableMinRenderSet,
                description: "Disable all meshrenderers that will not be seen",
                usage: "enableminrenderset");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.DisableMinRendererSet,
                DisableMinRenderSet,
                description: "Recovery meshrenderers that be disabled through 'enableminrenderset' command",
                usage: "disableminrenderset");            
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ForbidSystem,
                ForbidSystem,
                description: "禁止对应系统及子系统运行 ",
                usage: "ForbidSystem");            
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.PermitSystem,
                PermitSystem,
                description: "允许对应系统及子系统运行",
                usage: "PermitSystem");            
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.ShowSystem,
                ShowSystem,
                description: "显示所有受控系统",
                usage: "ShowSystem");

            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.WaterReflectUseCam,
                WaterReflectUseCam,
                description: "toggle water reflection mode, whether to use camera.render function",
                usage: "waterreflectusecam true/false");
            
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.Event,
                ManageEvent,
                description: "enable disable events",
                usage: "event {id} {1/0}");
            
            ConsoleCommandsDatabase.RegisterCommand(DebugCommands.FilterPlayer,
                FilterPlayer,
                description: "enable FilterPlayer ",
                usage: "filterPlayer {1/0}");

            var debugCommands = SharedCommandHandler.GetDebugCommands();
            foreach (var cmd in debugCommands)
            {
                ConsoleCommandsDatabase.RegisterCommand(cmd.Name, (args) => { return _handler.OnDebugMessage(new DebugCommand(cmd.GetType().ToString(), args)); }, cmd.Desc, cmd.Usage);
            }

            //ConsoleCommandsDatabase.RegisterCommand(DebugCommands.SetMaxQuality,
            //SetMaxQuality,
            //description: "设置最高等级质量",
            //usage: string.Format("{0} yes/no", DebugCommands.SetMaxQuality));

            //ConsoleCommandsDatabase.RegisterCommand(DebugCommands.GetMaxQuality,
            //GetMaxQuality,
            //description: "获取最高等级质量",
            //usage: string.Format("{0}", DebugCommands.GetMaxQuality));
        }

        private string FilterPlayer(string[] args)
        {
            PlayerPlayBackFilterSystem.Enable = args[0].Equals("1");
            return PlayerPlayBackFilterSystem.Enable + "";
        }

        private string ManageEvent(string[] args)
        {
            if (args.Length == 2)
            {
                int id = int.Parse(args[0]);
                if (id >= PlayerEventsExtensions.EventSwitch.Length)
                {
                    return string.Format("id muse < {0}",PlayerEventsExtensions.EventSwitch.Length );
                }

                PlayerEventsExtensions.EventSwitch[id] = !args[1].Equals("0");
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < PlayerEventsExtensions.EventSwitch.Length; i++)
            {
                sb.Append(string.Format("{0} {1} {2}\n", i, (EEventType) i, PlayerEventsExtensions.EventSwitch[i]));
                
            }
            return sb.ToString();
        }

        private string EnableFlagImmutability(string[] args)
        {
            if (args.Length == 1)
            {
                FlagImmutabilityComponent.DisableImmutability = args[0].Equals("1");
            }

            return "" + FlagImmutabilityComponent.DisableImmutability;
        }


        private string TestMap(string[] args)
        {
            if (null != _handler)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.TestMap, args));
            }

            return "命令执行失败";
        }

        private string SetFps(string[] args)
        {
            if (args.Length != 2) return "error";
            int fps = int.Parse(args[1]);
            if (args[0].Equals("cmd"))
            {
                StepExecuteManager.Instance.SetFps(EEcecuteStep.CmdFrameStep, fps);
            }
            else if (args[0].Equals("ui"))
            {
                StepExecuteManager.Instance.SetFps(EEcecuteStep.UIFrameStep, fps);
            }

            return "ok";
        }

        private string DebugTime(params string[] args)
        {
            var r = SingletonManager.Get<DurationHelp>().GetCmdTable();
            logger.Error(r);
            return r;
        }

        public void RegisterFreeCommand(string command, string desc, string usage)
        {
            ConsoleCommandsDatabase.RegisterFreeCommand(
                command,
                FreeCommand,
                description: desc,
                usage: usage
            );
        }

        private string FreeCommand(string command, params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(command, args));
            }
            return "OK";
        }

        public void RegisterOpenCallback(Action<bool> callback)
        {
            ConsoleCommandsDatabase.RegisterConsoleOpenCallback(callback);
        }

        public string SetFrameRate(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length < 2)
                {
                    return "Argument Error!";
                }
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetFrameRate, args));
            }

            return "OK";
        }

        public string PrintEntitas(params string[] args)
        {
            if (args.Length == 2)
            {
                var entity = _contexts.GetEntityWithEntityKey(new EntityKey(int.Parse(args[0]), short.Parse(args[1])));
                return entity.Dump();
            }


            return "kill me baby";
        }
        public string ListEntitas(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (EEntityType type in (EEntityType[])Enum.GetValues(typeof(EEntityType)))
            {
                sb.Append(type + ":\n");
                foreach (var entity in _contexts.GetEntitysWithEntityKey(type))
                {
                    sb.Append("    " + entity + "\n");
                }
            }
            return sb.ToString();
        }
        public string CountEntity(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (EEntityType type in (EEntityType[])Enum.GetValues(typeof(EEntityType)))
            {
                sb.Append(type).Append(":").Append(_contexts.GetEntitysWithEntityKey(type).Length).Append("\n");

            }
            return sb.ToString();
        }

        public string ShowAniInfo(params string[] args)
        {
            
            PlayerEntity player = _contexts.player.flagSelfEntity;
            if (player != null && player.hasState && player.hasThirdPersonAnimator)
            {                
                var result = string.Format("{0}\n{1}", player.state, player.thirdPersonAnimator.DebugJumpInfo());                
                return result;
            }
                
            if (null != _handler)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowAniInfo, args));
            }

            return "";
        }

        public string ClientMove(params string[] args)
        {
            if (null != _handler)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.ClientMove, args));
            }
            return "未工作，请联系程序";
        }


        public string SetQuality(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length == 0)
                {
                    //get current quality level
                    var cmd = new DebugCommand(DebugCommands.GetQuality, null);
                    _handler.OnDebugMessage(cmd);
                    if (cmd.Args == null)
                    {
                        return "Can not get current quality level.";
                    }

                    var strBuilder = new StringBuilder();
                    strBuilder.Append("current level:" + cmd.Args[0] + "\n");

                    //get all quality levels
                    cmd = new DebugCommand(DebugCommands.GetQualityList, null);
                    _handler.OnDebugMessage(cmd);
                    if (cmd.Args == null)
                    {
                        strBuilder.Append("Can not access all quality levels.");
                    }
                    else
                    {
                        strBuilder.Append("All Levels:\n");

                        for (int i = 0; i < cmd.Args.Length; ++i)
                        {
                            var qualityName = cmd.Args[i];
                            strBuilder.Append(i + "." + qualityName + "\n");
                        }
                    }


                    return strBuilder.ToString();
                }

                if (args.Length == 1)
                {
                    _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetQuality, args));
                    return "OK";
                }

                return "Argument Error!";
            }

            return "OK";
        }
        public string SetCustomQuality(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length == 0)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append("Can not get current quality level" + "\n");
                    strBuilder.Append("all levels:  1 2 3 4 5" + "\n");

                    return strBuilder.ToString();
                }

                if (args.Length == 1)
                {
                    _handler.OnDebugMessage(new DebugCommand(DebugCommands.Quality, args));
                    return "OK";
                }

                return "Argument Error!";
            }

            return "OK";
        }

        public string ShowDrawHitBoxOnBullet(params string[] args)
        {

            if (_handler != null)
            {
                string show = args.Length > 0 ? args[0].ToLower() : "show";
                string mode = args.Length > 1 ? args[1].ToLower() : "frame";
                if (show == "hide")
                {
                    if (mode == "bullet")
                    {
                        _handler.OnDebugMessage(new DebugCommand(DebugCommands.HideDrawHitBoxOnBullet));
                        return "HideDrawHitBoxOnBullet";
                    }
                    else
                    {
                        _handler.OnDebugMessage(new DebugCommand(DebugCommands.HideDrawHitBoxOnFrame));
                        return "HideDrawHitBoxOnFrame";
                    }
                }
                else
                {
                    if (mode == "bullet")
                    {
                        _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowDrawHitBoxOnBullet));
                        return "ShowDrawHitBoxOnBullet";
                    }
                    else
                    {
                        _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowDrawHitBoxOnFrame));
                        return "ShowDrawHitBoxOnFrame";
                    }

                }
            }
            return "Invalid arguments list";
        }

        public string ShowBullet(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableDrawBullet, args));
            }
            return "OK";
        }

        public string HideBullet(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.DisableDrawBullet, args));
            }
            return "OK";
        }

        public string SetWeapon(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetWeapon, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! item id should be int";
            }
            var weapon = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(i);
            if (null == weapon)
            {
                return string.Format("no weapon with id {0}", i);
            }
            return "OK";
        }

        public string DropWeapon(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.DropWeapon, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! slot should be int";
            }
            return "OK";
        }

        public string TestWeaponAssemble(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.TestWeaponAssemble, args));
            }
            return "OK";
        }

        public string SetEquip(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetEquip, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! bulletCount should be int";
            }
            var asset = SingletonManager.Get<AvatarAssetConfigManager>().GetAssetInfoById(i);
            if (string.IsNullOrEmpty(asset.AssetName))
            {
                return "illegal equipment id ";
            }
            return "OK";
        }

        public string ShowConfig(params string[] args)
        {
            var result = string.Empty;
            if (null != _handler)
            {
                result = _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowConfig, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            var name = args[0] as string;
            if (string.IsNullOrEmpty(name))
            {
                return "Illegal Argument";
            }
            result += "\n ok";
            return result;
        }

        public string ClearSceneObj(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ClearSceneObject, args));
            }
            return "ok";
        }

        public string ListDoorEntity(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ListDoorEntity, args));
            }

            return "ok";
        }

        public string CreateSceneObj(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.CreateSceneObject, args));
            }
            if (args.Length < 2)
            {
                return "Argument Error!";
            }
            return "ok";
        }

        public string ReloadConfig(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ReloadConfig, args));
            }
            return "ok";
        }
        public string ShowArtTools(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowArtTools, args));
            }
            return "ok";
        }

        public string ShowTerrainTrace(params string[] args)
        {
            if (null != _handler)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowTerrainTrace, args));
            }
            return "ok";
        }

        public string SwitchAttachment(params string[] args)
        {
            var result = "ok";
            if (null != _handler)
            {
                result += _handler.OnDebugMessage(new DebugCommand(DebugCommands.ClearAttachment, args));
            }
            return result;
        }

        public string ClearAttachment(params string[] args)
        {
            var result = "ok";
            if (null != _handler)
            {
                result += _handler.OnDebugMessage(new DebugCommand(DebugCommands.ClearAttachment, args));
            }
            return result;
        }

        public string ShowAvaliablePartType(params string[] args)
        {
            var result = string.Empty;
            if (null != _handler)
            {
                result += _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowAvaliablePartType, args));
            }
            return result;
        }

        public string SetAttachment(params string[] args)
        {
            var result = "";
            if (null != _handler)
            {
                result += _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetAttachment, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! bulletCount should be int";
            }
            var infos = SingletonManager.Get<WeaponPartsConfigManager>().GetModifyInfos(i);
            if (null == infos)
            {
                return "illegal attachment id";
            }
            return result;
        }

        public string SetCurrentBulet(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetCurBullet, args));
            }
            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! bulletCount should be int";
            }

            return "OK";
        }

        public string SetReservedBullet(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetReservedBullet, args));
            }

            if (args.Length < 1)
            {
                return "Argument Error!";
            }
            int i;
            if (!int.TryParse(args[0], out i))
            {
                return "Argument Error! bulletCount should be int";
            }

            return "OK";
        }

        public string KillMe(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.KillMe));
            }

            return "OK";
        }

        public string ChangeHp(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ChangeHp, args));
            }

            return "OK";
        }


        public string DragCar(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.DragCar));
            }

            return "OK";
        }

        public string ShowVehicleDebugInfo(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowVehicleDebugInfo, args));
            }

            return "OK";
        }

        public string SetVehicleHp(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length < 2)
                {
                    return "Argument Error!";
                }
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetVehicleHp, args));
            }

            return "OK";
        }

        public string EnableVehicleCollisionDamage(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length != 1)
                {
                    return "Argument Error!";
                }
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableVehicleCollisionDamage, args));
            }

            return "OK";
        }

        public string EnableVehicleCollisionDebug(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length != 1)
                {
                    return "Argument Error!";
                }
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableVehicleCollisionDebug, args));
            }

            return "OK";
        }

        public string SetVehicleDynamicPrediction(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetVehicleDynamicPrediction, args));
            }

            return "OK";
        }

        public string ShowClientVehicle(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowClientVehicle, args));
            }

            return "OK";
        }

        public string ShowServerVehicle(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowServerVehicle, args));
            }

            return "OK";
        }

        public string ResetVehicle(params string[] args)
        {
            if (args.Length != 1)
            {
                return "Invalid Argument!";
            }

            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ResetVehicle, args));
            }

            return "OK";
        }

        public string EnableVehicleCull(params string[] args)
        {
            if (args.Length != 2)
            {
                return "Invalid Argument!";
            }

            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableVehicleCull, args));
            }

            return "ok";
        }

        public string TestFrame(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.TestFrame, args));
            }

            return "OK";
        }


        public string StartSampler(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.HeatMapRestart, args));
            }

            return "OK";
        }

        public string StopSampler(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.HeatMapPause, args));
            }

            return "OK";
        }

        public string SetVehicleFuel(params string[] args)
        {
            if (_handler != null)
            {

                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetVehicleFuel, args));
            }

            return "OK";
        }

        public string SetVehicleInput(params string[] args)
        {
            if (_handler != null)
            {
                if (args.Length < 2)
                {
                    return "Argument Error!";
                }

                _handler.OnDebugMessage(new DebugCommand(DebugCommands.SetVehicleInput, args));
            }

            return "OK";
        }

        public string ShowExplosionRange(params string[] args)
        {
            if (_handler != null)
            {
                string show = args.Length > 0 ? args[0].ToLower() : "show";
                string range = args.Length > 1 ? args[1] : "1";
                if (show == "hide")
                {
                    _handler.OnDebugMessage(new DebugCommand(DebugCommands.HideExplosionRange));
                    return "Hide";
                }
                else if (show == "show")
                {
                    _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowExplosionRange, new string[] { range }));
                    return "Show " + range;
                }
            }

            return "Invalid arguments list";
        }

        public string ListEntites(params string[] args)
        {
            if (_handler != null)
            {
                string show = args.Length > 0 ? args[0].ToLower() : "player";

                if (show == "player")
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var player in _contexts.player.GetEntities())
                    {
                        sb.Append(String.Format("player {0} hp {1}",
                            player.entityKey, player.gamePlay.CurHp));
                        if (player.isFlagSelf)
                            sb.Append(" [Self]");
                        sb.Append("\n");
                    }
                    return sb.ToString();
                }
                else if (show == "vehicle")
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var vehicle in _contexts.vehicle.GetEntities())
                    {
                        if (vehicle.hasEntityKey)
                        {
                            sb.Append(String.Format("vehicle {0}",
                                vehicle.entityKey));
                            sb.Append("\n");
                        }
                    }
                    return sb.ToString();
                }
            }

            return "Invalid arguments list";
        }

        private string Dump(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("dump start");

            string targetScene = null;
            if (args.Length > 0)
            {
                if (args[0] == "scenes")
                {
                    sb.AppendLine("active scene: " + SceneManager.GetActiveScene().name);
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        sb.AppendLine(SceneManager.GetSceneAt(i).name);
                    }
                }
                else
                {
                    targetScene = args[0];
                }
            }
            else
            {
                targetScene = SceneManager.GetActiveScene().name;
            }

            if (targetScene != null)
            {
                bool validSceneName = false;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (targetScene == scene.name)
                    {
                        sb.AppendLine("dump scene contents: " + targetScene);
                        foreach (var go in scene.GetRootGameObjects())
                        {
                            DumpGameObject(go, 1, sb);
                        }

                        validSceneName = true;
                        break;
                    }
                }

                if (!validSceneName)
                {
                    sb.AppendLine("scene doesn't exist: " + targetScene);
                }
            }


            logger.Debug(sb.ToString());

            return "See Log";
        }

        private void DumpGameObject(GameObject go, int depth, StringBuilder sb)
        {
            for (var i = 0; i < depth; i++)
                sb.Append("    ");
            sb.AppendFormat("{0}, components: ", go.name);

            foreach (var comp in go.GetComponents<Component>())
            {
                if (comp != null)
                {
                    if (comp is Behaviour)
                        sb.AppendFormat("{0}:{1} | ", comp.GetType(), ((Behaviour)comp).enabled ? "true" : "false");
                    else if (comp is Renderer)
                        sb.AppendFormat("{0}:{1} | ", comp.GetType(), ((Renderer)comp).enabled ? "true" : "false");
                    else if (comp is Collider)
                        sb.AppendFormat("{0}:{1} | ", comp.GetType(), ((Collider)comp).enabled ? "true" : "false");
                    else
                        sb.AppendFormat("{0} | ", comp.GetType());
                }
            }

            sb.AppendLine();

            foreach (Transform child in go.GetComponentInChildren<Transform>())
            {
                DumpGameObject(child.gameObject, depth + 1, sb);
            }
        }

        private string Culling(params string[] args)
        {
            if (args.Length < 1)
            {
                return "need parameter";
            }

            switch (args[0])
            {
                case "on":
                    SceneCulling.Enable = true;
                    break;
                case "off":
                    SceneCulling.Enable = false;
                    break;
                case "log":
                    SceneCulling.Log = true;
                    break;
                case "near":
                case "median":
                case "far":
                case "all":
                    SceneCulling.SetCullingDistance(args[0], float.Parse(args[1]));
                    break;
                case "camera":
                    SceneCulling.EnableCameraOC = args[1].Equals("on");
                    Camera.main.useOcclusionCulling = args[1].Equals("on");
                    break;
                default:
                    return "wrong input";
            }

            return "OK";
        }

        private string TerrainSetting(params string[] args)
        {
            if (args.Length < 1)
            {
                return "need parameter";
            }

            switch (args[0])
            {
                case "veg":
                    SingletonManager.Get<DynamicScenesController>().SetVegetationActive(args[1] == "on");
                    break;
                case "height":
                    SingletonManager.Get<DynamicScenesController>().SetHeightmapActive(args[1] == "on");
                    break;
                case "lightprobe":
                    int count = SceneManager.sceneCount;
                    for (int i = 0; i < count; i++)
                    {
                        var scene = SceneManager.GetSceneAt(i);
                        foreach (var go in scene.GetRootGameObjects())
                        {
                            TurnOffLightProbe(go);
                        }
                    }
                    break;
                default:
                    return "wrong input";
            }

            return "OK";
        }

        void TurnOffLightProbe(GameObject go)
        {
            var comp = go.GetComponent<MeshRenderer>();
            if (comp != null)
            {
                comp.lightProbeUsage = LightProbeUsage.Off;
            }
            else
            {
                int count = go.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    TurnOffLightProbe(go.transform.GetChild(i).gameObject);
                }
            }
        }

        private string QualitySetting(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("LodBias: {0}", QualitySettings.lodBias);

            return sb.ToString();
        }

        private string LodBias(params string[] args)
        {
            if (null != _handler)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.LodBias, args));
            }
            return "未工作，请联系程序";
        }

        private string TreeDistance(params string[] args)
        {
            if (null != _handler)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.TreeDistance, args));
            }
            return "未工作，请联系程序";
        }

        public string PrintUIList(params string[] args)
        {
            if (_handler != null)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.UIList));
            }
            return "OK";
        }
        public string HideUi(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.HideUI, args));
            }
            return "OK";
        }
        public string ShowUi(params string[] args)
        {
            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowUI, args));
            }
            return "OK";
        }

        public string EnableProfiler(params string[] args)
        {
            if (args.Length == 0)
            {
                return Profiler.enabled ? "Profiler Enabled" : "Profiler Disabled";
            }

            if (_handler != null)
            {
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableProfiler, args));
            }

            return "Null Command Handler";
        }

        public string HeatMapStop(params string[] args)
        {
            App.Client.Tools.TerrainSampler sampler = GetComponent<App.Client.Tools.TerrainSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get TerrainSampler Component";
            }

            bool autoUpload = false;
            if (args.Length == 1 && args[0] == "true") autoUpload = true;

            sampler.ForceStopSampler(autoUpload);

            return "OK";
        }

        public string HeatMapStopAndExit(params string[] args)
        {
            App.Client.Tools.TerrainSampler sampler = GetComponent<App.Client.Tools.TerrainSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get TerrainSampler Component";
            }

            bool autoUpload = false;
            if (args.Length == 1 && args[0] == "true") autoUpload = true;

            sampler.ForceStopAndExitSampler(autoUpload);

            return "OK";
        }

        public string HeatMapPoints(params string[] args)
        {
            App.Client.Tools.TerrainSampler sampler = GetComponent<App.Client.Tools.TerrainSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get TerrainSampler Component";
            }

            if (args.Length != 2)
            {
                return "FALSE, can't parse args";
            }

            string arg0 = args[0], arg1 = args[1];
            if (!arg0.StartsWith("[(") || !arg0.EndsWith(")]") || (!arg1.Equals("false") && !arg1.Equals("true")))
            {
                return "FALSE, can't parse args";
            }

            string[] ss = arg0.Split(',');
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWith("[("))
                {
                    ss[i] = ss[i].Substring(2);
                }
                else if (ss[i].StartsWith("("))
                {
                    ss[i] = ss[i].Substring(1);
                }
                else if (ss[i].EndsWith(")"))
                {
                    ss[i] = ss[i].Substring(0, ss[i].Length - 1);
                }
                else if (ss[i].EndsWith(")]"))
                {
                    ss[i] = ss[i].Substring(0, ss[i].Length - 2);
                }
            }

            List<float> list = new List<float>();
            foreach (string s in ss)
            {
                float f;
                if (!float.TryParse(s, out f))
                {
                    return "FALSE, can't parse args";
                }
                list.Add(f);
            }

            bool auto = arg1.Equals("false") ? false : true;

            bool success = sampler.SamplerSomePoints(list, auto);
            if (!success)
            {
                return "FALSE, sample fail";
            }

            return "OK";
        }

        public string HeatMapScenes(params string[] args)
        {
            App.Client.Tools.TerrainSampler sampler = GetComponent<App.Client.Tools.TerrainSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get TerrainSampler Component";
            }

            if (args.Length != 2)
            {
                return "FALSE, can't parse args";
            }

            string arg0 = args[0], arg1 = args[1];
            if (!arg0.StartsWith("[(") || !arg0.EndsWith(")]") || (!arg1.Equals("false") && !arg1.Equals("true")))
            {
                return "FALSE, can't parse args";
            }

            string[] ss = arg0.Split(',');
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWith("[("))
                {
                    ss[i] = ss[i].Substring(2);
                }
                else if (ss[i].StartsWith("("))
                {
                    ss[i] = ss[i].Substring(1);
                }
                else if (ss[i].EndsWith(")"))
                {
                    ss[i] = ss[i].Substring(0, ss[i].Length - 1);
                }
                else if (ss[i].EndsWith(")]"))
                {
                    ss[i] = ss[i].Substring(0, ss[i].Length - 2);
                }
            }

            List<int> list = new List<int>();
            foreach (string s in ss)
            {
                int i;
                if (!int.TryParse(s, out i))
                {
                    return "FALSE, can't parse args";
                }
                list.Add(i);
            }

            bool auto = arg1.Equals("false") ? false : true;

            bool success = sampler.SamplerSomeScenes(list, auto);
            if (!success)
            {
                return "FALSE, sample fail";
            }

            return "OK";
        }

        private string EnableRecordProfiler(string[] args)
        {
            if (args.Length == 0)
            {
                return SingletonManager.Get<MyProfilerManager>().IsRecordOn ? "Profiler Enabled" : "Profiler Disabled";
            }

            if (args.Length > 2)
            {
                return "Argument Error!";
            }

            if (_handler != null)
            {
                _handler.OnDebugMessage(new DebugCommand(DebugCommands.EnableRecordProfiler, args));
            }

            return "OK";
        }

        private string SetMaxQuality(string[] args)
        {
            if (args.Length == 1)
            {
                GameQualitySettingManager.SetMaxQualityFromControl(args[0]);
            }
            else
            {
                return "Argument Error!";
            }
            return "OK";
        }

        private string GetMaxQuality(string[] args)
        {
            if (args.Length == 0)
            {
                return GameQualitySettingManager.GetMaxQualityStatue();
            }
            else
            {
                return "Argument Error!";
            }
        }

        private string EnableMinRenderSet(string[] args)
        {
            App.Client.Tools.MinRendererSetSampler sampler = GetComponent<App.Client.Tools.MinRendererSetSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get MinRendererSetSampler Component";
            }

            return sampler.DisableInvisibleRenderers() ? "OK" : "FALSE";
        }

        private string DisableMinRenderSet(string[] args)
        {
            App.Client.Tools.MinRendererSetSampler sampler = GetComponent<App.Client.Tools.MinRendererSetSampler>();
            if (sampler == null)
            {
                return "FALSE, can't get MinRendererSetSampler Component";
            }

            sampler.RecoveryLastCollects();

            return "OK";
        }

        private string WaterReflectUseCam(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                return "FALSE, params are wrong";
            }

            int flag = -1;
            if (args[0].Equals("false")) flag = 0;
            else if (args[0].Equals("true")) flag = 1;
            if (flag == -1)
            {
                return "FALSE, can't parse args";
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type classType = null;
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                classType = assembly.GetType("AQUAS_Reflection");
                if (classType != null) break;
            }

            if (classType == null)
            {
                return "FALSE, can't get class 'AQUAS_Reflection'";
            }

            System.Reflection.MethodInfo method = classType.GetMethod("SetReflectionUsingCamera");
            if (method == null)
            {
                return "FALSE, can't get method 'SetReflectionUsingCamera'";
            }

            UnityEngine.Object obj = FindObjectOfType(classType);
            if (obj == null)
            {
                return "FALSE, can't get 'SetReflectionUsingCamera' component";
            }

            method.Invoke(obj, new object[] { flag == 1 });

            return "OK";
        }


        private string ForbidSystem(string[] args)
        {
            if (args.Length < 1)
                return "Argument Error";
            if(_handler!=null)
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.ForbidSystem, args));
            return "Nothing to be done";
        }

        private string PermitSystem(string[] args)
        {
            if (args.Length < 1)
                return "Argument Error";
            if(_handler!=null)
                 return _handler.OnDebugMessage(new DebugCommand(DebugCommands.PermitSystem, args));
            return "Nothing to be done";
        }

        private string ShowSystem(string[] args)
        {
            if(_handler!=null)
                return _handler.OnDebugMessage(new DebugCommand(DebugCommands.ShowSystem, args));
            return "Nothing to be done";
        }
    }
}