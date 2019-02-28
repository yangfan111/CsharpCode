using System.Collections.Generic;
using App.Shared.GameModules.Player;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.Utils;
using UnityEngine;
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
using Core.GameInputFilter;
using App.Shared.GameModules.Player.Oxygen;
using Core.Event;
using Core.Statistics;
using App.Shared.GameModules.Player.Actions;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.para;
using Core.BulletSimulation;
using Core;
using Utils.Singleton;
using Core.WeaponLogic.Throwing;
using App.Shared.GameModules.Weapon;
using Core;
using Core.CharacterState.Posture;
using Core.Configuration;
using App.Shared.GameModules.Weapon.Behavior;
using Assets.App.Shared.EntityFactory;
using Core.AnimatorClip;

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
            return CreateNewPlayerEntity(playerContext,commonSessionObjects.WeaponModeLogic,
                position, playerInfo, true, false);
        }

        public static PlayerEntity CreateNewPlayerEntity(
            PlayerContext playerContext,
            IWeaponMode modelLogic,
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
            playerEntity.AddLatestAdjustCmd(-1, -1);
            playerEntity.AddUserCmdOwner(new UserCmdOwnerAdapter(playerEntity));
            playerEntity.AddEntityKey(new EntityKey(playerInfo.EntityId, (int)EEntityType.Player));
            playerEntity.AddPosition(position);

            playerEntity.AddVehicleCmdSeq(0);
            playerEntity.isFlagSyncSelf = true;

            //On server-side, do not sync player entity to other players until the player login server successfully
            if (!SharedConfig.IsServer)
            {
                playerEntity.isFlagCompensation = true;
                playerEntity.isFlagSyncNonSelf = true;
            }
            playerEntity.isFlagAutoMove = autoMove;
            playerEntity.isFlagSelf = prediction;
            playerEntity.AddOrientation(0, 0, 0, 0, 0);
            playerEntity.AddPlayerRotateLimit(false);
            playerEntity.AddPlayerMove(Vector3.zero, 0, true, false, 0);
            playerEntity.AddPlayerSkyMove(false, -1);
            playerEntity.AddPlayerSkyMoveInterVar();
            playerEntity.AddTime(0);
            playerEntity.AddGamePlay(100, 100);

            //            playerEntity.AddWeaponState();



            AddCameraStateNew(playerEntity);
           
            playerEntity.AddState();

            playerEntity.AddFirePosition();
            playerEntity.AddStateBefore();
            playerEntity.AddStateInterVar(new StateInterCommands(), new StateInterCommands(), new UnityAnimationEventCommands(), new UnityAnimationEventCommands());
            playerEntity.AddStateInterVarBefore();
            playerEntity.AddMoveUpdate();
            playerEntity.AddSkyMoveUpdate();
            playerEntity.AddPlayerMoveByAnimUpdate();

            playerEntity.AddFirstPersonAppearance(playerEntity.entityKey.Value.EntityId);
            playerEntity.AddFirstPersonAppearanceUpdate();
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
            playerEntity.AddPlayerMask((byte)(EPlayerMask.TeamA | EPlayerMask.TeamB), (byte)(EPlayerMask.TeamA | EPlayerMask.TeamB));
            playerEntity.AddOverrideBag(0);
            playerEntity.AttachPlayerAux();
#if UNITY_EDITOR
            if (SharedConfig.IsOffline)
            {
                playerEntity.playerWeaponAuxiliary.BagOpenLimitTime = 50000;
            }
#endif
            
            playerEntity.AddTriggerEvent();

            playerEntity.AddRaycastTest(5f,new List<GameObject>());
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
            Logger.Info("PostCreateNewPlayerEntity");
            var sessionObjects = contexts.session.commonSession;

            var sceneObjectFactory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;

            player.AddModeLogic();
            player.modeLogic.ModeLogic = sessionObjects.WeaponModeLogic;

            var stateManager = new CharacterStateManager();

            if(!player.hasStatisticsData)
            {
                player.AddStatisticsData(false, new BattleData(), new StatisticsData());
            }
            
            if(!player.hasAutoMoveInterface)
                player.AddAutoMoveInterface(new GameModules.Player.Move.PlayerAutoMove(player));

            var speed = new SpeedManager(player, contexts, stateManager, stateManager, stateManager.GetIPostureInConfig(),
                stateManager.GetIMovementInConfig());
            stateManager.SetSpeedInterface(speed);
            player.AddStateInterface(stateManager);

            var oxygen = new OxygenEnergy(100, 0);
            player.AddOxygenEnergyInterface(oxygen);

            var genericAction = new GenericAction();
            player.AddGenericActionInterface(genericAction);

            var clipManager = new AnimatorClipManager();
            player.AddAnimatorClip(clipManager);

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
                player.AddStateInterVar(new StateInterCommands(), new StateInterCommands(), new UnityAnimationEventCommands(), new UnityAnimationEventCommands());
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
                player.AddPlayerSkyMove(false, -1);
            
            if (!player.hasPlayerSkyMoveInterVar)
                player.AddPlayerSkyMoveInterVar();

            if (!player.hasCharacterBone)
                player.AddCharacterBone(0, 0, 0, -1, true);

            if (!player.hasNetworkWeaponAnimation)
                player.AddNetworkWeaponAnimation(string.Empty, 0, string.Empty, 0);

            if (!player.hasOverrideNetworkAnimator)
            {
                player.AddOverrideNetworkAnimator();
            }
            
            AddCameraStateNew(player);
            player.AddLocalEvents(new PlayerEvents());
            InitFiltedInput(player, sessionObjects.GameStateProcessorFactory); 

            if (!player.hasPingStatistics)
            {
                player.AddPingStatistics();
            }

            if (!player.hasFreeData)
            {
                FreeData fd = new FreeData(contexts, player);
                if (player.hasStatisticsData)
                {   
                    fd.AddFields(new ObjectFields(player.statisticsData.Statistics));
                }
                player.AddFreeData(fd);
            }
            player.AddTip();
            AttachWeaponComponents(contexts,player);
            player.AddPlayerHitMaskController(new CommonHitMaskController(contexts.player, player));
            player.AddThrowingUpdate(false);
            player.AddThrowingAction();
            player.throwingAction.ActionInfo = new ThrowingActionInfo();
        
        }

        public static void AttachWeaponComponents(Contexts contexts, PlayerEntity playerEntity)
        {
            WeaponEntityFactory.EntityIdGenerator = contexts.session.commonSession.EntityIdGenerator;
            WeaponEntityFactory.WeaponContxt = contexts.weapon;
            var emptyScan = WeaponUtil.CreateScan(WeaponUtil.EmptyHandId);
         
            // playerEntity.RemoveWeaponComponents();
            var greandeIds = WeaponUtil.ForeachFilterGreandeIds();
     //       WeaponUtil.EmptyWeapon = WeaponEntityFactory.CreateEmpty(emptyScan);
            playerEntity.AttachPlayerWeaponBags();
            playerEntity.AttachPlayerAux();
            playerEntity.AttachPlayerCustomize();
            playerEntity.AttachGrenadeCacheData(greandeIds);
            playerEntity.AttachPlayerAmmu();
            playerEntity.playerWeaponAuxiliary.HasAutoAction = true;
            playerEntity.AttachWeaponComponentBehavior(contexts, greandeIds);
       //     var entityId = contexts.session.commonSession.EntityIdGenerator.GetNextEntityId();

            //       playerEntity.AddEmptyHand();
            //playerEntity.emptyHand.EntityId = emptyHandEntity.entityKey.Value.EntityId;
            //playerEntity.RefreshOrientComponent(null);

            //playerEntity.AddWeaponAutoState();
        }

        public static PlayerWeaponBagData[] MakeFakeWeaponBag()
        {
            return new PlayerWeaponBagData[]
            {
                new PlayerWeaponBagData
                {
                    BagIndex = 0,
                    weaponList = new List<PlayerWeaponData>
                    {
                        new PlayerWeaponData
                        {
                            Index = 1,
                            WeaponTplId = 1,
                            WeaponAvatarTplId = 0,
                            UpperRail = 20001,
                            Magazine = 9,
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
                            UpperRail = 20004,
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
                new FilteredInput(),
                new DummyFilteredInput());
            player.AddUserCmdFilter(gameInputProcessor);
        }

        public static void AddCameraStateNew(PlayerEntity playerEntity)
        {
            if (!playerEntity.hasCameraStateNew)
            {
                playerEntity.AddCameraStateNew();            
            }

            if (!playerEntity.hasCameraFinalOutputNew)
            {
                playerEntity.AddCameraFinalOutputNew(SingletonManager.Get<CameraConfigManager>().PostTransitionTime);
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

            if (!playerEntity.hasThirdPersonDataForObserving)
            {
                playerEntity.AddThirdPersonDataForObserving(new CameraStateOutputNewComponent(), new CameraFinalOutputNewComponent());
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