using System.Collections.Generic;
using App.Shared.GameContexts;
using Assets.App.Shared.GameContexts;
using BehaviorDesigner.Runtime.Tasks;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.SpatialPartition;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.ContextInfos
{
    public class GameContextsUtility
    {
        public static bool IsIncludeSceneObject = true;

        public static Bin2D<IGameEntity> GetBin2D(IBin2DManager bin, int type)
        {
            if (bin == null) return null;
            var b = bin.GetBin2D(type);
            if (b == null) return null;
            return b.Bin2D;
        }

        public static IGameContexts GetReplicationGameContexts(Contexts contexts, IBin2DManager bin = null)
        {
            var entitasContextInfos = new Core.EntityComponent.GameContexts();
            entitasContextInfos.AddContextInfo(new PlayerGameContext(contexts.player,
                GetBin2D(bin, (int) EEntityType.Player)));
            entitasContextInfos.AddContextInfo(new BulletGameContext(contexts.bullet,
                GetBin2D(bin, (int) EEntityType.Bullet)));
            entitasContextInfos.AddContextInfo(new ThrowingGameContext(contexts.throwing,
                GetBin2D(bin, (int) EEntityType.Throwing)));
            entitasContextInfos.AddContextInfo(new ClientEffectGameContext(contexts.clientEffect,
                GetBin2D(bin, (int) EEntityType.ClientEffect)));
            entitasContextInfos.AddContextInfo(new VehicleGameContext(contexts.vehicle,
                GetBin2D(bin, (int) EEntityType.Vehicle)));
            entitasContextInfos.AddContextInfo(new FreeMoveGameContext(contexts.freeMove,
                GetBin2D(bin, (int) EEntityType.FreeMove)));
            entitasContextInfos.AddContextInfo(new SoundGameContext(contexts.sound,
                GetBin2D(bin, (int) EEntityType.Sound)));
            entitasContextInfos.AddContextInfo(new WeaponGameContext(contexts.weapon,
                GetBin2D(bin, (int)EEntityType.Weapon)));
            if (IsIncludeSceneObject)
            {
                entitasContextInfos.AddContextInfo(new SceneObjectGameContext(contexts.sceneObject,
                    GetBin2D(bin, (int) EEntityType.SceneObject)));
                entitasContextInfos.AddContextInfo(new MapObjectGameContext(contexts.mapObject,
                    GetBin2D(bin, (int) EEntityType.MapObject)));
            }
#if UNITY_EDITOR


            foreach (var context in entitasContextInfos.AllContexts)
            {
                AssertUtility.Assert(context.CanContainComponent<EntityKeyComponent>());
                AssertUtility.Assert(context.CanContainComponent<FlagDestroyComponent>());
                AssertUtility.Assert(context.CanContainComponent<EntityAdapterComponent>());
            }
#endif
            return entitasContextInfos;
        }

        public static bool SceneObjectFilter(Vector3 position)
        {
            return position.y <= 500;
        }

        public static int next_p2(int a)
        {
            int rval = 1;
            // rval<<=1 Is A Prettier Way Of Writing rval*=2; 
            while (rval < a) rval <<= 1;
            return rval;
        }

        public static IBin2DManager GetReplicationBin2DManager(float minX, float minY, float maxX, float maxY,
            int visibleRadius, Dictionary<int, int> customVisibleRadiusDict)
        {
            IBin2DManager bin2DManager = new Bin2DManager();
            for (int i = 0; i < (int) EEntityType.End; i++)
            {
                var v = visibleRadius;
                if (customVisibleRadiusDict.ContainsKey(i))
                {
                    v = customVisibleRadiusDict[i];
                }

                var cell = next_p2(v) / 4 > 8 ? next_p2(v) / 4 : 8;
                Bin2DConfig _config = new Bin2DConfig(minX, minY, maxX, maxY, cell, v);
                if (i == (int) EEntityType.SceneObject || i == (int) EEntityType.MapObject)
                {
                    bin2DManager.AddBin2D(i, new Bin2D<IGameEntity>(_config), v, SceneObjectFilter);
                }
                else
                {
                    bin2DManager.AddBin2D(i, new Bin2D<IGameEntity>(_config), v);
                }
            }

            return bin2DManager;
        }
    }
}