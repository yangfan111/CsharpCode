using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.framework.action;
using com.wd.free.action;
using com.wd.free.action.function;
using com.wd.free.config;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.trigger;
using gameplay.gamerule.free.action;
using com.wd.free.xml;
using com.wd.free.map.position;
using gameplay.gamerule.free.component;
using System.Reflection;
using Sharpen;
using App.Shared.FreeFramework.framework.para.exp;
using gameplay.gamerule.free.player;
using gameplay.gamerule.free.condition;
using App.Shared.FreeFramework.framework.action;
using com.wd.free.map;
using com.wd.free.action.stage;
using com.wd.free.entity;
using App.Shared.FreeFramework.Free.player;
using App.Server.GameModules.GamePlay.Free.player;
using App.Server.GameModules.GamePlay.Free.weapon;
using com.wooduan.free.ui;
using App.Shared.FreeFramework.UnitTest;
using com.wd.free.ai;
using App.Shared.FreeFramework.Free.condition;
using App.Shared.FreeFramework.framework.freelog;
using com.wd.free.buf;
using App.Shared.FreeFramework.Free.Weapon;
using App.Shared.FreeFramework.Free.Action;
using App.Shared.FreeFramework.framework.ai.move;
using com.wd.free.condition;
using App.Shared.FreeFramework.framework.camera;
using App.Shared.FreeFramework.framework.ai;

namespace com.wd.free.config
{
    public class FreeConfig
    {
        private static XmlAlias alias;

        public static XmlAlias GetAlias()
        {
            if (alias == null)
            {
                alias = new XmlAlias();

                alias.AddClass("unit", new ConfigGameUnit());
                alias.AddClass("triggers", new TriggerList());
                alias.AddClass("trigger", new GameTrigger());
                alias.AddClass("multi-action", new MultiGameAction());
                alias.AddClass("condition-action", new ConditionGameAction());
                alias.AddClass("para-action", new DefineParaAction());
                alias.AddClass("para-value", new ParaValue());
                alias.AddClass("exp-condition", new ExpParaCondition());
                alias.AddClass("add-para-action", new AddParaAction());
                alias.AddClass("add-para-value-action", new AddParaValueAction());
                alias.AddClass("remove-para-action", new RemoveParaAction());
                alias.AddClass("arg-value", new ArgValue());
                alias.AddClass("func-arg", new FuncArg());
                alias.AddClass("game-func", new GameFunc());
                alias.AddClass("string", new StringPara());
                alias.AddClass("int", new IntPara());
                alias.AddClass("long", new LongPara());
                alias.AddClass("float", new FloatPara());
                alias.AddClass("double", new DoublePara());
                alias.AddClass("bool", new BoolPara());
                alias.AddClass("xyz", new XYZPara());
                alias.AddClass("elapse", new TimeElapsePara());
                alias.AddClass("update-int", new UpdatePara());
                alias.AddClass("sort-list", new ParaListSet());
                alias.AddClass("map", new MapPara());
                alias.AddClass("exp-action", new ExpGameAction());
                alias.AddClass("formula-action", new FormulaAction());
                alias.AddClass("random-action", new RandomGameAction());
                alias.AddClass("roll-action", new RollGameAction());
                alias.AddClass("save-var-action", new SaveVarAction());
                alias.AddClass("string-map-action", new StringMapAction());
                alias.AddClass("random-string-action", new RandomStringAction());
                alias.AddClass("string-multi-action", new StringMultiAction());
                alias.AddClass("con-action", new RandomGameAction.ConAction());
                alias.AddClass("sort-action", new ListAddAction());
                alias.AddClass("clear-list-action", new ListClearAction());
                alias.AddClass("sort-list-action", new ListSortAction());
                alias.AddClass("for-list-action", new ListForAction());
                alias.AddClass("for-action", new ForAction());
                alias.AddClass("add-from-other-action", new AddParaFromOtherAction());
                alias.AddClass("action-skill", new ActionSkill());
                alias.AddClass("effect-skill", new RangeBufSkill());
                alias.AddClass("aura-skill", new AuraSKill());
                alias.AddClass("buf-skill", new RangeBufSkill());
                alias.AddClass("group-effect", new AuraSKill.GroupEffect());
                alias.AddClass("simple-effect", new SimpleSkillEffect());
                alias.AddClass("simple-skill", new SimpleInstantSkill());
                alias.AddClass("click-trigger", new SkillClickTrigger());
                alias.AddClass("multi-trigger", new SkillMultiTrigger());
                alias.AddClass("exp-trigger", new SkillExpTrigger());
                alias.AddClass("press-trigger", new SkillPressTrigger());
                alias.AddClass("cast-trigger", new SkillCastTrigger());
                alias.AddClass("time-trigger", new SkillTimeTrigger());
                alias.AddClass("key-interrupter", new SkillKeyInterrupter());
                alias.AddClass("multi-interrupter", new SkillMultiInterrupter());
                alias.AddClass("condition-interrupter", new SkillConditionInterrupter());
                alias.AddClass("free-item", new FreeItem());
                alias.AddField("condition-action", "trueAction", "true");
                alias.AddField("condition-action", "falseAction", "false");
                alias.AddField("for-list-action", "noneAction", "none-action");
                alias.AddField("AbstractSkill", "disableAction", "disable-action");
                alias.AddField("AbstractCoolDownSkill", "notReadyAction", "not-ready-action");
                alias.AddField("AbstractCoolDownSkill", "alreadyAction", "already-action");
                alias.AddField("AbstractCoolDownSkill", "cooldownAction", "cooldown-action");
                alias.AddField("AbstractCoolDownSkill", "interAction", "inter-action");
                alias.AddField("AbstractSkillTrigger", "interAction", "inter-action");
                alias.AddField("cast-trigger", "castAction", "cast-action");
                alias.AddField("cast-trigger", "interAction", "inter-action");
                alias.AddField("cast-trigger", "startTrigger", "start-trigger");
                alias.AddField("time-trigger", "castAction", "cast-action");
                alias.AddField("time-trigger", "interAction", "inter-action");
                alias.AddField("time-trigger", "startTrigger", "start-trigger");
                alias.AddField("free-item", "buyAction", "buy-action");
                alias.AddField("free-item", "clickSkill", "click-skill");
                alias.AddField("free-item", "addAction", "add-action");
                alias.AddField("free-item", "removeAction", "remove-action");
                alias.AddField("free-item", "createAction", "create-action");
                alias.AddField("free-item", "notReadyAction", "not-ready-action");
                alias.AddField("free-item", "condition", "ready-condition");
                alias.AddField("free-item", "dragCondition", "drag-condition");
                alias.AddField("free-item", "dragAction", "drag-action");
                alias.AddAttribue("unit", "key", "key");
                alias.AddAttribue("unit", "parent", "parent");
                alias.AddAttribue("unit", "paraParent", "para-parent");
                alias.AddAttribue("unit", "triggerParent", "trigger-parent");
                alias.AddAttribue("trigger", "key", "key");
                alias.AddAttribue("trigger", "desc", "desc");
                alias.AddAttribue("trigger", "group", "group");
                alias.AddAttribue("trigger", "name", "name");
                alias.AddAttribue("AbstractGameAction", "desc", "desc");
                alias.AddAttribue("para-value", "name", "name");
                alias.AddAttribue("para-value", "type", "type");
                alias.AddAttribue("para-value", "isPublic", "public");
                alias.AddAttribue("para-value", "desc", "desc");
                alias.AddAttribue("para-value", "value", "value");
                alias.AddAttribue("exp-condition", "exp", "exp");
                alias.AddAttribue("add-para-action", "key", "key");
                alias.AddAttribue("add-para-action", "override", "override");
                alias.AddAttribue("add-para-value-action", "key", "key");
                alias.AddAttribue("add-para-value-action", "override", "override");
                alias.AddAttribue("remove-para-action", "key", "key");
                alias.AddAttribue("remove-para-action", "para", "para");
                alias.AddAttribue("arg-value", "name", "name");
                alias.AddAttribue("arg-value", "value", "value");
                alias.AddAttribue("func-arg", "name", "name");
                alias.AddAttribue("func-arg", "type", "type");
                alias.AddAttribue("func-arg", "desc", "desc");
                alias.AddAttribue("func-arg", "defaultValue", "default-value");
                alias.AddAttribue("game-func", "key", "key");
                alias.AddAttribue("game-func", "name", "name");
                alias.AddAttribue("game-func", "desc", "desc");
                alias.AddAttribue("AbstractPara", "name", "name");
                alias.AddAttribue("AbstractPara", "desc", "desc");
                alias.AddAttribue("AbstractPara", "isPublic", "public");
                alias.AddAttribue("sort-list", "order", "order");
                alias.AddAttribue("sort-list", "unique", "unique");
                alias.AddAttribue("sort-list", "capacity", "capacity");
                alias.AddAttribue("map", "number", "number");
                alias.AddAttribue("exp-action", "exp", "exp");
                alias.AddAttribue("formula-action", "para", "para");
                alias.AddAttribue("formula-action", "formula", "formula");
                alias.AddAttribue("roll-action", "percent", "percent");
                alias.AddAttribue("save-var-action", "name", "name");
                alias.AddAttribue("save-var-action", "var", "var");
                alias.AddAttribue("string-map-action", "keys", "keys");
                alias.AddAttribue("string-map-action", "key", "key");
                alias.AddAttribue("string-map-action", "useKey", "use-key");
                alias.AddAttribue("random-string-action", "keys", "keys");
                alias.AddAttribue("random-string-action", "all", "all");
                alias.AddAttribue("random-string-action", "count", "count");
                alias.AddAttribue("random-string-action", "repeat", "repeat");
                alias.AddAttribue("string-multi-action", "keys", "keys");
                alias.AddAttribue("con-action", "probability", "probability");
                alias.AddAttribue("sort-action", "sorter", "sorter");
                alias.AddAttribue("sort-action", "key", "key");
                alias.AddAttribue("clear-list-action", "sorter", "sorter");
                alias.AddAttribue("clear-list-action", "key", "key");
                alias.AddAttribue("clear-list-action", "exp", "exp");
                alias.AddAttribue("sort-list-action", "sorter", "sorter");
                alias.AddAttribue("sort-list-action", "key", "key");
                alias.AddAttribue("sort-list-action", "exp", "exp");
                alias.AddAttribue("sort-list-action", "keep", "keep");
                alias.AddAttribue("for-list-action", "sorter", "sorter");
                alias.AddAttribue("for-list-action", "key", "key");
                alias.AddAttribue("for-list-action", "remove", "remove");
                alias.AddAttribue("for-action", "from", "from");
                alias.AddAttribue("for-action", "to", "to");
                alias.AddAttribue("for-action", "i", "i");
                alias.AddAttribue("add-from-other-action", "key", "key");
                alias.AddAttribue("add-from-other-action", "from", "from");
                alias.AddAttribue("add-from-other-action", "fields", "fields");
                alias.AddAttribue("add-from-other-action", "override", "override");
                alias.AddAttribue("AbstractReflectAddAction", "fields", "fields");
                alias.AddAttribue("AbstractReflectSetAction", "fields", "fields");
                alias.AddAttribue("AbstractReflectSetAction", "values", "values");
                alias.AddAttribue("AbstractSkill", "key", "key");
                alias.AddAttribue("AbstractSkill", "desc", "desc");
                alias.AddAttribue("AbstractCoolDownSkill", "cooldown", "cooldown");
                alias.AddAttribue("AbstractCoolDownSkill", "interCoolDown", "inter-cooldown");
                alias.AddAttribue("AbstractCoolDownSkill", "last", "last");
                alias.AddAttribue("AbstractCoolDownSkill", "always", "always");
                alias.AddAttribue("aura-skill", "range", "range");
                alias.AddAttribue("buf-skill", "range", "range");
                alias.AddAttribue("group-effect", "key", "key");
                alias.AddAttribue("group-effect", "condition", "condition");
                alias.AddAttribue("AbstractSkillEffect", "key", "key");
                alias.AddAttribue("AbstractSkillEffect", "time", "time");
                alias.AddAttribue("AbstractSkillEffect", "source", "source");
                alias.AddAttribue("AbstractSkillEffect", "level", "level");
                alias.AddAttribue("click-trigger", "key", "key");
                alias.AddAttribue("multi-trigger", "or", "or");
                alias.AddAttribue("exp-trigger", "exp", "exp");
                alias.AddAttribue("press-trigger", "key", "key");
                alias.AddAttribue("cast-trigger", "time", "time");
                alias.AddAttribue("cast-trigger", "key", "key");
                alias.AddAttribue("time-trigger", "time", "time");
                alias.AddAttribue("key-interrupter", "press", "press");
                alias.AddAttribue("key-interrupter", "release", "release");
                alias.AddAttribue("free-item", "key", "key");
                alias.AddAttribue("free-item", "name", "name");
                alias.AddAttribue("free-item", "desc", "desc");
                alias.AddAttribue("free-item", "img", "img");
                alias.AddAttribue("free-item", "iconSize", "icon-size");
                alias.AddAttribue("free-item", "unique", "unique");
                alias.AddAttribue("free-item", "width", "width");
                alias.AddAttribue("free-item", "height", "height");
                alias.AddAttribue("free-item", "consume", "consume");
                alias.AddAttribue("free-item", "useAll", "use-all");
                alias.AddAttribue("free-item", "itemStack", "item-stack");
                alias.AddAttribue("free-item", "useClose", "use-close");

                alias.AddClass("commons", new CommonActions());
                alias.AddClass("use-action", new UseCommonAction());
                alias.AddClass("common-action", new CommonGameAction());
                alias.AddAttribue("use-action", "key", "key");
                alias.AddAttribue("common-action", "scope", "scope");
                alias.AddAttribue("common-action", "key", "key");
                alias.AddAttribue("common-action", "name", "name");
                alias.AddAttribue("common-action", "group", "group");
                alias.AddAttribue("common-action", "desc", "desc");

                alias.AddClass("components", new GameComponents());
                alias.AddClass("use-component", new UseGameComponent());
                alias.AddAttribue("use-component", "key", "key");
                alias.AddAttribue("use-component", "name", "name");
                alias.AddAttribue("use-component", "desc", "desc");

                alias.AddClass("component-arg-action", new DefineComponentArgAction());

                alias.AddClass("component-pos", new ComponentPosSelector());
                alias.AddAttribue("component-pos", "name", "name");
                alias.AddAttribue("component-pos", "title", "title");
                alias.AddField("component-pos", "defaultPos", "default-pos");

                alias.AddClass("component-effect", new ComponentFreeEffect());
                alias.AddAttribue("component-effect", "name", "name");
                alias.AddAttribue("component-effect", "title", "title");
                alias.AddField("component-effect", "defaultEffect", "default-effect");

                alias.AddClass("component-action", new ComponentAction());
                alias.AddAttribue("component-action", "name", "name");
                alias.AddAttribue("component-action", "title", "title");
                alias.AddField("component-action", "defaultAction", "default-action");

                alias.AddClass("component-ui", new ComponentUIComponent());
                alias.AddAttribue("component-ui", "name", "name");
                alias.AddAttribue("component-ui", "title", "title");
                alias.AddField("component-ui", "component", "default-ui");

                alias.AddClass("component-trigger", new ComponentTrigger());
                alias.AddAttribue("component-trigger", "name", "name");
                alias.AddAttribue("component-trigger", "title", "title");
                alias.AddAttribue("component-trigger", "desc", "desc");
                alias.AddField("component-trigger", "defaultTrigger", "default-trigger");

                alias.AddClass("component-condition", new ComponentParaCondition());
                alias.AddAttribue("component-condition", "name", "name");
                alias.AddAttribue("component-condition", "title", "title");
                alias.AddAttribue("component-condition", "desc", "desc");
                alias.AddField("component-condition", "defaultCondition", "default-condition");

                alias.AddClass("component-skill", new ComponentSkill());
                alias.AddAttribue("component-skill", "name", "name");
                alias.AddAttribue("component-skill", "title", "title");
                alias.AddAttribue("component-skill", "desc", "desc");
                alias.AddField("component-skill", "defaultSkill", "default-skill");


                alias.AddClass("player-action-skill", new PlayerActionSkill());
                alias.AddAttribue("AbstractPlayerAction", "player", "player");
                alias.AddClass("add-skill-action", new AddSkillAction());

                alias.AddClass("assign-position", new PosAssignSelector());
                alias.AddClass("adjust-position", new PosAdjustSelector());
                alias.AddAttribue("assign-position", "x", "x");
                alias.AddAttribue("assign-position", "y", "y");
                alias.AddAttribue("assign-position", "z", "z");
                alias.AddAttribue("assign-position", "yaw", "yaw");
                alias.AddAttribue("assign-position", "pitch", "pitch");
                alias.AddAttribue("adjust-position", "x", "x");
                alias.AddAttribue("adjust-position", "y", "y");
                alias.AddAttribue("adjust-position", "z", "z");
                alias.AddAttribue("adjust-position", "pitch", "pitch");
                alias.AddAttribue("adjust-position", "yaw", "yaw");
                alias.AddClass("direction-position", new PosDirectionSelector());
                alias.AddAttribue("direction-position", "distance", "distance");
                alias.AddAttribue("direction-position", "height", "height");
                alias.AddAttribue("direction-position", "pitch", "pitch");
                alias.AddAttribue("direction-position", "angle", "angle");


                alias.AddClass("time-filter-action", new TimeFilterAction());
                alias.AddAttribue("time-filter-action", "interval", "interval");
                alias.AddAttribue("time-filter-action", "firstDelay", "first-delay");

                alias.AddClass("create-free-buf-action", new FreeBufCreateAction());
                alias.AddClass("free-buf", new FreeBuf());
                alias.AddField("free-buf", "eatAction", "eat-action");
                alias.AddField("free-buf", "enterAction", "enter-action");
                alias.AddField("free-buf", "leaveAction", "leave-action");
                alias.AddField("free-buf", "effectAction", "effect-action");
                alias.AddField("free-buf", "createAction", "create-action");
                alias.AddAttribue("free-buf", "key", "key");
                alias.AddAttribue("free-buf", "type", "type");
                alias.AddAttribue("free-buf", "vars", "vars");
                alias.AddAttribue("free-buf", "condition", "condition");
                alias.AddAttribue("free-buf", "effect", "effect");
                alias.AddAttribue("free-buf", "consume", "consume");
                alias.AddAttribue("free-buf", "time", "time");

                alias.AddClass("timer-action", new TimerGameAction());
                alias.AddClass("cancel-timer-action", new CancelTimerAction());
                alias.AddField("timer-action", "startAction", "start-action");
                alias.AddField("timer-action", "action", "action");
                alias.AddAttribue("timer-action", "name", "name");
                alias.AddAttribue("timer-action", "time", "time");
                alias.AddAttribue("timer-action", "count", "count");
                alias.AddAttribue("cancel-timer-action", "name", "name");

                AliasOnes(alias);
            }

            return alias;
        }

        private static void AliasOnes(XmlAlias alias)
        {
            aliasOne(alias, new ActionCondition());
            aliasOne(alias, new PlayerHonorAction());
            aliasOne(alias, new PlayerHonorAction.PlayerHonor());
            aliasOne(alias, new AndCondition());
            aliasOne(alias, new RandomSeedAction());
            aliasOne(alias, new RemoveFreeBufAction());
            aliasOne(alias, new AddMultiFrameAction());
            aliasOne(alias, new StageTimeAction());
            aliasOne(alias, new OneTimeStageAction());
            aliasOne(alias, new TimeLastAction());
            aliasOne(alias, new ChangeMoveAction());
            aliasOne(alias, new AddPredictSkillAction());
            aliasOne(alias, new PlayerAnimationAction());
            aliasOne(alias, new NewRemoveWeaponAction());
            aliasOne(alias, new PlayerDisableAction());
            aliasOne(alias, new SimpleMessageAction());
            aliasOne(alias, new MessageField());
            aliasOne(alias, new VarTestValue());
            aliasOne(alias, new PlayerTestValue());
            aliasOne(alias, new IniAvatarAction());
            aliasOne(alias, new OrderAiAction());
            aliasOne(alias, new OneTimeAiAction());
            aliasOne(alias, new WaitTimeAiAction());
            aliasOne(alias, new FinishStepAiAction());
            aliasOne(alias, new RegionCondition());
            aliasOne(alias, new SkillConditionTrigger());
            aliasOne(alias, new DebugFreeAction());
            aliasOne(alias, new FileLogAction());
            aliasOne(alias, new DefinePlayerBufAction());
            aliasOne(alias, new DirectMoveCondition());
            aliasOne(alias, new DefineWeaponSkillAction());
            aliasOne(alias, new HasWeaponCondition());
            aliasOne(alias, new PreloadResourceAction());
            aliasOne(alias, new PlayerBattleDataResetAction());
            aliasOne(alias, new MoveToAiAction());
            aliasOne(alias, new FaceToAiAction());
            aliasOne(alias, new PlayerRaycastCondition());
            aliasOne(alias, new OrParaCondition());
            aliasOne(alias, new NotParaCondition());
            aliasOne(alias, new AndParaCondition());
            aliasOne(alias, new CameraFollowAction());
            aliasOne(alias, new CameraResetAction());
            aliasOne(alias, new AnimationTestValue());
            aliasOne(alias, new PlayerTestValue());
            aliasOne(alias, new UiTestValue());
            aliasOne(alias, new VarTestValue());
            aliasOne(alias, new WeaponTestValue());
            aliasOne(alias, new PlayerInterceptKeyAction());
            aliasOne(alias, new StartUnitTestAction());
            aliasOne(alias, new SetRoomStatusAction());
            aliasOne(alias, new OneCaseAction());
            aliasOne(alias, new PlayerObserveAction());
        }

        // 会把父类的字段也会加入，需要注意当以前的代码中父类的字段没有按照这样的命名规范时会有问题
        private static void aliasOne(XmlAlias alias, object obj)
        {
            string classAlias = ToXmlName(obj.GetType().Name);
            alias.AddClass(classAlias, obj);

            string[] fields = ReflectionCache.GetFieldNames(obj);
            foreach (string field in fields)
            {
                FieldInfo fi = ReflectionCache.GetField(obj, field);
                string type = fi.FieldType.Name.ToLower();
                if (ReflectionCache.IsSimpleField(type))
                {
                    alias.AddAttribue(classAlias, fi.Name, ToXmlName(fi.Name));
                }
                else
                {
                    alias.AddField(classAlias, fi.Name, ToXmlName(fi.Name));
                }
            }
        }

        private static string ToXmlName(string name)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (c.IsUpper())
                {
                    if (i > 0)
                    {
                        sb.Append('-');

                    }
                    sb.Append(char.ToLower(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
