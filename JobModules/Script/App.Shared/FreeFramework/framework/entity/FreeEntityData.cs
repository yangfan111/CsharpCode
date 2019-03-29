using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.unit;
using Core.Free;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using UnityEngine;
using App.Shared.FreeFramework.framework.ui.component;
using com.wd.free.map.position;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    public class FreeEntityData : IFreeData, IGameUnit
    {
        public string name;
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
        public List<PlayerEntity> follows;
        public GameObject gameObject;

        public List<ISkill> skills;

        [NonSerialized]
        private UnitSkill skill;

        public SimpleParaList paras;

        [NonSerialized]
        private FreeMoveEntity _entity;

        [NonSerialized]
        private int id;

        private IntPara xPara;
        private IntPara yPara;
        private IntPara zPara;

        public FreeEntityData(FreeMoveEntity entity)
        {
            this._entity = entity;
            this.paras = new SimpleParaList();
            this.paras.AddFields(new ObjectFields(this));
            this.id = entity.entityKey.Value.EntityId;

            this.follows = new List<PlayerEntity>();

            xPara = new IntPara("x", 0);
            yPara = new IntPara("y", 0);
            zPara = new IntPara("z", 0);

            paras.AddPara(xPara);
            paras.AddPara(yPara);
            paras.AddPara(zPara);
        }

        public FreeMoveEntity FreeMoveEntity
        {
            get { return _entity; }
        }

        public string Key { get { return name; } }

        public void Start(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            args.TempUse("entity", this);
            args.TempUse(name, this);

            if (createAction != null)
            {
                createAction.Act(args);
            }

            if (skills != null && skills.Count > 0)
            {
                skill = new UnitSkill(this);
                foreach (ISkill sk in skills)
                {
                    skill.AddSkill(sk);
                }
            }

            move.Start(fr, _entity);

            xPara.SetValue(_entity.position.Value.x);
            yPara.SetValue(_entity.position.Value.y);
            zPara.SetValue(_entity.position.Value.z);

            if (effect != null)
            {
                AutoPositionValue auto = new AutoPositionValue();
                auto.SetId(_entity.entityKey.Value.EntityId.ToString());
                auto.SetField("pos");
                effect.AddAuto(auto);

                AutoScaleValue scale = new AutoScaleValue();
                scale.SetId(_entity.entityKey.Value.EntityId.ToString());
                scale.SetField("scale");
                effect.AddAuto(scale);

                effect.SetSelector(new PosAssignSelector(_entity.position.Value.x.ToString(),
                    _entity.position.Value.y.ToString(), _entity.position.Value.z.ToString()));

                //Debug.LogFormat("start pos {0}", _entity.position.Value.ToString());

                effect.Act(args);
            }

            args.Resume(name);
            args.Resume("entity");
        }

        public void SetMove(IEventArgs args, IFreeMove move)
        {
            this.move = move;

            args.TempUse("entity", this);
            args.TempUse(name, this);

            move.Start((FreeRuleEventArgs)args, _entity);

            args.Resume(name);
            args.Resume("entity");
        }

        public void Frame(IEventArgs args, int interval)
        {
            if (_entity.isFlagDestroy)
            {
                return;
            }
            args.TempUse("entity", this);
            args.TempUse(name, this);

            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            move.Frame(fr, _entity, interval);

            xPara.SetValue(_entity.position.Value.x);
            yPara.SetValue(_entity.position.Value.y);
            zPara.SetValue(_entity.position.Value.z);

            foreach (PlayerEntity player in follows)
            {
                Vector3 v = _entity.position.Value;
                //v.y = v.y + 20;
                player.position.Value = v;
            }
            if(gameObject != null)
            {
                gameObject.transform.position = _entity.position.Value;
            }

            if (skill != null)
            {
                skill.Frame((FreeRuleEventArgs)args);
            }

            if (frameAction != null)
            {
                frameAction.Act(args);
            }

            args.Resume(name);
            args.Resume("entity");
        }

        public long GetID()
        {
            return _entity.entityKey.Value.EntityId;
        }

        public string GetKey()
        {
            return name;
        }

        public UnitSkill GetUnitSkill()
        {
            return skill;
        }

        public XYZPara GetXYZ()
        {
            return new XYZPara();
        }

        public ParaList GetParameters()
        {
            return paras;
        }
    }
}
