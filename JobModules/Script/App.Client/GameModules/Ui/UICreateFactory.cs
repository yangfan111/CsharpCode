using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Models.Biochemical;
using App.Client.GameModules.Ui.Models.Blast;
using App.Client.GameModules.Ui.Models.Chicken;
using App.Client.GameModules.Ui.Models.Common;
using App.Client.GameModules.Ui.Models.Common.Map;
using App.Client.GameModules.Ui.Models.Group;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Common;
using App.Shared.Components;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Common;
using Assets.UiFramework.Libs;
using Core.Ui;
using Core.Utils;
using UIComponent.UI;
using CommonMaxMap = App.Client.GameModules.Ui.Models.Common.CommonMaxMap;

namespace Assets.App.Client.GameModules.Ui
{
    public class UiCreateFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UiCreateFactory));

        private static UIConfig UIConfig = new UIConfig();
        private static int currentGameRule;
        private static Contexts contexts;

        private static Action<AbstractModel> _onModelInitialized;

        public static void Initialize(Contexts contexts, Action<AbstractModel> onModelInitialized)
        {
            UiCreateFactory.contexts = contexts;
            _onModelInitialized = onModelInitialized;
        }

        public static int AddCreateUI(int gameRule)
        {
            CreateCommonUI();
            //CreateTeamUI(gameRule);
            //CreateSurvivalUI(gameRule);
            //CreateBlastUI(gameRule);
            //CreateAnnihilationUI(gameRule);

            string preui = contexts.session.commonSession.RoomInfo.PreLoadUI;
            if(preui != null)
            {
                _logger.DebugFormat("PreLoadUI:{0}", preui);
                string[] uis = preui.Split(',');
                foreach (string ui in uis)
                {
                    if (!string.IsNullOrEmpty(ui))
                    {
                        contexts.ui.uISession.CreateUi.Add(ui);
                    }
                }
            }

            _logger.Info("已预加载资源:" + preui);

            ReplaceWeaponHud(gameRule);//临时处理需要6号槽的武器Hud

            return contexts.ui.uISession.CreateUi.Count;
        }

		private static void ReplaceWeaponHud(int gameRule)
        {
            if (GameRules.Bomb != gameRule) return;
            var list = contexts.ui.uISession.CreateUi;
            list.RemoveAll((it) => it == UiNameConstant.CommonWeaponHudModel || it == UiNameConstant.BlastWeaponHudModel);
            list.Add(UiNameConstant.BlastWeaponHudModel);
        }

        public static void CreateUi(Action<AbstractModel> onModelLoaded)
        {
            if (contexts.ui.uISession.CreateUi == null || contexts.ui.uISession.CreateUi.Count == 0) return;

            foreach (var name in contexts.ui.uISession.CreateUi)
            {
                UiCreateFactory.CreateUiByName(name, onModelLoaded);
            }
            contexts.ui.uISession.CreateUi.Clear();
        }

        public static void CreateUiByName(string name, Action<AbstractModel> onModelLoaded)
        {
           // _logger.DebugFormat("UI Create Name: {0}", name);
            var uiData = UIConfig.GetUIConfig(name);
            if (uiData == null) return;
            IUiAdapter uiAdapter;
            if (uiData.Params == null)
            {
                uiAdapter = Activator.CreateInstance(uiData.Adapter) as IUiAdapter;
            }
            else
            {
                uiAdapter = Activator.CreateInstance(uiData.Adapter, uiData.Params) as IUiAdapter;
            }

            uiAdapter.UiSessionComponent = contexts.ui.uISession;
            uiAdapter.UserInputManager = contexts.userInput.userInputManager;
            var model = Activator.CreateInstance(uiData.Model, uiAdapter) as AbstractModel;
            model.Initialize(onModelLoaded);
            model.SetVisible(true);
            model.UIRoot(UiCommon.UIManager.GetLayer(uiData.Layer));

            _onModelInitialized(model);

            contexts.ui.uISession.UiAdapters[name] = uiAdapter;
            var groupController = model as IUiGroupController;
            if (groupController == null)
            {
                return;
            }
            foreach (var group in uiData.Groups)
            {
                if (contexts.ui.uISession.UiGroup.ContainsKey(group) == false)
                    contexts.ui.uISession.UiGroup[group] = new List<IUiGroupController>();
                contexts.ui.uISession.UiGroup[group].Add(groupController);
            }
        }

        public static void RegisterAllUi(Contexts contexts)
        {
            //Fix
            RegisterUi(UiNameConstant.CommonScreenFlashModel, typeof(CommonScreenFlashModel), typeof(ScreenFlashUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonFocusMaskModel, typeof(CommonFocusMaskModel), typeof(FocusMaskUIAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonPlayerInfo, typeof(CommonPlayerInfo), typeof(PlayerInfoUIAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonMenuModel, typeof(CommonMenuModel), typeof(MenuUiAdapter), UILayer.Alert, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix,UiGroup.Singleton });
            RegisterUi(UiNameConstant.CommonVideoSettingModel, typeof(CommonVideoSettingModel), typeof(VideoSettingUiAdapter), UILayer.Alert, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonMCountDownModel, typeof(CommonMCountDownModel), typeof(CountDownUiAdapter), UILayer.Foreground, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonRevengeTagModel, typeof(CommonRevengeTagModel), typeof(RevengeTagUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonDeadModel, typeof(CommonDeadModel), typeof(DeadUiAdapter), UILayer.Decorate, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonDebugInfoModel, typeof(CommonDebugInfoModel), typeof(DebugInfoUiAdapter), UILayer.Alert, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });

            RegisterUi(UiNameConstant.CommonHealthGroup, typeof(CommonHealthGroup), typeof(PlayerStateUiAdapter), UILayer.Base, null, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide });
            RegisterUi(UiNameConstant.CommonPickUpModel, typeof(CommonPickUpModel), typeof(PickUpUiAdapter), UILayer.Base, new object[] { contexts.player, contexts.sceneObject, contexts.mapObject, contexts.session, contexts.vehicle, contexts.freeMove, contexts.userInput.userInputManager.Instance, contexts.ui }, 
                new UiGroup[] { UiGroup.Base});
            RegisterUi(UiNameConstant.CommonMiniMap, typeof(CommonMiniMap), typeof(MiniMapUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonCrossHairModel, typeof(CommonCrossHairModel), typeof(CrossHairUiAdapter), UILayer.Foreground, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide , UiGroup.TimeCountDownHide});
            RegisterUi(UiNameConstant.CommonHurtedModel, typeof(CommonHurtedModel), typeof(HurtedUiAdapter), UILayer.Foreground, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix});
            RegisterUi(UiNameConstant.CommonKillFeedbackModel, typeof(CommonKillFeedbackModel), typeof(KillFeedBackUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base , UiGroup.MapHide });
            RegisterUi(UiNameConstant.CommonKillMessageModel, typeof(CommonKillMessageModel), typeof(KillMessageUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.CommonLoadingModel, typeof(CommonLoadingModel), typeof(LoadingUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.CommonLocationModel, typeof(CommonLocationModel), typeof(LocationUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base , UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonLocateModel, typeof(CommonLocateModel), typeof(LocationUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonParachuteModel, typeof(CommonParachuteModel), typeof(ParachuteStateUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base , UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonRangingModel, typeof(CommonRangingModel), typeof(RangingUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.CommonTechStatModel, typeof(CommonTechStatModel), typeof(TechStatUiAdapter), UILayer.Tip, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Alert });
            RegisterUi(UiNameConstant.CommonWeaponHudModel, typeof(CommonWeaponHudModel), typeof(WeaponStateUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonVehicleTipModel, typeof(CommonVehicleTipModel), typeof(VehicleTipUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base ,UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonChatModel, typeof(CommonChatModel), typeof(ChatUiAdapter), UILayer.Chat, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup .SurvivalBagHide});
            RegisterUi(UiNameConstant.CommonCountdownTipModel, typeof(CommonCountdownTipModel), typeof(CountdownTipUiAdapter), UILayer.Tip, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Alert });
            RegisterUi(UiNameConstant.CommonTaskTipModel, typeof(CommonTaskTipModel), typeof(TaskTipUiAdapter), UILayer.Pop, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.CommonOperationTipModel, typeof(CommonOperationTipModel), typeof(OperationTipUiAdapter), UILayer.Tip, new object[] { contexts },
                new UiGroup[] { UiGroup.Alert });
            RegisterUi(UiNameConstant.CommonSystemTipModel, typeof(CommonSystemTipModel), typeof(SystemTipUiAdapter), UILayer.Tip, new object[] { contexts },
                new UiGroup[] { UiGroup.Base });
            //pop
//            RegisterUi(UiNameConstant.CommonMaxMap, typeof(CommonMaxMap), typeof(MiniMapUiAdapter), UILayer.Pop, new object[] { contexts },
//                new UiGroup[] { UiGroup.Pop, UiGroup.Singleton });
            RegisterUi(UiNameConstant.CommonMap, typeof(CommonMap), typeof(MiniMapUiAdapter), UILayer.Pop, new object[] { contexts },
                new UiGroup[] { UiGroup.Pop, UiGroup.Singleton });
            RegisterUi(UiNameConstant.CommonNoticeModel, typeof(CommonNoticeModel), typeof(NoticeUiAdapter), UILayer.Alert, new object[] { contexts },
                new UiGroup[] { UiGroup.Alert,UiGroup.GameOverHide });
            RegisterUi(UiNameConstant.CommonPaintDiscModel, typeof(CommonPaintDiscModel), typeof(PaintUiAdapter), UILayer.Pop, new object[] { contexts },
                new UiGroup[] { UiGroup.Pop,UiGroup.Singleton, UiGroup.SurvivalBagHide });

            //team
            RegisterUi(UiNameConstant.GroupScoreModel, typeof(GroupScoreModel), typeof(GroupScoreUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.GroupRecordModel, typeof(GroupRecordModel), typeof(GroupRecordUiAdapter), UILayer.Pop, new object[] { contexts }, new UiGroup[] { UiGroup.Pop, UiGroup.Singleton });
            RegisterUi(UiNameConstant.CommonWeaponBagTipModel, typeof(CommonWeaponBagTipModel), typeof(WeaponBagTipUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonWeaponBagModel, typeof(CommonWeaponBagModel), typeof(WeaponBagUiAdapter), UILayer.Pop, new object[] { contexts }, new UiGroup[] { UiGroup.Pop, UiGroup.Singleton });
            RegisterUi(UiNameConstant.CommonGameOverModel, typeof(CommonGameOverModel), typeof(GameOverUiAdapter), UILayer.Tip, new object[] { contexts }, new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonGameTitleModel, typeof(CommonGameTitleModel), typeof(GameTitleUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });

            RegisterUi(UiNameConstant.ChickenBagModel, typeof(ChickenBagModel), typeof(ChickenBagUiAdapter), UILayer.Pop, new object[] { contexts },
                new UiGroup[] { UiGroup.Base, UiGroup.Singleton});
            RegisterUi(UiNameConstant.ChickenScoreModel, typeof(ChickenScoreModel), typeof(ChickenScoreUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonTeam, typeof(CommonTeam), typeof(TeamUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base , UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonSplitModel, typeof(CommonSplitModel), typeof(SplitUiAdapter), UILayer.Alert, new object[] { contexts }, new UiGroup[] { UiGroup.Fix });
            RegisterUi(UiNameConstant.CommonSuoDuModel, typeof(CommonSuoDuModel), typeof(SuoDuUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.ChickenPlaneModel, typeof(ChickenPlaneModel), typeof(PlaneUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });

            RegisterUi(UiNameConstant.BlastScoreModel, typeof(BlastScoreModel), typeof(BlastScoreUiAdapter), UILayer.Base, new object[] { contexts }, 
                new UiGroup[] { UiGroup.Base, UiGroup.MapHide, UiGroup.SurvivalBagHide });
            RegisterUi(UiNameConstant.CommonRoundOverModel, typeof(CommonRoundOverModel), typeof(RoundOverUiAdapter), UILayer.Tip, new object[] { contexts }, new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.BlastWeaponHudModel, typeof(BlastWeaponHudModel), typeof(BlastWeaponStateUiAdapter), UILayer.Base, new object[] { contexts }, new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.CommonBlastTipsModel, typeof(CommonBlastTipsModel), typeof(BlastTipsAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Base });
            RegisterUi(UiNameConstant.BlastRecordModel, typeof(BlastRecordModel), typeof(GroupRecordUiAdapter), UILayer.Pop, new object[] { contexts }, new UiGroup[] { UiGroup.Pop, UiGroup.Singleton });

            #region BiochemicalMark
            RegisterUi(UiNameConstant.BiochemicalMarkModel, typeof(BiochemicalMarkModel), typeof(BiochemicalMarkUiAdapter), UILayer.Base, new object[] { contexts },
                new UiGroup[] { UiGroup.Fix });
            #endregion

        }

        private static void RegisterUi(string name, Type mdel, Type adapter, UILayer layer, object[] param, UiGroup[] groups)
        {
            UIConfig.Regisiter(new UiConfigData() { Name = name, Model = mdel, Adapter = adapter, Layer = layer, Params =param, Groups = groups });
        }

        private static void CreateCommonUI()
        {
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonFocusMaskModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonHealthGroup);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonNoticeModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonScreenFlashModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonPickUpModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonMiniMap);
//            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonMaxMap);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonMap);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonPlayerInfo);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonCrossHairModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonHurtedModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonKillFeedbackModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonKillMessageModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonMCountDownModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonMenuModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonParachuteModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonRangingModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonTechStatModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonVehicleTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonChatModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponHudModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonCountdownTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonTaskTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonLocateModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonDeadModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonDebugInfoModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonVideoSettingModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonPaintDiscModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonOperationTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonSystemTipModel);
            //contexts.ui.uISession.CreateUi.Add(UiNameConstant.ChickenBagModel);
            //contexts.ui.uISession.CreateUi.Add(UiNameConstant.ChickenPlaneModel);
            //contexts.ui.uISession.CreateUi.Add(UiNameConstant.BiochemicalMarkModel);

        }

        public static void CreateTeamUI(int gameRule)
        {
            if (GameRules.Team != gameRule) return;

            contexts.ui.uISession.CreateUi.Add(UiNameConstant.GroupScoreModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.GroupRecordModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponBagModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponBagTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameOverModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameTitleModel);

        }

        public static void CreateSurvivalUI(int gameRule)
        {
            var survivalRules = new int[] {GameRules.LadderSoloSurvival, GameRules.LadderDoubleSurvival,
                GameRules.LadderFourGroupSurvival, GameRules.LadderFiveGroupSurvival,
                GameRules.SoloSurvival , GameRules.DoubleSurvival, GameRules.FourGroupSurvival, GameRules.FiveGroupSurvival};
            if (Array.IndexOf(survivalRules, gameRule) == -1) return;

            contexts.ui.uISession.CreateUi.Add(UiNameConstant.ChickenScoreModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonTeam);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonSplitModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonSuoDuModel);
           
        }

        public static void CreateBlastUI(int gameRule)
        {
            if (GameRules.Bomb != gameRule) return;

            contexts.ui.uISession.CreateUi.Add(UiNameConstant.BlastScoreModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonRoundOverModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponBagTipModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponBagModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameOverModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameTitleModel);
            contexts.ui.uISession.CreateUi.Remove(UiNameConstant.CommonWeaponHudModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.BlastWeaponHudModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonBlastTipsModel);
        }

        public static void CreateAnnihilationUI(int gameRule)
        {
            if (GameRules.Annihilation != gameRule) return;

            contexts.ui.uISession.CreateUi.Add(UiNameConstant.BlastScoreModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonRoundOverModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonWeaponBagModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameOverModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.CommonGameTitleModel);
            contexts.ui.uISession.CreateUi.Remove(UiNameConstant.CommonWeaponHudModel);
            contexts.ui.uISession.CreateUi.Add(UiNameConstant.BlastWeaponHudModel);
        }

        public static void Destroy()
        {
            UIConfig.configDic.Clear();
        }

    }

    public class UIConfig
    {
        public Dictionary<string, UiConfigData> configDic = new Dictionary<string, UiConfigData>();

        public void Regisiter(UiConfigData ui)
        {
            configDic.Add(ui.Name, ui);
        }

        public UiConfigData GetUIConfig(string name)
        {
            if (configDic.ContainsKey(name))
            {
                return configDic[name];
            }
            return null;
        }
    }

    public class UiConfigData
    {
        public string Name;
        public Type Model;
        public Type Adapter;
        public UILayer Layer;
        public object[] Params;
        public UiGroup[] Groups;
    }
}