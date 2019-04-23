﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.Module;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;

namespace App.Shared.GameModules.Configuration
{
    public class SubResourceConfigurationInitModule : GameModule
    {
        public SubResourceConfigurationInitModule(ISessionState sessionState)
        {
            AddSystem(new SubResourceLoadSystem(sessionState, new CharacterSpeedSubResourceHandler()));
            AddSystem(new SubResourceLoadSystem(sessionState, new WeaponAvatarAnimSubResourceHandler()));
           // AddSystem(new SubResourceLoadSystem(sessionState, new ShaderWarmUpHandler()));
        }
    }

    public class ShaderWarmUpHandler : AbstractSubResourceLoadHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ShaderWarmUpHandler));

        public readonly AssetInfo[] _toWarmUpShaderAssets = new[]
            {new AssetInfo("shader", "Standard (MSAO)"), new AssetInfo("shader", "Standard (MSAO)_FirstPerson")};

        protected override bool LoadSubResourcesImpl()
        {
            bool hasAsset = false;
            foreach (var asset in _toWarmUpShaderAssets)
            {
                if (AddLoadRequest(asset))
                    _logger.InfoFormat("Add WeaponAvatarAnimSubResource {0}", asset);
                hasAsset = true;
            }

            return hasAsset;
        }

        protected override void OnLoadSuccImpl(UnityObject unityObj)
        {
            var asset = unityObj.As<ShaderVariantCollection>();
            _logger.InfoFormat("ShaderVariantCollection:{0}",asset);
            if (null != asset)
            {
                asset.WarmUp();
                
            }
        }
    }
}