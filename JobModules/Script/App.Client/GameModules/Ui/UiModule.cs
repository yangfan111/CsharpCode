using System.Collections.Generic;
using System.Text;
using App.Client.GameModules.Free;
using App.Client.GameModules.Ui.System;
using App.Client.Utility;
using App.Shared;
using App.Shared.DebugHandle;
using Assets.UiFramework.Libs;
using Core.GameModule.Module;
using Core.Utils;
using UnityEngine;
using Assets.App.Client.GameModules.Ui;
using Core.GameModule.Interface;
using UIComponent.UI;

namespace App.Client.GameModules.Ui
{
    public class UiModule : GameModule, IUiHfrSystem
    {
        static List<AbstractModel> _uiModels = new List<AbstractModel>();
        static Dictionary<string, AbstractModel> _uiModelsDic = new Dictionary<string, AbstractModel>();

        public static void AddModel(AbstractModel model)
        {
            _uiModels.Add(model);
        }

        public static UiModule instance = null;
        public static Contexts contexts = null;

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UiModule));

        public UiModule(Contexts contexts)
        {
            UiModule.contexts = contexts;

            instance = this;
            var loader = new UiResourceLoader(contexts.session.commonSession.AssetManager);
            AbstractModel.SetUiResourceLoader(loader);
            FreeGlobalVars.Loader = loader;
            if (UIImageLoader.LoadSpriteAsync == null) //没有通过大厅直接进入游戏
            {
                UIImageLoader.LoadSpriteAsync = loader.RetriveSpriteAsync;
            }

            if (UIImageLoader.LoadTextureAsync == null)
            {
                UIImageLoader.LoadTextureAsync = loader.RetriveTextureAsync;
            }

            AddModelSystems();
            AddSystem(new UiSessionSystem(contexts));
            //AddSystem(new UiPlayerDataInitSystem(contexts));
            AddSystem(new ObserveUISystem(contexts));
            AddSystem(this);
        }

        private void AddModelSystems()
        {
            foreach (var model in _uiModels)
            {
                if (model is IUserSystem)
                    AddSystem(model as IUserSystem);
            }
        }

        public void HideUI(string name)
        {
            if (CursorLocker.SystemUnlock) return;
            contexts.ui.uISession.UiState[name] = false;
        }

        public void ShowUI(string name)
        {
            if (CursorLocker.SystemUnlock) return;
            contexts.ui.uISession.UiState[name] = true;
        }

        public string GetUIIndex()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            var names = contexts.ui.uISession.UiAdapters.Keys;
            foreach (var name in names)
            {
                sb.Append(name);
                sb.Append("\n");
            }

            return sb.ToString();
        }


        public static void DestroyAll()
        {
            foreach (var model in _uiModels)
            {
                model.Destory();
            }

            _uiModels.Clear();
            _uiModelsDic.Clear();
            UiCreateFactory.Destroy();
            UiCommon.UIManager.Destroy();
        }

        public static void ShowSpecialModelAndHideALL(string modelName)
        {
            foreach (var key in _uiModelsDic.Keys)
            {
                if (key != modelName)
                {
                    _uiModelsDic[key].SetVisible(false);
                }
                else
                {
                    _uiModelsDic[key].SetVisible(true);
                }
            }
        }

        public static void HideSpecialModelAndShowALL(string modelName)
        {
            foreach (var key in _uiModelsDic.Keys)
            {
                if (key != modelName)
                {
                    _uiModelsDic[key].SetVisible(true);
                }
                else
                {
                    _uiModelsDic[key].SetVisible(false);
                }
            }
        }

        public static string ProcessDebugCommand(DebugCommand message)
        {
            switch (message.Command)
            {
                case DebugCommands.UIList:
                    return instance.GetUIIndex();
                case DebugCommands.ShowUI:
                    if (message.Args.Length < 1) return "require name";
                    instance.ShowUI(message.Args[0]);
                    return "OK";
                case DebugCommands.HideUI:
                    if (message.Args.Length < 1) return "require name";
                    instance.HideUI(message.Args[0]);
                    return "OK";
                case DebugCommands.HeatMapRestart:
                    SharedConfig.StopSampler = false;
                    return "OK";
                case DebugCommands.HeatMapPause:
                    SharedConfig.StopSampler = true;
                    return "OK";
            }

            return string.Empty;
        }

        public static void CloseGameSettingUI()
        {
            var go = GameObject.Find("HallGameController");
            if (go == null)
                return;
            go.SendMessage("HideSettingWindow");
        }

        public void OnUiRender(float intervalTime)
        {
            UiCommon.UIManager.Update();
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SwitchUIRootShow();
            }
        }

        private void SwitchUIRootShow()
        {
            var root = UiCommon.UIManager.UIRoot;
            root.SetActive(!root.activeSelf);
        }
    }
}