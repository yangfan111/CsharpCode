using System;
using Core;
using Core.Configuration;
using Core.GameModule.System;
using Core.Utils;
using UnityEngine;
using XmlConfig;
using Utils.Configuration;
using App.Shared.GameModules.Configuration;
using Utils.Appearance.Weapon;
using Assets.Utils.Configuration;
using App.Shared.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
using Utils.Appearance;
using System.Text;
using App.Shared.Util;
using Core.GameTime;
using Core.MyProfiler;
using Core.SessionState;
using Entitas;
//using UnityEditorInternal;
using Utils.SettingManager;
using Utils.Singleton;
using QualityLevel = Utils.SettingManager.QualityLevel;
using App.Shared.GameModules.Weapon;

namespace App.Shared.DebugHandle
{
    public class SharedCommandHandler
    {
        public static LoggerAdapter _logger = new LoggerAdapter(typeof(SharedCommandHandler));
        
        public static string ProcessGameSettingCommnands(DebugCommand message,SessionStateMachine stateMachine)
        {
            if (message.Command == DebugCommands.SetFrameRate)
            {
                string target = message.Args[0].ToLower();
                var frameRate = int.Parse(message.Args[1]);

                GameSettingUtility.SetFrameRate(target, frameRate);
                return "ok";

            }
            else if (message.Command == DebugCommands.GetQuality)
            {
                var qualityName = GameSettingUtility.GetQualityName();
                message.Args = new string[1];
                message.Args[0] = qualityName;
                return "ok";
            }
            else if (message.Command == DebugCommands.GetQualityList)
            {
                message.Args = GameSettingUtility.GetQualityNameList();
                return "ok";
            }
            else if (message.Command == DebugCommands.SetQuality)
            {
                int levelIndex = int.Parse(message.Args[0]);
                GameSettingUtility.SetQuality(levelIndex);
                return "ok";
            }
            else if (message.Command == DebugCommands.Quality)
            {
                int levelIndex = int.Parse(message.Args[0]);
                SettingManager.GetInstance().SetQuality((QualityLevel)levelIndex);
                return "ok";
            }
            else if (message.Command == DebugCommands.LodBias)
            {
                if (message.Args.Length > 0)
                {
                    float val;
                    if (float.TryParse(message.Args[0], out val))
                    {
                        QualitySettings.lodBias = val;
                    }
                    else
                    {
                        return "参数不合法，需要能转为float类型";
                    }
                }
                else
                {
                    return QualitySettings.lodBias.ToString();
                }

                return "ok";
            }
            else if (message.Command == DebugCommands.TreeDistance)
            {
                if(message.Args.Length > 1)
                {
                    float val; 
                    if (float.TryParse(message.Args[0], out val))
                    {
                        SingletonManager.Get<DynamicScenesController>().SetTreeDistance(val);
                    }
                    else
                    {
                        return "参数不合法，需要能转为float类型";
                    }
                    if(float.TryParse(message.Args[1], out val))
                    {
                        SingletonManager.Get<DynamicScenesController>().SetGrassDensity(val);
                    }
                    else
                    {
                        return "参数不合法，需要能转为float类型";
                    }
                }
                else if (message.Args.Length > 0)
                {
                    float val;
                    if (float.TryParse(message.Args[0], out val))
                    {
                        SingletonManager.Get<DynamicScenesController>().SetTreeDistance(val);
                    }
                    else
                    {
                        return "参数不合法，需要能转为float类型";
                    }
                }
                else
                {
                    return string.Format("tree {0}, grass {1}", SingletonManager.Get<DynamicScenesController>().GetTreeDistance(),
                        SingletonManager.Get<DynamicScenesController>().GetGrassDensity());
                }

                return "ok";
            }
            else if (message.Command == DebugCommands.PermitSystem)
            {
                if (stateMachine.PermitSystem(message.Args[0]))
                    return "ok";
                return "wrong path";
            }
            else if (message.Command == DebugCommands.ForbidSystem)
            {
                if(stateMachine.ForbidSystems(message.Args[0]))
                    return "ok";
                return "wrong path";
            }
            else if (message.Command == DebugCommands.ShowSystem)
            {
                var treeNode  = stateMachine.GetUpdateSystemTree();
                return TransSystemTreeToString(treeNode);
            }
            return String.Empty;
        }

        public static List<MapObjectEntity> ScanObjInMap(MapObjectEntity[] entities, Vector3 site, float radius)
        {
            var result = new List<MapObjectEntity>();
            foreach (var mapObjectEntity in entities)
            {
                if ((mapObjectEntity.position.Value - site).magnitude <= radius)
                {
                    result.Add(mapObjectEntity);
                }
            }
            return result;
        }

        public static string TransSystemTreeToString(SystemTreeNode treeNode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TransSystemTreeNodeToString(treeNode, 0));
            return sb.ToString();
        }
        
        public static string TransSystemTreeNodeToString(SystemTreeNode treeNode,int level)
        {
            StringBuilder sb = new StringBuilder();
            int times = level;
            while (times-- >= 0) sb.Append("\t");
            sb.Append(treeNode.state + "\t" + treeNode.systemName+"\n");
            foreach (var item in treeNode.subNode)
            {
                sb.Append(TransSystemTreeNodeToString(item, level + 1));
            }
            return sb.ToString();
        }
        
        public static void ProcessHitBoxCommands(DebugCommand message)
        {
            if (message.Command == DebugCommands.ShowDrawHitBoxOnBullet)
            {
                DebugConfig.DrawHitBoxOnBullet = true;
            }
            else if (message.Command == DebugCommands.HideDrawHitBoxOnBullet)
            {
                DebugConfig.DrawHitBoxOnBullet = false;
            }
            else if (message.Command == DebugCommands.ShowDrawHitBoxOnFrame)
            {
                DebugConfig.DrawHitBoxOnFrame = true;
            }
            else if (message.Command == DebugCommands.HideDrawHitBoxOnFrame)
            {
                DebugConfig.DrawHitBoxOnFrame = false;
            }
            else if (message.Command == DebugCommands.EnableDrawBullet)
            {
                DebugConfig.DrawBulletLine = true;
            }
            else if (message.Command == DebugCommands.DisableDrawBullet)
            {
                DebugConfig.DrawBulletLine = false;
            }
        }

        private static IWeaponModelController _weaponModelController;
        private static GameObject _twaRootGo;
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(SharedCommandHandler));
        
        public static string ProcessPlayerCommands(DebugCommand message, Contexts contexts, PlayerEntity player, ICommonSessionObjects sessionObjects, ICurrentTime currentTime)
        {
            var result = "";
            PlayerWeaponComponentsAgent bagLogicImp;
            switch (message.Command)
            {
                case DebugCommands.ClientMove:
                    var pos = new Vector3(0, 1000, 0);
                    var yaw = 0f;
                    switch (message.Args.Length)
                    {
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                    if (message.Args.Length > 0)
                    {
                        float x;
                        if (float.TryParse(message.Args[0], out x))
                        {
                            pos.x = x;
                        }
                    }
                    if (message.Args.Length == 2)
                    {
                        pos.y = 10000;
                        float z;
                        if (float.TryParse(message.Args[1], out z))
                        {
                            pos.z = z;
                        }
                    }
                    if (message.Args.Length == 3)
                    {
                        float y;
                        if (float.TryParse(message.Args[1], out y))
                        {
                            pos.y = y;
                        }
                        float z;
                        if (float.TryParse(message.Args[2], out z))
                        {
                            pos.z = z;
                        }
                    }
                    var ray = new Ray(pos, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        player.position.Value = hit.point;
                        result = "ok";
                    }
                    else
                    {
                        player.position.Value = pos;
                        result = "没有检测到碰撞，资源未加载完成或此位置没有碰撞，请指定y值";
                    }
                    
                    break;
                case DebugCommands.KillMe:
                    player.gamePlay.CurHp = 0;
                    player.gamePlay.ChangeLifeState(Components.Player.EPlayerLifeState.Dead, currentTime.CurrentTime);
                    break;
                case DebugCommands.ShowAniInfo:
                    result = string.Format("{0}\n{1}", player.state, player.thirdPersonAnimator.DebugJumpInfo());       
                    break;
                case DebugCommands.TestMap:
                    result =  BigMapDebug.HandleCommand(player,message.Args);
                    break;
                case DebugCommands.ChangeHp:
                    player.gamePlay.CurHp = int.Parse(message.Args[0]);
                    break;
                case DebugCommands.SetCurBullet:
                    player.WeaponController().HeldWeaponAgent.BaseComponent.Bullet= int.Parse(message.Args[0]);
                    break;
                case DebugCommands.SetReservedBullet:
                    if (message.Args.Length > 1)
                    {
                        int slot = int.Parse(message.Args[0]);
                        int count = int.Parse(message.Args[1]);

                        player.WeaponController().SetReservedBullet((EWeaponSlotType)slot, count);
                    }
                    else
                    {
                        int count = int.Parse(message.Args[0]);
                        player.WeaponController().SetReservedBullet( count);
                    }
                    break;
                case DebugCommands.SetGrenade:
                    IGrenadeCacheHelper helper = player.WeaponController().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
                    helper.AddCache(37);
                    helper.AddCache(37);
                    helper.AddCache(38);
                    helper.AddCache(39);
                    player.WeaponController().PickUpWeapon(WeaponUtil.CreateScan(37));
                    break;
                case DebugCommands.SetWeapon:
                    {
                        int weaponIdToSet = 0;
                        int avatarId = 0;
                        var weaponSlotToSet = 0;
                        if (message.Args.Length > 0)
                        {
                            weaponIdToSet = int.Parse(message.Args[0].Trim());
                        }
                        if (message.Args.Length > 2)
                        {

                            avatarId = int.Parse(message.Args[1].Trim());
                        }
                        if (message.Args.Length > 3)
                        {
                            weaponSlotToSet = int.Parse(message.Args[2].Trim());
                        }
                        var weaponInfo = WeaponUtil.CreateScan(weaponIdToSet,(val)=> val.AvatarId = avatarId > 0 ? avatarId : 0);
                        if (weaponSlotToSet != 0)
                        {
                            player.WeaponController().ReplaceWeaponToSlot((EWeaponSlotType)weaponSlotToSet, weaponInfo);
                        }
                        else
                        {
                            player.WeaponController().PickUpWeapon(weaponInfo);
                        }
                    }
                        break;
                case DebugCommands.DropWeapon:
                    var dropSlot = int.Parse(message.Args[0]);
                    player.WeaponController().DropWeapon((EWeaponSlotType)dropSlot);
                    break;
                case DebugCommands.TestWeaponAssemble:
                    if (null == _twaRootGo)
                    {
                        _twaRootGo = new GameObject("TwaRoot");
                        _twaRootGo.transform.parent = Camera.main.transform;
                        _twaRootGo.transform.localPosition = new Vector3(0, 0, 2);
                        _twaRootGo.transform.localEulerAngles = new Vector3(0, 90, 0);
                    }
                    if (null == _weaponModelController)
                    {
                        _weaponModelController = new WeaponModelController(
                        //SingletonManager.Get<WeaponResourceConfigManager>(),
                        //SingletonManager.Get<WeaponPartsConfigManager>(),
                        //SingletonManager.Get<WeaponAvatarConfigManager>(),
                        new WeaponModelLoadController<object>(
                            new WeaponModelLoader(sessionObjects.AssetManager),
                            new WeaponModelAssemblyController(_twaRootGo))
                        );
                    }
                    var operate = int.Parse(message.Args[0]);
                    var dataId = int.Parse(message.Args[1]);
                    switch (operate)
                    {
                        case 0:
                            _weaponModelController.SetWeapon(dataId);
                            break;
                        case 1:
                            _weaponModelController.SetPart(dataId);
                            break;
                        case 2:
                            _weaponModelController.RemovePart(dataId);
                            break;
                    }
                    break;
                case DebugCommands.SetAttachment:
                    var res = Core.Enums.EFuncResult.Failed;
                    var id = 0;
                    if (message.Args.Length == 2)
                    {
                        var slot = int.Parse(message.Args[0]);
                        id = int.Parse(message.Args[1]);
                      

                        res = player.WeaponController().SetWeaponPart((EWeaponSlotType)slot, id) ? Core.Enums.EFuncResult.Success : Core.Enums.EFuncResult.Failed;
                    }
                    else
                    {
                        id = int.Parse(message.Args[0]);
                        res = player.WeaponController().SetWeaponPart(id) ? Core.Enums.EFuncResult.Success : Core.Enums.EFuncResult.Failed;
                    }

                    switch (res)
                    {
                        case Core.Enums.EFuncResult.Exception:
                            result = "exception occurs";
                            break;
                        case Core.Enums.EFuncResult.Failed:
                            result = "attachment doesn't match";
                            break;
                        case Core.Enums.EFuncResult.Success:
                            result = "attach " + id + " to weapon";
                            break;
                    }
                    break;
                case DebugCommands.ClearAttachment:
                    var weaponSlot = (EWeaponSlotType)int.Parse(message.Args[0]);
                    var part = (EWeaponPartType)int.Parse(message.Args[1]);
                    player.WeaponController().DeleteWeaponPart(weaponSlot, part);
                    break;
                case DebugCommands.SwitchAttachment:
                    break;
                case DebugCommands.SetEquip:
                    player.appearanceInterface.Appearance.ChangeAvatar(int.Parse(message.Args[0]));
                    break;
                case DebugCommands.ShowAvaliablePartType:
                    int weaponId = player.WeaponController().HeldWeaponAgent.ConfigId;
                    if (weaponId > 0)
                    {
                        var list = SingletonManager.Get<WeaponPartsConfigManager>().GetAvaliablePartTypes(weaponId);
                        for (int i = 0; i < list.Count; i++)
                        {
                            result += list[i] + ",";
                        }
                    }
                    break;
                case DebugCommands.ReloadConfig:
                    ConfigReloadSystem.Reload = true;
                    break;

                case DebugCommands.ShowArtTools:
                    GameObject.Instantiate(Resources.Load<GameObject>("RuntimeTestTools"));
                    result = "RuntimeTestTools";
                    break;

                case DebugCommands.ShowTerrainTrace:
                    SharedConfig.IsShowTerrainTrace = !SharedConfig.IsShowTerrainTrace;
                    break;
            }
            return result;
        }

        
        
        public static string ProcessDebugCommand(DebugCommand message, Contexts context)
        {
            var result = string.Empty;
            switch (message.Command)
            {
                case DebugCommands.ListTriggerObj:
                    result = ListTriggerObj(context.mapObject, message.Args);
                    break;
                case DebugCommands.ListDoorEntity:
                    break;
                case DebugCommands.ShowConfig:
                    var config = message.Args[0] as string;
                    switch (config)
                    {
                        case "camera":
                            result = SingletonManager.Get<CameraConfigManager>().XMLContent;
                            break;
                    }
                    break;
                case DebugCommands.EnableRecordProfiler:
                {
                    var isClient = true;
                    var enabled = false;
                    if (message.Args.Length == 1)
                    {
                        enabled = int.Parse(message.Args[0]) != 0;
                    }
                    else if (message.Args.Length == 2)
                    {
                        isClient = message.Args[0].ToLower().Equals("c");
                        enabled = int.Parse(message.Args[1]) != 0;
                    }

                    if (isClient && !SharedConfig.IsServer ||
                        !isClient && SharedConfig.IsServer)
                    {
                        if (enabled)
                        {
                            SingletonManager.Get<MyProfilerManager>().EnableProfiler();
                        }
                        else
                        {
                            SingletonManager.Get<MyProfilerManager>().DisableProfiler();
                        }
                    }

                    break;
                }
                case DebugCommands.EnableProfiler:
                {
                    var cmdargs = CmdParser.ParseCommandLine(message.Args, "-");
                    var isClient = false;
                    var isServer = false;
                    var value = 0;
                    isClient = TryGetArgs<int>(cmdargs, "c", out value);
                    if(!isClient)
                        isServer = TryGetArgs<int>(cmdargs, "s", out value);
                    var enabled = value != 0;
                    
                    var isProfileGpu = TryGetArgs<int>(cmdargs, "g", out value);
                    var profileGpu = value != 0;
                    //profilegpu is only valid on client
                    profileGpu = !SharedConfig.IsServer && profileGpu;
                    
                    if (isClient && !SharedConfig.IsServer ||
                        isServer && SharedConfig.IsServer)
                    {
                        if (enabled)
                        {
                            UnityProfiler.EnableProfiler(profileGpu);
                            result = String.Format("ok, start {0} profiler profile gpu {1}", SharedConfig.IsServer ? "server" : "client", profileGpu ? "enabled" : "disabled") ;
                        }
                        else
                        {
                            UnityProfiler.DisableProfiler();
                            result = "ok, stop profiler";
                        }
                    }
                    else if (isProfileGpu)
                    {
                        UnityProfiler.EnableProfileGpu(profileGpu);
                        result = String.Format("ok, profile gpu {0}", (profileGpu ? "enabled" : "disabled"));
                    }
                    else if(!isServer)
                    {
                        result = "Invalid Argument.";
                    }

                    break;
                }

            }
            return result;
        }

        private static bool TryGetArgs<T>(Dictionary<string, string> cmdArgs, string argName, out T  value)
            where T : IConvertible
        {
            var name = argName.ToLower();
            if (cmdArgs.ContainsKey(name))
            {
                value = (T) Convert.ChangeType(cmdArgs[name], typeof(T));
                return true;
            }

            value = default(T);
            return false;
        }

        public static void ProcessMapObjectCommand(DebugCommand message, MapObjectContext context,
            IMapObjectEntityFactory mapObjectEntityFactory, PlayerEntity player)
        {
            switch (message.Command)
            {
                case DebugCommands.ClearSceneObject:
                    context.DestroyAllEntities();
                    break;
                case DebugCommands.ListDoorEntity:
                    var mapEntities = context.GetEntities();
                    for (int i = 0; i < mapEntities.Length; ++i)
                    {
                        var mapEntity = mapEntities[i];
                        if (mapEntity.hasDoorData && mapEntity.hasRawGameObject)
                        {
                            var obj = mapEntity.rawGameObject.Value;

                            string path = "/" + obj.name;
                            while (obj.transform.parent != null)
                            {
                                obj = obj.transform.parent.gameObject;
                                path = "/" + obj.name + path;
                            }

                            Logger.InfoFormat("DoorEntity {0} {1}", mapEntity, path);
                        }
                    }
                    break;
            }
        }
        
        public static void ProcessSceneObjectCommand(DebugCommand message, SceneObjectContext context,
            ISceneObjectEntityFactory sceneObjectEntityFactory, PlayerEntity player)
        {
            switch (message.Command)
            {
                case DebugCommands.CreateSceneObject:
                    int category;
                    int id;
                    if (int.TryParse(message.Args[0], out category) && int.TryParse(message.Args[1], out id))
                    {
                        sceneObjectEntityFactory.CreateSimpleEquipmentEntity((Assets.XmlConfig.ECategory) category, id,
                            1, player.position.Value);
                    }
                    break;
                case DebugCommands.ClearSceneObject:
                    context.DestroyAllEntities();
                    break;
            }
        }

        public static string ListTriggerObj(MapObjectContext context,  string[] args)
        {
            var position = new Vector3(
                float.Parse(args[0]),
                float.Parse(args[1]),
                float.Parse(args[2])         
            );
            
            StringBuilder sb = new StringBuilder();
                sb.Append("TriggerObjList:\n");
            
            var site = position;
            float radius = 25f;
            
            foreach (var MapObj in context.GetEntities())
            {
                if (MapObj == null) continue;
                if ((MapObj.position.Value - site).magnitude <= radius)
                {
                    if (!MapObj.hasRawGameObject || MapObj.rawGameObject.Value == null) 
                        continue;
						//sb.AppendFormat("EntityWithoutRaw:{0}, Pos:[{1}]\n", MapObj.GetType(), MapObj.position.Value);
                    sb.AppendFormat("Entity:{0}, Pos:[{1}]\n", MapObj.rawGameObject.Value.name, MapObj.position.Value);
                }
            }

            var result = sb.ToString();
            _logger.ErrorFormat("Time={0}, {1}", Time.time,  result);
            return result;
        }
        
        public static void ProcessVehicleCommand(DebugCommand message, VehicleContext vehicleContext, PlayerEntity player)
        {

            if (message.Command == DebugCommands.ShowExplosionRange)
            {
                VehicleDebugUtility.ShowExplosionRange(vehicleContext, player, true, float.Parse(message.Args[0]));
            }
            else if (message.Command == DebugCommands.HideExplosionRange)
            {
                VehicleDebugUtility.ShowExplosionRange(vehicleContext, player, false, 1.0f);
            }
            else if (message.Command == DebugCommands.DragCar)
            {
                VehicleDebugUtility.DragCar(vehicleContext, player);
            }
            else if (message.Command == DebugCommands.ShowVehicleDebugInfo)
            {
                VehicleDebugUtility.ToggleVehicleDebugInfo();
            }
            else if (message.Command == DebugCommands.SetVehicleHp)
            {
                VehicleDebugUtility.SetVehicleHp(vehicleContext, int.Parse(message.Args[0]), int.Parse(message.Args[1]));
            }
            else if (message.Command == DebugCommands.SetVehicleFuel)
            {
                VehicleDebugUtility.SetVehicleFuel(vehicleContext, int.Parse(message.Args[0]), int.Parse(message.Args[1]));
            }
            else if (message.Command == DebugCommands.SetVehicleInput)
            {
                VehicleDebugUtility.SetVehicleInput(vehicleContext, message.Args);
            }
            else if (message.Command == DebugCommands.EnableVehicleCollisionDamage)
            {
                VehicleDebugUtility.EnableVehicleCollisionDamage(int.Parse(message.Args[0]) != 0);
            }
            else if (message.Command == DebugCommands.EnableVehicleCollisionDebug)
            {
                VehicleDebugUtility.EnableVehicleCollisionDebug(int.Parse(message.Args[0]) != 0);
            }
            else if (message.Command == DebugCommands.SetVehicleDynamicPrediction)
            {
                VehicleDebugUtility.SetVehicleDynamicPrediction(vehicleContext, int.Parse(message.Args[0]) != 0);
            }
            else if (message.Command == DebugCommands.ShowClientVehicle)
            {
                VehicleDebugUtility.ShowVehicles(vehicleContext, false);
            }
            else if (message.Command == DebugCommands.ShowServerVehicle)
            {
                VehicleDebugUtility.ShowVehicles(vehicleContext, true);
            }
            else if (message.Command == DebugCommands.ResetVehicle)
            {
                VehicleDebugUtility.ResetVehicle(vehicleContext, int.Parse(message.Args[0]));
            }
            else if (message.Command == DebugCommands.TestFrame)
            {
                SharedConfig.runTestRame = string.Join(",", message.Args);
            }else if (message.Command == DebugCommands.EnableVehicleCull)
            {
                VehicleDebugUtility.EnableVehicleCull(int.Parse(message.Args[0]) != 0, int.Parse(message.Args[1]) != 0);
            }else if (message.Command == DebugCommands.SetVehicleActiveUpdateRate)
            {
                VehicleDebugUtility.SetVehicleActiveUpdateRate(int.Parse(message.Args[0]) != 0, int.Parse(message.Args[1]));
            }
        }

        public static string ProcessCommands(DebugCommand cmd, Contexts contexts, PlayerEntity player)
        {
            var type = Type.GetType(cmd.Command);
            var fieldInfos = new Queue<FieldInfo>();
            if(null != type)
            {
                var instance = Activator.CreateInstance(type);
                var i = 1;
                while(i < 100)
                {
                    var arg = type.GetField("arg" + i);
                    if(null == arg)
                    {
                        break;
                    }
                    fieldInfos.Enqueue(arg);
                    i++;
                }
                var abstractDebugCommand = instance as AbstractDebugCommand;
                return abstractDebugCommand.Process(contexts, player, cmd.Args, fieldInfos);
            }
            return "";
        }

        private static List<AbstractDebugCommand> _debugCommands = new List<AbstractDebugCommand>
        {
            new TexBias(),
            new AddGrenade(),
            new DumpDof(),
            new ChangeMask(),
            new FireInfo(),
        };

        public static List<AbstractDebugCommand> GetDebugCommands()
        {
            return _debugCommands;
        }
    }

    public interface IDebugCommand
    {
        string Process(Contexts contexts, PlayerEntity player, string[] args, Queue<FieldInfo> fieldInfos);
    }

    public abstract class AbstractDebugCommand : IDebugCommand
    {
        private Queue<string> _argQueue = new Queue<string>();
        private Queue<FieldInfo> _fieldInfos = new Queue<FieldInfo>();
        public string Process(Contexts contexts, PlayerEntity player, string[] args, Queue<FieldInfo> fieldInfos)
        {
            foreach (var arg in args)
            {
                _argQueue.Enqueue(arg);
            }
            for(; ; )
            {
                if (fieldInfos.Count < 1)
                {
                    break;
                }
                var fieldInfo = fieldInfos.Dequeue();
                switch(fieldInfo.FieldType.Name.ToString())
                {
                    case "Int32":
                        var i = GetInt();
                        fieldInfo.SetValue(this, i);
                        break;
                    case "Single":
                        var f = GetFloat();
                        Debug.LogFormat("set flaot {0}", f);
                        fieldInfo.SetValue(this, f);
                        break;
                    case "Vecotr3":
                        fieldInfo.SetValue(this, GetV3());
                        break;
                    case "String":
                        fieldInfo.SetValue(this, GetNext());
                        break;
                    default:
                        break;
                }
            }
            return OnProcess(contexts, player, args);
        }

        abstract protected string OnProcess(Contexts contexts, PlayerEntity player, string[] args);
        abstract public string Desc { get; }
        virtual public string Name {
            get
            {
                return GetType().Name;
            }
        }
        public string Usage
        {
            get
            {
                var str = GetType().ToString();
                foreach(var info in _fieldInfos)
                {
                    str += info.FieldType;
                }
                return str;
            }
        }

        private string GetNext()
        {
            if(_argQueue.Count > 0)
            {
                return _argQueue.Dequeue();                 
            }
            return string.Empty;
        }

        private bool HasNext()
        {
            return _argQueue.Count > 0;
        }

        protected int GetInt()
        {
            if(HasNext())
            {
                var arg = GetNext();
                int i;
                if(int.TryParse(arg, out i))
                {
                    return i;
                }
                return 0;
            }
            return 0;
        }

        protected Vector3 GetV3()
        {
            float x = 0, y = 0, z = 0;
            if(HasNext())
            {
                x = GetFloat();
            }
            if(HasNext())
            {
                y = GetFloat();
            }
            if(HasNext())
            {
                z = GetFloat();
            }
            return new Vector3(x, y, z);
        }

        protected float GetFloat()
        {
            if(HasNext())
            {
                float f;
                if (float.TryParse(GetNext(), out f))
                {
                    return f;
                }
                return 0;
            }
            return 0;
        }

        protected Vector2 GetV2()
        {
            float x = 0, y = 0;
            if(HasNext())
            {
                x = GetFloat(); 
            }

            if(HasNext())
            {
                y = GetFloat();
            }
            return new Vector2(x, y);
        }
    }

    internal class TexBias : AbstractDebugCommand
    {
        public int arg1;

        public override string Desc
        {
            get
            {
                return "修改贴图质量";
            }
        }

        protected override string OnProcess(Contexts contexts, PlayerEntity player, string[] args)
        {
            QualitySettings.masterTextureLimit = arg1;
            return "ok";
        }
    }

    internal class AddGrenade : AbstractDebugCommand
    {
        public int arg1;

        public override string Desc
        {
            get
            {
                return "添加手雷";
            }
        }

        protected override string OnProcess(Contexts contexts, PlayerEntity player, string[] args)
        {
            var helper = player.WeaponController().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
            helper.AddCache(arg1);
            return "ok";
        }
    }

    internal class DumpDof : AbstractDebugCommand
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DumpDof));
        public override string Desc
        {
            get
            {
                return "";
            }
        }

        protected override string OnProcess(Contexts contexts, PlayerEntity player, string[] args)
        {
            var weaponP1 = player.appearanceInterface.Appearance.GetWeaponP1InHand();
            if(null != weaponP1)
            {
                var upper = BoneMount.FindChildBoneFromCache(weaponP1, BoneName.ScopeLocator);
                var renderers = upper.GetComponentsInChildren<Renderer>();
                var sb = new StringBuilder(); 
                foreach(var renderer in renderers)
                {
                    var mat = renderer.material;
                    sb.AppendLine("name : " + renderer.name);
                    sb.AppendLine("mat : " + mat.name);
                    sb.AppendLine("renderQueue : " + mat.renderQueue);
                    if(mat.HasProperty("_Mode"))
                    {
                        sb.AppendLine("mode : " + mat.GetFloat("_Mode"));
                    }
                }
                Logger.Error(sb.ToString());
                return sb.ToString();
            }
            else
            {
                return "no weapon p1";
            }
        }
    }

    public class ChangeMask : AbstractDebugCommand
    {
        public override string Desc
        {
            get
            {
                return "修改玩家的Mask";
            }
        }
        public int arg1;
        public int arg2;

        protected override string OnProcess(Contexts contexts, PlayerEntity player, string[] args)
        {
            if(arg1 > 0)
            {
                player.playerMask.SelfMask = (int)arg1;
            }
            if(arg2 > 0)
            {
                player.playerMask.TargetMask = (byte)arg2;
            }
            return "ok";
        }
    }

    public class FireInfo : AbstractDebugCommand
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireInfo));
        public int arg1;
        public override string Desc
        {
            get
            {
                return "获取开火命中统计信息 -1：服务器信息 0：简略 1：伤害miss信息 2：miss信息 3：总开火信息（慎用）";
            }
        }

        protected override string OnProcess(Contexts contexts, PlayerEntity player, string[] args)
        {
            if(SharedConfig.IsServer)
            {
                if(arg1 < 0)
                {
                    return "";
                }
                var content = contexts.session.commonSession.BulletInfoCollector.GetStatisticData(arg1);
                if(player.hasNetwork)
                {
                    var msg = Protobuf.ServerDebugMessage.Allocate();
                    msg.Content = content;
                    player.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.DebugMessage, msg);
                    msg.ReleaseReference();
                }
                Logger.Info(content);
                return content; 
            }
            else
            {
                SharedConfig.ShowHitFeedBack = !SharedConfig.ShowHitFeedBack;
                return contexts.session.commonSession.BulletInfoCollector.GetStatisticData(arg1) + " ShowHItFeedBack is " + SharedConfig.ShowHitFeedBack;
            }
        }
    }
}



