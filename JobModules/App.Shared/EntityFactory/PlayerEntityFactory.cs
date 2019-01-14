using App.Shared.GameModules.Player;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.Utils;
using UnityEngine;
using App.Shared.WeaponLogic;
using Core.Room;
using App.Shared.Player;
using Utils.Configuration;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using Assets.App.Shared.GameModules.Player.Robot.Adapter;
using Assets.Utils.Configuration;
using App.Shared.GameInputFilter;
using Core.Animation;
using Utils.Utils;
using Core.Configuration.Sound;
using App.Shared.Terrains;
using App.Shared.Configuration;
using Core.GameInputFilter;
using App.Shared.GameModules.Player.Oxygen;
using Core.Event;
using Core.Statistics;
using App.Shared.GameModules.Player.Actions;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.player;
using com.wd.free.para;
using Core.BulletSimulation;
using App.Shared.WeaponLogic.HitCheck;
using Core.GameModeLogic;
using Utils.Singleton;
using App.Shared.Sound;

namespace App.Shared.EntityFactory
{
    public class PlayerCameraConstants
    {
        public const int CarTransitionTime = 1;
        public const int Layer = 1 << 10;
    }

   public static class PlayerEntityFactory
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityFactory));

        public static PlayerEntity CreateNewServerPlayerEntity(PlayerContext playerContext,
            ICommonSessionObjects commonSessionObjects,
            IEntityIdGenerator entityIdGenerator,  Vector3 position,
            IPlayerInfo playerInfo)
        {
            var entityId = entityIdGenerator.GetNextEntityId();
            playerInfo.EntityId = entityId;
            return CreateNewPlayerEntity(playerContext,  commonSessionObjects.WeaponModeLogic,  
                position, playerInfo, true, false);
        }


        public static PlayerEntity CreateNewPlayerEntity(
            PlayerContext playerContext,
            IWeaponSlotController weaponSlotController,
            Vector3 position,
            ICreatePlayerInfo playerInfo,
            bool prediction,
            bool autoMove)
        {
            PlayerEntity playerEntity = playerContext.CreateEntity();
            var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(playerInfo.RoleModelId).Sex;
            var modelName = sex == 1 ? SharedConfig.maleModelName : SharedConfig.femaleModelName;
           

            playerEntity.AddPlayerInfo(playerInfo.EntityId, playerInfo.PlayerId, playerInfo.PlayerName, playerInfo.RoleModelId,modelName,
                playerInfo.TeamId, playerInfo.Num, playerInfo.Level, playerInfo.BackId, playerInfo.TitleId, playerInfo.BadgeId, playerInfo.AvatarIds, playerInfo.WeaponAvatarIds,playerInfo.Camp);
          
            playerEntity.playerInfo.WeaponBags = playerInfo.WeaponBags;
            playerEntity.AddUserCmd();
            playerEntity.AddUserCmdSeq(0);
            playerEntity.AddUserCmdOwner(new UserCmdOwnerAdapter(playerEntity));
            playerEntity.AddEntityKey(new EntityKey(playerInfo.EntityId, (int)EEntityType.Player));
            playerEntity.AddPosition(position);

            playerEntity.AddVehicleCmdSeq(0);
            playerEntity.isFlagCompensation = true;
            playerEntity.isFlagSyncSelf = true;
            playerEntity.isFlagSyncNonSelf = true;
            playerEntity.isFlagAutoMove = autoMove;
            playerEntity.isFlagSelf = prediction;
            playerEntity.AddOrientation(0, 0, 0, 0, 0);
            playerEntity.AddPlayerRotateLimit(false);
            playerEntity.AddPlayerMove(Vector3.zero, 0, true, false, 0);
            playerEntity.AddPlayerSkyMove(true, -1);
            playerEntity.AddPlayerSkyMoveInterVar();
            playerEntity.AddTime(0);
            playerEntity.AddGamePlay(100, 100);

            playerEntity.AddWeaponState(0, 0);

#if UNITY_EDITOR
            if (SharedConfig.IsOffline)
            {
                playerEntity.weaponState.BagOpenLimitTime = 50000;
            }
#endif

            playerEntity.AddPlayerWeaponState(0, false, 0, 0, 0, 0, false, 0, 0);
            AddCameraStateNew(playerEntity);
            playerEntity.AddState();

            playerEntity.AddFirePosition();
            playerEntity.AddStateBefore();
            playerEntity.AddStateInterVar(new StateInterCommands(), new StateInterCommands(), new StateInterCommands(), new StateInterCommands());
            playerEntity.AddStateInterVarBefore();
            playerEntity.AddMoveUpdate();
            playerEntity.AddSkyMoveUpdate();
            playerEntity.AddPlayerMoveByAnimUpdate();

            playerEntity.AddFirstPersonAppearance(playerEntity.entityKey.Value.EntityId);
            playerEntity.AddThirdPersonAppearance();
            //playerEntity.AddNetworkWeaponAnimation(string.Empty, 0, string.Empty, 0);

            playerEntity.AddLatestAppearance();
            playerEntity.latestAppearance.Init();
            playerEntity.AddPredictedAppearance();

            playerEntity.AddOxygenEnergy(0);

            playerEntity.AddSound();
            playerEntity.AddUpdateMessagePool();
            playerEntity.AddRemoteEvents(new PlayerEvents());

            playerEntity.AddStatisticsData(false, new BattleData(), new StatisticsData());
            weaponSlotController.InitPlayerWeaponBySlotInfo(playerEntity);
            playerEntity.AddPlayerMask((byte)(EPlayerMask.TeamA | EPlayerMask.TeamB), (byte)(EPlayerMask.TeamA | EPlayerMask.TeamB));
            playerEntity.AddOverrideBag(0);
            //Logger.Info(playerEntity.Dump());
            return playerEntity;
        }

        public static void CreateRobotPlayerEntity(Contexts contexts, PlayerEntity player, IRobotConfig robotConfig,
            IRobotUserCmdProvider robotUserCmdProvider, IUserCmdGenerator userCmdGenerator)
        {
            var navMeshAgent = player.RootGo().AddComponent<NavMeshAgent>();
            var behaviorTree = player.RootGo().AddComponent<BehaviorTree>();
            navMeshAgent.autoTraverseOffMeshLink = false;
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.stoppingDistance = 0.1f;
            behaviorTree.enabled = false;

            var robot = new PlayerRobotAdapter(contexts, player, navMeshAgent, robotUserCmdProvider,
                new DummyRobotSpeedInfo(),
                userCmdGenerator, new DummyRobotConfig());

            player.AddRobot(robot);
        }

        /// <summary>
        /// ����Ҫ��Դ���صĳ�ʼ��
        /// </summary>
        /// <param name="player"></param>
        /// <param name="vehicleContext"></param>
        public static void PostCreateNewPlayerEntity(
            PlayerEntity player,
            Contexts contexts)
        {
            var sessionObjects = contexts.session.commonSession;
            
            var sceneObjectFactory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;

            player.AddModeLogic();
            player.modeLogic.ModeLogic = sessionObjects.WeaponModeLogic;

            var stateManager = new CharacterStateManager();
            var playerWeaponStateAdapter = new PlayerWeaponStateAdapter(player, stateManager, stateManager, contexts.ui);

            if(!player.hasStatisticsData)
            {
                player.AddStatisticsData(false, new BattleData(), new StatisticsData());
            }
            player.AddWeaponFactory(new WeaponLogic.WeaponFactory(playerWeaponStateAdapter, stateManager,
                contexts.session.entityFactoryObject.WeaponLogicFactory,
                contexts.session.commonSession.FreeArgs));

            var speed = new SpeedManager(player, stateManager, stateManager, stateManager.GetIPostureInConfig(),
                stateManager.GetIMovementInConfig(), playerWeaponStateAdapter);
            stateManager.SetSpeedInterface(speed);
            player.AddStateInterface(stateManager);

            var oxygen = new OxygenEnergy(100, 0);
            player.AddOxygenEnergyInterface(oxygen);

            var genericAction = new GenericAction();
            player.AddGenericActionInterface(genericAction);

            var clipManager = new AnimatorClipManager();
            player.AddAnimatorClip(clipManager);

            player.RefreshPlayerWeaponLogic(-1);

            if (!player.hasPlayerRotateLimit)
            {
                player.AddPlayerRotateLimit(false);
            }

            if (!player.hasFirePosition)
            {
                player.AddFirePosition();
            }
            
            if (!player.hasState)
                player.AddState();
            if (!player.hasStateInterVar)
                player.AddStateInterVar(new StateInterCommands(), new StateInterCommands(), new StateInterCommands(), new StateInterCommands());
            if (!player.hasStateBefore)
                player.AddStateBefore();
            if (!player.hasStateInterVarBefore)
                player.AddStateInterVarBefore();
            ComponentSynchronizer.SyncToStateComponent(player.state, player.stateInterface.State);

            if (!player.hasVehicleCmdSeq)
                player.AddVehicleCmdSeq(0);
            if (!player.hasUserCmd)
                player.AddUserCmd();
          
            if (!player.hasControlledVehicle)
                player.AddControlledVehicle();
            
            if (!player.hasPlayerSkyMove)
                player.AddPlayerSkyMove(true,-1);
            
            if (!player.hasPlayerSkyMoveInterVar)
                player.AddPlayerSkyMoveInterVar();

            if (!player.hasCharacterBone)
                player.AddCharacterBone(0);

            if (!player.hasNetworkWeaponAnimation)
                player.AddNetworkWeaponAnimation(string.Empty, 0, string.Empty, 0);

            AddCameraStateNew(player);
            var bagLogic = new WeaponBagLogic(
                player,
                SingletonManager.Get<WeaponConfigManager>());

            player.AddBag(bagLogic);
            var grenadeInventory = new GrenadeBagCacheAgent(player.grenadeInventoryData);
            player.AddGrenadeInventoryHolder(grenadeInventory);
            player.AddLocalEvents(new PlayerEvents());
            InitFiltedInput(player, sessionObjects.GameStateProcessorFactory); 
/*
            player.AddSoundManager(new PlayerSoundManager(player,
                soundContext, 
                PlayerSoundConfigManager.Instance, 
                SoundConfigManager.Instance, 
                sessionObjects.SoundEntityFactory,
                SingletonManager.Get<TerrainManager>(),
                SingletonManager.Get<MapConfigManager>()));
*/
            player.AddSoundManager(new DummyPlayerSoundManager());
            player.AddPlayerAction(new PlayerWeaponActionLogic(player,
                bagLogic,
                sceneObjectFactory,
                player.modeLogic.ModeLogic,
                grenadeInventory,
                player.modeLogic.ModeLogic,
                player.modeLogic.ModeLogic));
            
            if (!player.hasPingStatistics)
            {
                player.AddPingStatistics();
            }

            if (!player.hasFreeData)
            {
                FreeData fd = new FreeData(player);
                if (player.hasStatisticsData)
                {   
                    fd.AddFields(new ObjectFields(player.statisticsData.Statistics));
                }
                player.AddFreeData(fd);
            }
            player.AddPlayerHitMaskController(new CommonHitMaskController(contexts.player, player));
            player.AddTip();
            player.AddThrowingUpdate(false);
        }

        public static PlayerWeaponBagData[] MakeFakeWeaponBag()
        {
            return new PlayerWeaponBagData[]
            {
                new PlayerWeaponBagData
                {
                    BagIndex = 0, 
                    weaponList = new System.Collections.Generic.List<PlayerWeaponData>
                    {
                        new PlayerWeaponData
                        {
                            Index = 1,
                            WeaponTplId = 1,
                            WeaponAvatarTplId = 0,
                        },
                        new PlayerWeaponData
                        {
                            Index = 2,
                            WeaponTplId = 24,
                            WeaponAvatarTplId = 0,
                        },
                        new PlayerWeaponData
                        {
                            Index = 3,
                            WeaponTplId = 31,
                            WeaponAvatarTplId = 0,
                        },
                        new PlayerWeaponData
                        {
                            Index = 4,
                            WeaponTplId = 37,
                            WeaponAvatarTplId = 0,
                        },
                        new PlayerWeaponData
                        {
                            Index = 5,
                            WeaponTplId = 38,
                            WeaponAvatarTplId = 0,
                        },
                        new PlayerWeaponData
                        {
                            Index = 6,
                            WeaponTplId = 39,
                            WeaponAvatarTplId = 0,
                        }

                    },
                },
                new PlayerWeaponBagData
                {
                    BagIndex = 1,
                    weaponList = new System.Collections.Generic.List<PlayerWeaponData>
                    {
                        new PlayerWeaponData
                        {
                            Index = 1,
                            WeaponTplId = 4,
                        },
                        new PlayerWeaponData
                        {
                            Index = 2,
                            WeaponTplId = 26,
                        },
                        new PlayerWeaponData
                        {
                            Index = 3,
                            WeaponTplId = 32,
                        },
                        new PlayerWeaponData
                        {
                            Index = 4,
                            WeaponTplId = 38,
                        }
                    }
                }
            };
        }

      

        public static void InitFiltedInput(PlayerEntity player, IGameStateProcessorFactory gameStateProcessorFactory)
        {
            var stateProviderPool = gameStateProcessorFactory.GetProviderPool() as StateProviderPool;
            var statePool = gameStateProcessorFactory.GetStatePool();
            if(null != stateProviderPool)
            {
                stateProviderPool.AddStateProvider(player, statePool); 
            }
            else
            {
                Logger.Error("state provider pool is null !!");
            }
            var gameInputProcessor = new GameInputProcessor(new UserCommandMapper(), 
                new StateProvider(new PlayerStateAdapter(player), statePool),  
                new FilteredInput());
            player.AddUserCmdFilter(gameInputProcessor);
        }

        public static void RefreshPlayerWeaponLogic(this PlayerEntity player, int weaponId)
        {
            bool noWeapon = false;
            if (weaponId == UniversalConsts.InvalidIntId || weaponId == 0)
            {
                weaponId = SingletonManager.Get<WeaponAvatarConfigManager>().GetEmptyHandId();
                noWeapon = true;
            }

            var cfg = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(weaponId);
            if (null == cfg)
            {
                Logger.ErrorFormat("no weapon config with id {0} exist ", weaponId);
                return;
            }
            if(!player.hasWeaponFactory)
            {
                return;
            }
            var factory = player.weaponFactory.Factory;
            factory.Prepare(weaponId);
            if (!player.hasWeaponLogic)
            {
                player.AddWeaponLogic(factory.GetPlayerWeaponState());
            }
            if(!player.hasWeaponLogicInfo)
            {
                player.AddWeaponLogicInfo();
            }
            player.weaponLogic.Weapon = factory.GetWeaponLogic();
            player.weaponLogic.WeaponSound = factory.GetWeaponSoundLogic();
            player.weaponLogic.WeaponEffect = factory.GetWeaponEffectLogic();
            player.weaponLogicInfo.WeaponId = weaponId;
            player.weaponLogicInfo.LastWeaponId = weaponId;
#if UNITY_EDITOR
            RefreshVisualWeaponConfig(player);
#endif
            // 更新枪械时，后坐力重置 
            player.orientation.PunchPitch = 0;
            player.orientation.PunchYaw = 0;
            player.orientation.WeaponPunchPitch = 0;
            player.orientation.WeaponPunchYaw = 0;

            if (noWeapon)
            {
                player.playerMove.DefaultSpeed = player.weaponLogic.Weapon.GetBaseSpeed();
            }

            if (null != player.weaponLogic.Weapon)
            {
                player.weaponLogic.Weapon.EmptyHand = noWeapon;
            }
            else
            {
                Logger.Error("weapon of player weapon logic is null !!");
            }
        }

        private static void RefreshVisualWeaponConfig(PlayerEntity player)
        {
            if(!player.hasWeaponConfig)
            {
                player.AddWeaponConfig();
                player.weaponConfig.LogicConfig = new WeaponConfigNs.VisualConfigGroup();
            }
            player.weaponLogic.Weapon.SetVisualConfig(ref player.weaponConfig.LogicConfig);
        }


        public static void AddCameraStateNew(PlayerEntity playerEntity)
        {
            if (!playerEntity.hasCameraStateNew)
            {
                playerEntity.AddCameraStateNew();
               
            }

            if (!playerEntity.hasCameraFinalOutputNew)
            {
               
                playerEntity.AddCameraFinalOutputNew();
            }

            if (!playerEntity.hasCameraStateOutputNew)
            {
                playerEntity.AddCameraStateOutputNew();
            }

            if (!playerEntity.hasCameraConfigNow)
            {
                playerEntity.AddCameraConfigNow();
            }

            if (!playerEntity.hasCameraArchor)
            {
                playerEntity.AddCameraArchor();
            }

            if (!playerEntity.hasCameraStateUpload)
            {
                playerEntity.AddCameraStateUpload();
            }
            
        }

   

        public static void AddStateComponent(PlayerEntity playerEntity)
        {
            var index = PlayerComponentsLookup.State;
            var component = playerEntity.CreateComponent<StateComponent>(index);
            playerEntity.AddComponent(index, component);
        }
    }
}