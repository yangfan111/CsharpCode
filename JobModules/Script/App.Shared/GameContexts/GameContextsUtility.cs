﻿using System.Collections.Generic;
using App.Shared.GameContexts;
using Assets.App.Shared.GameContexts;
using BehaviorDesigner.Runtime.Tasks;
using Core.Components;
using Core.EntityComponent;
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
            var gameContexts = new Core.EntityComponent.GameContexts();
            gameContexts.AddContextEle(new PlayerGameContext(contexts.player,
                GetBin2D(bin, (int) EEntityType.Player)));
            gameContexts.AddContextEle(new BulletGameContext(contexts.bullet,
                GetBin2D(bin, (int) EEntityType.Bullet)));
            gameContexts.AddContextEle(new ThrowingGameContext(contexts.throwing,
                GetBin2D(bin, (int) EEntityType.Throwing)));
            gameContexts.AddContextEle(new ClientEffectGameContext(contexts.clientEffect,
                GetBin2D(bin, (int) EEntityType.ClientEffect)));
            gameContexts.AddContextEle(new VehicleGameContext(contexts.vehicle,
                GetBin2D(bin, (int) EEntityType.Vehicle)));
            gameContexts.AddContextEle(new FreeMoveGameContext(contexts.freeMove,
                GetBin2D(bin, (int) EEntityType.FreeMove)));
            gameContexts.AddContextEle(new SoundGameContext(contexts.sound,
                                                                         GetBin2D(bin, (int) EEntityType.Sound)));
            gameContexts.AddContextEle(new WeaponGameContext(contexts.weapon,
                GetBin2D(bin, (int)EEntityType.Weapon)));
            if (IsIncludeSceneObject)
            {
                gameContexts.AddContextEle(new SceneObjectGameContext(contexts.sceneObject,
                    GetBin2D(bin, (int) EEntityType.SceneObject)));
                gameContexts.AddContextEle(new MapObjectGameContext(contexts.mapObject,
                    GetBin2D(bin, (int) EEntityType.MapObject)));
            }
#if UNITY_EDITOR


            foreach (var context in gameContexts.AllContexts)
            {
                AssertUtility.Assert(context.CanContainComponent<EntityKeyComponent>());
                AssertUtility.Assert(context.CanContainComponent<FlagDestroyComponent>());
                AssertUtility.Assert(context.CanContainComponent<EntityAdapterComponent>());
            }
#endif
            return gameContexts;
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
        /// player -128
        /// bullet -128
        /// client -128
        /// throw  -128
        /// sceneObject -4
        /// freeMove  -2048
        /// MapObject -32
        public static IBin2DManager GetReplicationBin2DManager(float minX, float minY, float maxX, float maxY,
            int defaultVisibleRadius, Dictionary<int, int> customVisibleRadiusDict)
        {
            IBin2DManager bin2DManager = new Bin2DManager();
            for (int i = 0; i < (int) EEntityType.End; i++)
            {
                var v = defaultVisibleRadius;
                if (customVisibleRadiusDict.ContainsKey(i))
                {
                    v = customVisibleRadiusDict[i];
                }

                var cell = next_p2(v) / 8 > 2 ? next_p2(v) / 8 : 2;
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