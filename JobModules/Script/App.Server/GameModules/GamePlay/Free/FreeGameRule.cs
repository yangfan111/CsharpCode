using System;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.entity;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.para;
using Core.Prediction.UserPrediction.Cmd;
using Free.framework;
using gameplay.gamerule.free.ui;
using Sharpen;
using UnityEngine;
using App.Shared.GameModules.Bullet;
using Core.Utils;
using App.Server.Scripts.Config;
using WeaponConfigNs;
using App.Shared.Components.Player;
using App.Client.GameModules.GamePlay.Free;
using com.cpkf.yyjd.tools.util.math;
using App.Shared.Util;
using com.wd.free.action;
using App.Shared.FreeFramework.framework.trigger;
using Utils.Singleton;
using App.Shared.GameModules.Player;
using Core.Free;
using App.Shared.FreeFramework.Free.player;

namespace App.Server.GameModules.GamePlay
{
    public class FreeGameRule : IGameRule, IParable
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FreeGameRule));

        private bool firstTime;
        private long serverTime;
        private long startTime;
        private int unitTestStage;

        private long lastServerTime;

        private SimpleParaList paras;
        private int mapId;
        private int ruleId;

        private FreeRuleEventArgs args;

        private const string StatePara = "gameStage";

        private const string EnterPara = "canEnter";

        private bool serverGameOver;
        private bool serverGameExit;

        public ServerRoom Room;

        public FreeGameRule(ServerRoom room)
        {
            this.args = (FreeRuleEventArgs)room.RoomContexts.session.commonSession.FreeArgs;
            this.mapId = room.RoomContexts.session.commonSession.RoomInfo.MapId;
            this.ruleId = room.RoomContexts.session.commonSession.RoomInfo.ModeId;

            this.Room = room;

            RandomUtil.SetSeed(RandomUtil.Random(1, 10000000));

            SendMessageAction.sender = new FreeMessageSender();

            try
            {
                string rule = RuleMap.GetRuleName(ruleId);
                FreeRuleConfig config = FreeRuleConfig.GetRule(rule, SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Mysql);
                args.Triggers.Merge(config.Triggers);
                args.ComponentMap = config.GameComponentMap;
                args.AddDefault(this);
                args.Functions.Merge(config.Functions);
                args.rule = this;
                args.FreeContext.TestCase.Merge(config.Triggers);

                AddCommonTrigger();
            }
            catch (Exception e)
            {
                Debug.LogError("加载模式" + RuleMap.GetRuleName(ruleId) + "失败.\n" + e.Message);
                _logger.Error("加载模式" + RuleMap.GetRuleName(ruleId) + "失败.\n", e);
            }

            this.paras = new SimpleParaList();
            paras.AddFields(new ObjectFields(this));

            this.paras.AddPara(new IntPara(StatePara, 0));
            this.paras.AddPara(new IntPara(EnterPara, 0));
        }

        // 通用的不变的游戏机制
        private void AddCommonTrigger()
        {
            args.Triggers.AddTrigger(FreeTriggerConstant.PRELOAD_RESOURCE, new PlayerPreloadAction());
        }

        public bool GameOver
        {
            get { return serverGameOver; }
            set { serverGameOver = value; }
        }

        public string FreeType { get { return RuleMap.GetRuleName(ruleId); } }

        public void Reload(Contexts room, string rule)
        {
            //FreeRuleConfig config = FreeRuleConfig.GetRule(rule, true);
            //triggers = new GameTriggers();

            //triggers.Merge(config.Triggers);

            //args.Functions.Clear();
            //args.Functions.Merge(this._config.Functions);

            //foreach (PlayerEntity playerEntity in room.player.GetEntities())
            //{
            //    playerEntity.RemoveFreeData();
            //    FreeData fd = new FreeData(playerEntity);
            //    fd.AddFields(new PlayerFields(playerEntity));
            //    playerEntity.AddFreeData(fd);

            //    args.TempUse("current", (FreeData)playerEntity.freeData.FreeData);

            //    triggers.Trigger(ADD_PLAYER, args);

            //    args.Resume("current");
            //}
        }

        public ParaList GetParameters()
        {
            return paras;
        }

        public long ServerTime
        {
            get { return serverTime; }
        }

        public int GameStage
        {
            get
            {
                return (int)args.GetDefault().GetParameters().Get(StatePara).GetValue();
            }
        }

        public int EnterStatus
        {
            get
            {
                return (int)args.GetDefault().GetParameters().Get(EnterPara).GetValue();
            }
        }

        public Contexts Contexts
        {
            get { return args.GameContext; }
        }

        public bool GameExit
        {
            get { return serverGameExit; }
            set { serverGameExit = value; }

        }

        private FloatPara damagePara;

        public float HandleDamage(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage)
        {
            if (damagePara == null)
            {
                damagePara = new FloatPara("damage", damage.damage);
            }

            damagePara.SetValue(Math.Min(damage.damage, target.gamePlay.CurModeHp));

            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.DAMAGE))
            {
                if (source != null)
                {
                    args.TempUse("source", (FreeData)source.freeData.FreeData);
                }
                args.TempUse("target", (FreeData)target.freeData.FreeData);

                SimpleParaList dama = new SimpleParaList();
                dama.AddFields(new ObjectFields(damage));
                SimpleParable sp = new SimpleParable(dama);
                args.TempUse("damage", sp);

                args.TempUsePara(damagePara);

                this.args.Triggers.Trigger(FreeTriggerConstant.DAMAGE, args);

                if (source != null)
                {
                    args.Resume("source");
                }

                args.Resume("target");
                args.Resume("damage");
                args.ResumePara("damage");
            }


            return (float)damagePara.GetValue();
        }

        public void KillPlayer(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage)
        {
            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.KILL_PLAYER))
            {
                if (source != null)
                {
                    args.TempUse("killer", (FreeData)source.freeData.FreeData);
                }
                args.TempUse("killed", (FreeData)target.freeData.FreeData);

                SimpleParaList dama = new SimpleParaList();
                dama.AddFields(new ObjectFields(damage));
                SimpleParable sp = new SimpleParable(dama);
                args.TempUse("damage", sp);

                this.args.Triggers.Trigger(FreeTriggerConstant.KILL_PLAYER, args);

                if (source != null)
                {
                    args.Resume("killer");
                }
                args.Resume("killed");
                args.Resume("damage");
            }

            if (source != null)
            {
                source.statisticsData.Statistics.SetSrcDataByDamageInfo(damage);
            }
            if (target != null)
            {
                target.statisticsData.Statistics.SetTarDataByDamageInfo(damage);
            }
        }

        public void Update(Contexts room, int interval)
        {
            FreeLog.Reset();

            //if (firstTime == false)
            //{
            //    GameStart(room);

            //    firstTime = true;
            //}

            serverTime = Runtime.CurrentTimeMillis() - startTime;

            args.FreeContext.TimerTask.TimeElapse(args, (int)(serverTime - lastServerTime));
            UpdateFreeMoveEntity(room, interval);
            args.FreeContext.TestCase.Frame(args);
            args.FreeContext.MultiFrame.Act(args);

            PlayerEntity[] players = args.GameContext.player.GetInitializedPlayerEntities();
            for (int i = 0; i < players.Length; i++)
            {
                FreeData freeData = (FreeData)players[i].freeData.FreeData;
                freeData.Bufs.Frame(args);
                if (!args.Triggers.IsEmpty(FreeTriggerConstant.FRAME_PLAYER))
                {
                    args.Trigger(FreeTriggerConstant.FRAME_PLAYER, new TempUnit("player", freeData));
                }
            }

            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.FRAME))
            {
                this.args.Triggers.Trigger(FreeTriggerConstant.FRAME, args);
            }


            lastServerTime = serverTime;

            FreeLog.Print();
        }

        private void UpdateFreeMoveEntity(Contexts room, int interval)
        {
            FreeMoveEntity[] freeMoves = room.freeMove.GetEntities();
            foreach (var freeMoveEntity in freeMoves)
            {
                if (freeMoveEntity.freeData.FreeData != null)
                {
                    FreeLog.SetTrigger(freeMoveEntity.freeData.FreeData);
                    ((FreeEntityData)freeMoveEntity.freeData.FreeData).Frame(args, interval);
                }
            }
        }

        public void GameStart(Contexts room)
        {
            this.startTime = Runtime.CurrentTimeMillis();

            this.serverTime = 0;

            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.GAME_INI))
            {
                this.args.Triggers.Trigger(FreeTriggerConstant.GAME_INI, args);
            }
        }

        public void GameEnd(Contexts room)
        {

        }

        public void PlayerPressCmd(Contexts room, PlayerEntity player, IUserCmd cmd)
        {
            FreeLog.Reset();

            args.input.SetUserCmd(cmd);

            FreeData freeData = (FreeData)player.freeData.FreeData;

            if (freeData.Player.gamePlay.LifeState != (int)EPlayerLifeState.Dead)
            {
                args.TempUse("current", freeData);

                args.FreeContext.Bufs.Eat(player, args);

                freeData.freeInventory.UsingItem(args);

                //freeData.GetUnitSkill().Frame(args);

                freeData.StateTimer.AutoRemove(args, serverTime);

                args.Resume("current");
            }

            FreeLog.Print();
        }

        public void PlayerEnter(Contexts room, PlayerEntity player)
        {
            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.ADD_PLAYER))
            {
                args.TempUse("current", (FreeData)player.freeData.FreeData);

                this.args.Triggers.Trigger(FreeTriggerConstant.ADD_PLAYER, args);

                args.Resume("current");
            }
        }

        public void PlayerLeave(Contexts room, PlayerEntity player)
        {
            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.REMOVE_PLAYER))
            {
                if (null != player.freeData && null != player.freeData.FreeData)
                {
                    args.TempUse("leave", (FreeData)player.freeData.FreeData);
                }

                this.args.Triggers.Trigger(FreeTriggerConstant.REMOVE_PLAYER, args);

                args.Resume("leave");
            }
        }

        private StringPara _eventKey;

        public void HandleFreeEvent(Contexts room, PlayerEntity player, SimpleProto message)
        {
            if (message.Key == 1)
            {
                if (_eventKey == null)
                {
                    _eventKey = new StringPara("event", "");
                }
                if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.CLICK_IMAGE))
                {
                    _eventKey.SetValue(message.Ss[0]);

                    args.TempUsePara(_eventKey);

                    this.args.Triggers.Trigger(FreeTriggerConstant.CLICK_IMAGE, args);

                    args.ResumePara("event");
                }
            }
        }

        //TODO 找到调用的位置，确认逻辑行为
        public void HandleWeaponState(Contexts contexts, PlayerEntity player)
        {
            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.WEAPON_STATE))
            {
                SimpleParaList dama = new SimpleParaList();
                //dama.AddFields(new ObjectFields(state));
                //dama.AddPara(new IntPara("CarryClip", state.ReservedBulletCount));
                //dama.AddPara(new IntPara("Clip", state.LoadedBulletCount));
                ////dama.AddPara(new IntPara("ClipType", (int)state.Caliber));
                //dama.AddPara(new IntPara("id", (int)state.CurrentWeapon));
                SimpleParable sp = new SimpleParable(dama);
                args.TempUse("state", sp);
                args.TempUse("current", (FreeData)player.freeData.FreeData);

                this.args.Triggers.Trigger(FreeTriggerConstant.WEAPON_STATE, args);

                args.Resume("state");
                args.Resume("current");
            }
        }

        public void HandleWeaponFire(Contexts contexts, PlayerEntity player, WeaponResConfigItem weapon)
        {
            if (!this.args.Triggers.IsEmpty(FreeTriggerConstant.WEAPON_FIRE))
            {
                SimpleParaList dama = new SimpleParaList();
                dama.AddFields(new ObjectFields(weapon));
                SimpleParable sp = new SimpleParable(dama);
                args.TempUse("weapon", sp);
                args.TempUse("current", (FreeData)player.freeData.FreeData);

                this.args.Triggers.Trigger(FreeTriggerConstant.WEAPON_FIRE, args);

                args.Resume("weapon");
                args.Resume("current");
            }
        }
    }
}
