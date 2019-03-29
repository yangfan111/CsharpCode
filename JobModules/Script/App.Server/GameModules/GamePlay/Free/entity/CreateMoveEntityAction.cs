using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.util;
using Core.EntityComponent;
using gameplay.gamerule.free.ui;
using UnityEngine;
using App.Server.GameModules.GamePlay.free.player;
using Utils.AssetManager;
using gameplay.gamerule.free.ui.component;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class CreateMoveEntityAction : AbstractPlayerAction
    {
        public String name;
        public IFreeMove move;
        public IGameAction createAction;
        public IGameAction action;
        public IGameAction removeAction;
        public IGameAction deadAction;
        public IGameAction damageAction;
        public IParaCondition condition;
        public IParaCondition removeCondition;
        public IParaCondition removedCondition;
        public IGameAction frameAction;
        public FreeEffectCreateAction effect;
        public int length;
        public int width;
        public int height;
        public int hp;
        public bool @fixed;
        public int distance;

        public List<ISkill> skills;

        public override void DoAction(IEventArgs args)
        {
            FreeMoveEntity en = args.GameContext.freeMove.CreateEntity();
            en.AddEntityKey(new EntityKey(args.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            en.AddPosition(new Vector3());
            en.AddFreeData(FreeUtil.ReplaceVar(name, args), new FreeEntityData(en));
            en.freeData.Cat = "";
            en.freeData.Value = "";
            en.freeData.Scale = new Vector3(1, 1, 1);

            if (distance > 0)
            {
                en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, distance);
            }

            en.isFlagSyncNonSelf = true;

            FreeEntityData entityData = (FreeEntityData)en.freeData.FreeData;

            entityData.move = (IFreeMove)SerializeUtil.Clone(move);
            entityData.createAction = createAction;
            entityData.action = action;
            entityData.name = FreeUtil.ReplaceVar(name, args);
            entityData.condition = condition;
            entityData.removeAction = removeAction;
            entityData.effect = (FreeEffectCreateAction)SerializeUtil.Clone(effect);
            entityData.width = width;
            entityData.length = length;
            entityData.height = height;
            entityData.deadAction = deadAction;
            entityData.damageAction = damageAction;
            entityData.frameAction = (IGameAction)SerializeUtil.Clone(frameAction);
            entityData.removeCondition = removeCondition;
            entityData.removedCondition = removedCondition;
            entityData.hp = hp;

            if (width > 0 && height > 0 && length > 0)
            {
                AssetInfo info = GetAssetInfo(args, effect);
                if (!string.IsNullOrEmpty(info.AssetName))
                {
                    args.GameContext.session.commonSession.AssetManager.LoadAssetAsync(
                        entityData, info, SetGameObject);
                }
            }

            if (skills != null)
            {
                entityData.skills = new List<ISkill>();
                foreach (ISkill skill in skills)
                {
                    entityData.skills.Add((ISkill)SerializeUtil.Clone(skill));
                }
            }

            FreeData fd = GetPlayer(args);
            if (fd != null)
            {
                args.TempUse("creator", fd);
            }

            entityData.Start(args);

            if (fd != null)
            {
                args.Resume("creator");
            }
        }

        private AssetInfo GetAssetInfo(IEventArgs args, FreeEffectCreateAction effect)
        {
            if (effect != null)
            {
                if (effect.GetEffects().Count >= 1)
                {
                    IFreeEffect ef = effect.GetEffects()[0];
                    if (ef is FreeParticleEffect)
                    {
                        FreeParticleEffect particle = (FreeParticleEffect)ef;
                        string url = particle.GetStyle(args, null);
                        int last = url.LastIndexOf("/");
                        return new AssetInfo(url.Substring(0, last), url.Substring(last + 1));
                    }
                }
            }

            return default(AssetInfo);
        }

        private void SetGameObject(FreeEntityData obj, UnityObject unityObj)
        {
            GameObject go = unityObj.AsGameObject;
            go.name = this.name;
            ((FreeEntityData)obj).gameObject = go;
        }
    }
}
