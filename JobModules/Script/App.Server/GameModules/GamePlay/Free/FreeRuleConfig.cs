using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using App.Server.GameModules.GamePlay.free;
using App.Server.GameModules.GamePlay.Free.action;
using App.Server.GameModules.GamePlay.Free.entity;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.cpkf.yyjd.tools.util;
using com.graphbuilder.struc;
using com.wd.free.action;
using com.wd.free.config;
using com.wd.free.map.position;
using com.wd.free.trigger;
using com.wd.free.xml;
using commons.data;
using commons.data.mysql;
using commons.util;
using gameplay.gamerule.free.action;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.map;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using MySql.Data.MySqlClient;
using Sharpen;
using StringUtil = com.cpkf.yyjd.tools.util.StringUtil;
using App.Server.GameModules.GamePlay.Free.condition;
using App.Server.GameModules.GamePlay.Free.player;
using gameplay.gamerule.free.player;
using App.Server.GameModules.GamePlay.Free.action.ui;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.framework.ui;
using App.Server.GameModules.GamePlay.Free.map;
using App.Server.GameModules.GamePlay.Free.weapon;
using App.Server.GameModules.GamePlay.Free.ui;
using App.Server.GameModules.GamePlay.Free.item.config;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.Free.replacer;
using UnityEngine;
using App.Shared.FreeFramework.Free.Map;
using App.Server.GameModules.GamePlay.Free.chicken;
using gameplay.gamerule.free.component;
using App.Server.GameModules.GamePlay.Free.app;
using App.Server.GameModules.GamePlay.Free.hall;
using App.Shared.FreeFramework.UnitTest;
using Assets.App.Server.GameModules.GamePlay.Free.UnitTest;

namespace App.Server.GameModules.GamePlay
{
    [Serializable]
    public class FreeRuleConfig
    {
        private string name;
        private TriggerList triggers;
        private CommonActions commons;
        private GameComponents components;
        private GameComponentMap gameComponentMap;

        private string imports;

        [NonSerialized]
        private static XmlAlias alias;

        [NonSerialized]
        private static string _connectionString;

        static FreeRuleConfig()
        {
            FreeUtil.RegisterReplacer(new ItemDefReplacer());
            FreeUtil.RegisterReplacer(new TextDescReplacer());
        }

        public FreeRuleConfig()
        {
            this.triggers = new TriggerList();
            this.commons = new CommonActions();
            this.components = new GameComponents();
            this.gameComponentMap = new GameComponentMap();
        }

        public static FreeRuleConfig GetRule(string name, Boolean mysql)
        {
            FreeRuleConfig rule = (FreeRuleConfig)FromXml(name, GetRuleXml(name, mysql));

            rule.gameComponentMap.Merge(rule.components);

            foreach (UseGameComponent ugc in rule.components)
            {
                ugc.Merge(rule.triggers);
            }

            if (rule.imports != null)
            {
                foreach (string sub in rule.imports.Split(","))
                {
                    rule.merge(rule, (FreeRuleConfig)FromXml(sub, GetRuleXml(sub, mysql)), mysql);
                }
            }

            foreach (UseGameComponent ugc in rule.components)
            {
                rule.merge(rule, (FreeRuleConfig)FromXml(ugc.GetKey(), GetRuleXml(ugc.GetKey(), mysql)), mysql);
            }

            return rule;
        }

        private void merge(FreeRuleConfig root, FreeRuleConfig sub, bool mysql)
        {
            if (sub == null)
            {
                return;
            }

            root.gameComponentMap.Merge(sub.components);

            if (sub.commons != null)
            {
                root.commons.Merge(sub.commons);
            }

            if (sub.triggers != null)
            {
                root.triggers.MergeTriggerList(sub.triggers);
            }

            foreach (UseGameComponent ugc in sub.components)
            {
                ugc.Merge(root.triggers);
            }

            if (!string.IsNullOrEmpty(sub.imports))
            {
                foreach (string subSub in sub.imports.Split(","))
                {
                    merge(root, (FreeRuleConfig)FromXml(subSub, GetRuleXml(subSub, mysql)), mysql);
                }
            }

            foreach (UseGameComponent ugc in sub.components)
            {
                merge(root, (FreeRuleConfig)FromXml(ugc.GetKey(), GetRuleXml(ugc.GetKey(), mysql)), mysql);
            }
        }


        public TriggerList Triggers
        {
            get { return triggers; }
        }

        public CommonActions Functions
        {
            get { return commons; }
        }

        public static string GetRuleXml(string name, bool mysql)
        {
            if (mysql)
            {
                List<DataRecord> list = MysqlUtil.SelectRecords("select * from rule where `key` = '" + name + "'", MysqlConnection);
                if (list.Count > 0)
                {
                    return RemoveComment(list[0].GetValue("config"));
                }
            }
            else
            {
                return RemoveComment(GetFileContent(Application.dataPath + "/GameData/Server/Rule/" + name + ".xml", Encoding.UTF8));
            }

            return "";
        }

        public static string GetXmlContent(string fileName)
        {
            return RemoveComment(GetFileContent(Application.dataPath + "/GameData/Server/GameConfig/" + fileName + ".xml", Encoding.UTF8));
        }

        private static string GetFileContent(string filename, Encoding enconding)
        {
            StreamReader din = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), enconding);
            StringBuilder st = new StringBuilder();
            string str;

            while ((str = din.ReadLine()) != null)
            {
                st.Append(str);
                st.Append("\n");
            }

            din.Close();

            return st.ToString();
        }

        private static String RemoveComment(String xml)
        {
            XMLParser p = new XMLParser(xml, true);

            XmlNode any = p.GetNode("//*");
            XmlDocument doc = any.OwnerDocument;

            XmlNodeList nl = p.GetNodes("//*[@comment='true']");
            if (nl != null)
            {
                for (int i = 0; i < nl.Count; i++)
                {
                    XmlNode node = nl[i];
                    if (node != null && node.ParentNode != null)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }

            return XMLWriter.Format(doc);
        }

        public static string MysqlConnection
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    MySqlConnectionStringBuilder config = new MySqlConnectionStringBuilder();
                    //config.Database = "we";
                    //config.Server = "192.168.0.2";
                    //config.Port = 3306;
                    //config.UserID = "root";
                    //config.Password = "@wan5d.com@";

                    XMLParser parser = new XMLParser(Application.dataPath + "/GameData/Server/connection.xml");

                    config.Server = parser.GetNodeValue("//host");
                    config.Database = parser.GetNodeValue("//db-name");
                    config.UserID = parser.GetNodeValue("//user");
                    config.Password = parser.GetNodeValue("//password");
                    config.Port = uint.Parse(parser.GetNodeValue("//port"));
                    config.CharacterSet = "utf8";

                    _connectionString = config.ConnectionString;
                }

                return _connectionString;
            }
        }

        public GameComponentMap GameComponentMap
        {
            get { return gameComponentMap; }
            set { gameComponentMap = value; }
        }

        public static object XmlToObject(string xml)
        {
            return XmlParser.FromXml(RemoveComment(xml), GetRuleAlias());
        }

        public static object FromXml(string name, string xml)
        {
            FreeRuleConfig config = (FreeRuleConfig)XmlParser.FromXml(RemoveComment(xml), GetRuleAlias());
            config.name = name;
            foreach (GameTrigger gt in config.Triggers.GetTriggers())
            {
                gt.SetRule(config.name);
            }

            return config;
        }

        public static XmlAlias GetRuleAlias()
        {
            if (alias == null)
            {
                alias = FreeConfig.GetAlias();

                SetRuleAlias(alias);
                aliasItem(alias);
                aliasCommon(alias);
                aliasUnity(alias);
                aliasUI(alias);
            }

            return alias;
        }

        private static void aliasUnity(XmlAlias alias)
        {
            aliasOne(alias, new ReloadRuleAction());
            aliasOne(alias, new FreePositionMove());
            aliasOne(alias, new RemoveEntityAction());
            aliasOne(alias, new DistanceCondition());
            aliasOne(alias, new PlayerMoveAction());
            aliasOne(alias, new BombHurtAction());
            aliasOne(alias, new BombCircleAction());
            aliasOne(alias, new CommentAction());
            aliasOne(alias, new CreateFixEntityAction());
            aliasOne(alias, new PlayerHideAction());
            aliasOne(alias, new OnlyDataItemUi());
            aliasOne(alias, new FreeUiDuplicateAction());
            aliasOne(alias, new CreateSceneObjectAction());
            aliasOne(alias, new PlayerHurtAction());
            aliasOne(alias, new RegisterCommandAction());
            aliasOne(alias, new RefreshSceneItemAction());
            aliasOne(alias, new OnePrefabEvent());
            aliasOne(alias, new FreeUiAddChildAction());
            aliasOne(alias, new UnityTraceAction());
            aliasOne(alias, new RefreshItemAtPosAction());
            aliasOne(alias, new PosGroundSelector());
            aliasOne(alias, new PlayerItemAvatarAction());
            aliasOne(alias, new PlayerItemWeaponAction());
            aliasOne(alias, new DebugInfoAction());
            aliasOne(alias, new UnityInventoryUi());
            aliasOne(alias, new UnityEmptyInventoryUi());
            aliasOne(alias, new UnityOneInventoryUi());
            aliasOne(alias, new NewAddWeaponAction());
            aliasOne(alias, new NewRemoveWeaponAction());
            aliasOne(alias, new PlayerItemPartAction());
            aliasOne(alias, new RemoveSceneObjectAction());
            aliasOne(alias, new FindItemAction());
            aliasOne(alias, new MysqlLogAction());
            aliasOne(alias, new CreateCarAction());
            aliasOne(alias, new PosTypeCondition());
            aliasOne(alias, new PlayerResetAction());
            aliasOne(alias, new SimpleBagAuto());
            aliasOne(alias, new CreateBoxItemAction());
            aliasOne(alias, new ScoreInfoAction());
            aliasOne(alias, new UseCodeAction());
            aliasOne(alias, new SetPlayerGameStateAction());
            aliasOne(alias, new SetPlayerUiStateAction());
            aliasOne(alias, new SetPlayerCastStateAction());
            aliasOne(alias, new SelectFreeMoveAction());
            aliasOne(alias, new PlayerStateCondition());
            aliasOne(alias, new LockMouseAction());
            aliasOne(alias, new PlayerSpeedAction());
            aliasOne(alias, new ShowAppUiAction());
            aliasOne(alias, new PlayerAnimationAction());
            aliasOne(alias, new PlaySoundAction());
            aliasOne(alias, new FollowEntityAction());
            aliasOne(alias, new ChickenRuleAction());
            aliasOne(alias, new ChickenRuleCondition());
            aliasOne(alias, new GameOverAction());
            aliasOne(alias, new PlayerReliveAction());
            aliasOne(alias, new BattleEndAction());
            aliasOne(alias, new SingleEndAction());
            aliasOne(alias, new PlayerWeaponDropAction());
            aliasOne(alias, new PlayerArmorAction());
            aliasOne(alias, new SelectNearPointAction());
            aliasOne(alias, new GroupTechStatUiAction());
            aliasOne(alias, new PlayerMaskAction());
            aliasOne(alias, new RealTimeAction());
            aliasOne(alias, new ClearSceneEntitiesAction());
            aliasOne(alias, new CreateCastSceneEntityAction());
            aliasOne(alias, new RemoveCastSceneEntityAction());
            aliasOne(alias, new PlayerBagLockAction());
            aliasOne(alias, new TestCaseAction());
            aliasOne(alias, new TestCaseMultiAction());
            aliasOne(alias, new PlayerReportAction());
            aliasOne(alias, new ChangeWeaponAction());
            aliasOne(alias, new ServerShutdownAction());
            aliasOne(alias, new SetUnitTestDataAction());
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

        private static void aliasUI(XmlAlias alias)
        {
            alias.AddClass("poison-circle-action", new PoisonCirlceAction());
            alias.AddField("poison-circle-action", "fromPos", "from-pos");
            alias.AddField("poison-circle-action", "toPos", "to-pos");
            alias.AddAttribue("poison-circle-action", "fromRadius", "from-radius");
            alias.AddAttribue("poison-circle-action", "toRadius", "to-radius");
            alias.AddAttribue("poison-circle-action", "fromWaitTime", "from-wait-time");
            alias.AddAttribue("poison-circle-action", "toWaitTime", "to-wait-time");
            alias.AddAttribue("poison-circle-action", "fromMoveTime", "from-move-time");
            alias.AddAttribue("poison-circle-action", "toMoveTime", "to-move-time");
        }

        private static void aliasCommon(XmlAlias alias)
        {
            alias.AddClass("code-action", new UseCodeAction());

            alias.AddClass("create-entity-action", new CreateMoveEntityAction());
            alias.AddField("create-entity-action", "removeAction", "remove-action");
            alias.AddField("create-entity-action", "removeCondition", "remove-condition");
            alias.AddField("create-entity-action", "createAction", "create-action");
            alias.AddField("create-entity-action", "deadAction", "dead-action");
            alias.AddField("create-entity-action", "damageAction", "damage-action");
            alias.AddField("create-entity-action", "frameAction", "frame-action");
            alias.AddField("create-entity-action", "removedCondition", "removedCondition");
            alias.AddAttribue("create-entity-action", "name", "name");
            alias.AddAttribue("create-entity-action", "hp", "hp");
            alias.AddAttribue("create-entity-action", "fixed", "fixed");
            alias.AddAttribue("create-entity-action", "length", "length");
            alias.AddAttribue("create-entity-action", "width", "width");
            alias.AddAttribue("create-entity-action", "height", "height");


            alias.AddAttribue("AbstractMapRegion", "useOut", "use-out");
            alias.AddClass("circle-region", new MapCircleRegion());
            alias.AddAttribue("circle-region", "radius", "radius");
            alias.AddAttribue("circle-region", "change", "change");
            alias.AddAttribue("circle-region", "zRange", "z-range");

            alias.AddClass("entity-position", new PosEntitySelector());
            alias.AddAttribue("entity-position", "condition", "condition");
            alias.AddAttribue("entity-position", "player", "player");
            alias.AddAttribue("entity-position", "distance", "distance");
            alias.AddAttribue("entity-position", "height", "height");
            alias.AddAttribue("entity-position", "pitch", "pitch");
            alias.AddAttribue("entity-position", "angle", "angle");
            alias.AddAttribue("entity-position", "fromEye", "from-eye");

            alias.AddClass("free-position", new PosEditSelector());
            alias.AddAttribue("free-position", "type", "type");
            alias.AddAttribue("free-position", "index", "index");
            alias.AddAttribue("free-position", "notSame", "not-same");
            alias.AddAttribue("free-position", "birth", "birth");

            alias.AddClass("angle-position", new PosAngleSelector());
            alias.AddAttribue("angle-position", "radius", "radius");
            alias.AddAttribue("angle-position", "angle", "angle");
            alias.AddField("angle-position", "targetPos", "target-pos");
            alias.AddField("angle-position", "pos", "pos");

            alias.AddClass("not-move", new FreeNotMove());
            alias.AddField("not-move", "startPos", "pos");

            alias.AddClass("text-msg-action", new TextMessageAction());
            alias.AddAttribue("text-msg-action", "message", "message");
            alias.AddAttribue("text-msg-action", "interval", "interval");

            alias.AddClass("select-player-action", new SelectPlayerAction());
            alias.AddField("select-player-action", "noneAction", "none-action");
            alias.AddAttribue("select-player-action", "condition", "exp");
            alias.AddAttribue("select-player-action", "order", "order");
            alias.AddAttribue("select-player-action", "count", "count");
            alias.AddAttribue("select-player-action", "observer", "observer");
            alias.AddAttribue("select-player-action", "selectedName", "target");

            alias.AddClass("select-point-action", new SelectPointAction());
            alias.AddAttribue("select-point-action", "count", "count");
        }

        private static void SetRuleAlias(XmlAlias alias)
        {
            alias.AddClass("rule", new FreeRuleConfig());
            alias.AddField("rule", "triggers", "triggers");
            alias.AddAttribue("rule", "imports", "imports");

            alias.AddClass("create-ui-action", new FreeUICreateAction());
            alias.AddClass("image-ui", new FreeImageComponet());
            alias.AddClass("r-image-ui", new FreeRotationImageComponet());
            alias.AddClass("prefab-ui", new FreePrefabComponet());
            alias.AddClass("text-ui", new FreeTextComponet());
            alias.AddClass("number-ui", new FreeNumberComponet());
            alias.AddClass("list-ui", new FreeListComponet());
            alias.AddClass("rader-ui", new FreeRaderComponet());
            alias.AddClass("exp-ui", new FreeExpComponet());
            alias.AddClass("time-auto", new AutoTimeValue());
            alias.AddClass("time-once-auto", new AutoOneTimeValue());
            alias.AddClass("time-unit-auto", new AutoTimeUnitValue());
            alias.AddClass("string-auto", new AutoTimeStringValue());
            alias.AddClass("const-auto", new AutoConstValue());
            alias.AddClass("visible-auto", new AutoVisibleValue());
            alias.AddClass("point-auto", new AutoPointValue());
            alias.AddClass("rotate-auto", new AutoRotateValue());
            alias.AddClass("position-auto", new AutoPositionValue());
            alias.AddClass("two-position-auto", new AutoTwoPositionValue());
            alias.AddClass("percent-auto", new AutoPercentValue());
            alias.AddClass("cover-auto", new AutoImgCoverValue());
            alias.AddClass("player-auto", new AutoPlayerValue());
            alias.AddClass("client-auto", new AutoClientValue());
            alias.AddClass("update-ui-action", new FreeUIUpdateAction());
            alias.AddClass("update-effect-action", new FreeEffectUpdateAction());
            alias.AddClass("effect-value", new FreeEffectValue());
            alias.AddClass("image-value", new FreeUIImageValue());
            alias.AddClass("text-value", new FreeUITextValue());
            alias.AddClass("number-value", new FreeUINumberValue());
            alias.AddClass("list-value", new FreeUIListValue());
            alias.AddClass("show-ui-action", new FreeUIShowAction());
            alias.AddField("cover-auto", "coverU", "u");
            alias.AddField("cover-auto", "coverV", "v");
            alias.AddAttribue("create-ui-action", "key", "key");
            alias.AddAttribue("create-ui-action", "show", "show");
            alias.AddAttribue("create-ui-action", "atBottom", "at-bottom");
            alias.AddAttribue("AbstractFreeComponent", "desc", "desc");
            alias.AddAttribue("AbstractFreeComponent", "event", "event");
            alias.AddAttribue("AbstractFreeComponent", "width", "width");
            alias.AddAttribue("AbstractFreeComponent", "height", "height");
            alias.AddAttribue("AbstractFreeComponent", "x", "x");
            alias.AddAttribue("AbstractFreeComponent", "y", "y");
            alias.AddAttribue("AbstractFreeComponent", "relative", "relative");
            alias.AddAttribue("AbstractFreeComponent", "parent", "parent");
            alias.AddAttribue("image-ui", "url", "url");
            alias.AddAttribue("image-ui", "coverUrl", "cover-url");
            alias.AddAttribue("image-ui", "center", "center");
            alias.AddAttribue("image-ui", "fixed", "fixed");
            alias.AddAttribue("image-ui", "originalSize", "original-size");
            alias.AddAttribue("image-ui", "useMask", "use-mask");
            alias.AddAttribue("image-ui", "isMask", "is-mask");
            alias.AddAttribue("image-ui", "mirror", "mirror");
            alias.AddAttribue("prefab-ui", "name", "name");
            alias.AddAttribue("r-image-ui", "url", "url");
            alias.AddAttribue("r-image-ui", "reverse", "reverse");
            alias.AddAttribue("text-ui", "text", "text");
            alias.AddAttribue("text-ui", "size", "size");
            alias.AddAttribue("text-ui", "color", "color");
            alias.AddAttribue("text-ui", "bold", "bold");
            alias.AddAttribue("text-ui", "font", "font");
            alias.AddAttribue("text-ui", "hAlign", "h-align");
            alias.AddAttribue("text-ui", "vAlign", "v-align");
            alias.AddAttribue("text-ui", "multiLine", "multi-line");
            alias.AddAttribue("number-ui", "number", "number");
            alias.AddAttribue("number-ui", "font", "font");
            alias.AddAttribue("number-ui", "length", "length");
            alias.AddAttribue("number-ui", "align", "align");
            alias.AddAttribue("number-ui", "scale", "scale");
            alias.AddAttribue("number-ui", "showZero", "show-zero");
            alias.AddAttribue("list-ui", "column", "column");
            alias.AddAttribue("list-ui", "row", "row");
            alias.AddAttribue("AbstractAutoValue", "field", "field");
            alias.AddAttribue("AbstractAutoValue", "all", "all");
            alias.AddAttribue("AbstractAutoValue", "id", "id");
            alias.AddAttribue("time-auto", "time", "time");
            alias.AddAttribue("time-auto", "from", "from");
            alias.AddAttribue("time-auto", "to", "to");
            alias.AddAttribue("time-auto", "reverse", "reverse");
            alias.AddAttribue("time-once-auto", "time", "time");
            alias.AddAttribue("time-once-auto", "from", "from");
            alias.AddAttribue("time-once-auto", "to", "to");
            alias.AddAttribue("time-unit-auto", "unit", "unit");
            alias.AddAttribue("time-unit-auto", "scale", "scale");
            alias.AddAttribue("time-unit-auto", "desc", "desc");
            alias.AddAttribue("string-auto", "time", "time");
            alias.AddAttribue("string-auto", "values", "values");
            alias.AddAttribue("const-auto", "value", "value");
            alias.AddAttribue("visible-auto", "reverse", "reverse");
            alias.AddAttribue("visible-auto", "xyz", "xyz");
            alias.AddAttribue("point-auto", "xyz", "xyz");
            alias.AddAttribue("point-auto", "delta", "delta");
            alias.AddAttribue("rotate-auto", "angle", "angle");
            alias.AddAttribue("position-auto", "distance", "distance");
            alias.AddAttribue("position-auto", "height", "height");
            alias.AddAttribue("position-auto", "pitch", "pitch");
            alias.AddAttribue("position-auto", "angle", "angle");
            alias.AddAttribue("two-position-auto", "distance", "distance");
            alias.AddAttribue("two-position-auto", "height", "height");
            alias.AddAttribue("two-position-auto", "source", "source");
            alias.AddAttribue("two-position-auto", "target", "target");
            alias.AddAttribue("two-position-auto", "toSource", "to-source");
            alias.AddAttribue("player-auto", "key", "key");
            alias.AddAttribue("client-auto", "key", "key");
            alias.AddAttribue("client-auto", "type", "type");
            alias.AddAttribue("update-ui-action", "key", "key");
            alias.AddAttribue("update-effect-action", "key", "key");
            alias.AddAttribue("AbstractFreeUIValue", "seq", "seq");
            alias.AddAttribue("AbstractFreeUIValue", "autoStatus", "auto-status");
            alias.AddAttribue("AbstractFreeUIValue", "autoIndex", "auto-index");
            alias.AddAttribue("effect-value", "value", "value");
            alias.AddAttribue("image-value", "url", "url");
            alias.AddClass("prefab-value", new FreePrefabValue());
            alias.AddClass("one-field-value", new OnePrefabValue());
            alias.AddAttribue("one-field-value", "field", "field");
            alias.AddAttribue("one-field-value", "value", "value");
            alias.AddAttribue("text-value", "text", "text");
            alias.AddAttribue("number-value", "number", "number");
            alias.AddAttribue("list-value", "sorts", "sorts");
            alias.AddAttribue("list-value", "order", "order");
            alias.AddAttribue("list-value", "range", "range");
            alias.AddAttribue("list-value", "capacity", "capacity");
            alias.AddAttribue("list-value", "fields", "fields");
            alias.AddAttribue("list-value", "autoCondition", "auto-condition");
            alias.AddAttribue("show-ui-action", "key", "key");
            alias.AddAttribue("show-ui-action", "time", "time");

            alias.AddClass("create-effect-action", new FreeEffectCreateAction());
            alias.AddClass("show-effect-action", new FreeEffectShowAction());
            alias.AddClass("delete-effect-action", new FreeEffectDeleteAction());
            alias.AddClass("single-effect", new FreeSingleEffect());
            alias.AddClass("particle-effect", new FreeParticleEffect());
            alias.AddClass("particle-fix-effect", new FreeFixParticleLinkEffect());
            alias.AddAttribue("create-effect-action", "key", "key");
            alias.AddAttribue("create-effect-action", "show", "show");
            alias.AddAttribue("create-effect-action", "pvs", "pvs");
            alias.AddAttribue("create-effect-action", "scale", "scale");
            alias.AddAttribue("create-effect-action", "rotation", "rotation");
            alias.AddAttribue("create-effect-action", "img", "img");
            alias.AddAttribue("create-effect-action", "desc", "desc");
            alias.AddAttribue("show-effect-action", "key", "key");
            alias.AddAttribue("show-effect-action", "time", "time");
            alias.AddAttribue("delete-effect-action", "key", "key");
            alias.AddAttribue("single-effect", "xyz", "xyz");
            alias.AddAttribue("single-effect", "scale", "scale");
            alias.AddAttribue("single-effect", "rotation", "rotation");
            alias.AddAttribue("single-effect", "depth", "depth");
            alias.AddAttribue("single-effect", "ptype", "ptype");
            alias.AddAttribue("single-effect", "culling", "culling");
            alias.AddAttribue("single-effect", "resUrl", "res-url");
            alias.AddAttribue("single-effect", "model", "model");
            alias.AddAttribue("single-effect", "gtype", "gtype");
            alias.AddAttribue("single-effect", "alpha", "alpha");
            alias.AddAttribue("single-effect", "fixed", "fixed");
            alias.AddAttribue("single-effect", "fixPoint", "fix-point");
            alias.AddAttribue("cube-effect", "xyz", "xyz");
            alias.AddAttribue("cube-effect", "scale", "scale");
            alias.AddAttribue("cube-effect", "rotation", "rotation");
            alias.AddAttribue("cube-effect", "resUrl", "res-url");
            alias.AddAttribue("cube-effect", "alpha", "alpha");
            alias.AddAttribue("two-effect", "size", "size");
            alias.AddAttribue("two-effect", "source", "source");
            alias.AddAttribue("two-effect", "target", "target");
            alias.AddAttribue("two-effect", "resUrl", "res-url");
            alias.AddAttribue("two-effect", "fromZ", "from-z");
            alias.AddAttribue("two-effect", "toZ", "to-z");
            alias.AddAttribue("particle-effect", "xyz", "xyz");
            alias.AddAttribue("particle-effect", "scale", "scale");
            alias.AddAttribue("particle-effect", "rotation", "rotation");
            alias.AddAttribue("particle-effect", "name", "name");
            alias.AddAttribue("particle-two-effect", "scale", "scale");
            alias.AddAttribue("particle-two-effect", "source", "source");
            alias.AddAttribue("particle-two-effect", "target", "target");
            alias.AddAttribue("particle-two-effect", "name", "name");
            alias.AddAttribue("particle-two-effect", "adjust", "adjust");
            alias.AddAttribue("particle-two-effect", "fromZ", "from-z");
            alias.AddAttribue("particle-two-effect", "toZ", "to-z");
            alias.AddAttribue("particle-fix-effect", "scale", "scale");
            alias.AddAttribue("particle-fix-effect", "name", "name");
            alias.AddAttribue("particle-fix-effect", "adjust", "adjust");

        }

        private static void aliasItem(XmlAlias alias)
        {
            alias.AddClass("define-item-action", new DefineItemAction());
            alias.AddClass("free-game-item", new FreeGameItem());
            alias.AddClass("create-item-player-action", new CreateItemToPlayerAction());
            alias.AddClass("create-item-scene-action", new CreateItemToSceneAction());
            alias.AddClass("player-drop-item-action", new PlayerDropItemAction());
            alias.AddClass("player-use-item-action", new PlayerUseItemAction());
            alias.AddClass("player-remove-item-action", new PlayerRemoveItemAction());
            alias.AddClass("set-item-count-action", new SetItemCountAction());
            alias.AddClass("select-item-action", new SelectItemAction());
            alias.AddClass("move-item-action", new ItemMoveAction());
            alias.AddClass("drop-item-action", new DropItemToSceneAction());
            alias.AddClass("create-inventory-action", new CreateInventoryAction());
            alias.AddClass("pos-hotkey", new PositionHotKey());
            alias.AddClass("condition-key", new ConditionHotKey());
            alias.AddClass("inventory-ui", new SimpleInventoryUI());
            alias.AddClass("back-ui", new SimpleBackUI());
            alias.AddClass("item-ui", new SimpleItemUI());
            alias.AddClass("open-inventory-action", new OpenInventoryAction());
            alias.AddClass("resort-inventory-action", new ResortInventoryAction());
            alias.AddField("free-game-item", "enterAction", "enter-action");
            alias.AddField("free-game-item", "leaveAction", "leave-action");
            alias.AddField("create-item-player-action", "failAction", "fail-action");
            alias.AddField("create-inventory-action", "inventoryUI", "inventory-ui");
            alias.AddField("create-inventory-action", "openAction", "open-action");
            alias.AddField("create-inventory-action", "closeAction", "close-action");
            alias.AddField("create-inventory-action", "dropAction", "drop-action");
            alias.AddField("create-inventory-action", "hotkey", "hotkey");
            alias.AddField("inventory-ui", "backUI", "back-ui");
            alias.AddField("inventory-ui", "itemUI", "item-ui");
            alias.AddField("inventory-ui", "errorAction", "error-action");
            alias.AddField("inventory-ui", "moveOutAction", "move-out-action");
            alias.AddField("inventory-ui", "canNotMoveAction", "can-not-move-action");
            alias.AddField("inventory-ui", "moveAction", "move-action");
            alias.AddAttribue("free-game-item", "cat", "cat");
            alias.AddAttribue("free-game-item", "time", "time");
            alias.AddAttribue("create-item-player-action", "name", "name");
            alias.AddAttribue("create-item-player-action", "key", "key");
            alias.AddAttribue("create-item-player-action", "count", "count");
            alias.AddAttribue("create-item-scene-action", "key", "key");
            alias.AddAttribue("create-item-scene-action", "time", "time");
            alias.AddAttribue("create-item-scene-action", "count", "count");
            alias.AddAttribue("player-drop-item-action", "cat", "cat");
            alias.AddAttribue("player-use-item-action", "item", "item");
            alias.AddAttribue("player-remove-item-action", "exp", "exp");
            alias.AddAttribue("player-remove-item-action", "item", "item");
            alias.AddAttribue("player-remove-item-action", "count", "count");
            alias.AddAttribue("set-item-count-action", "exp", "exp");
            alias.AddAttribue("set-item-count-action", "count", "count");
            alias.AddAttribue("select-item-action", "exp", "exp");
            alias.AddAttribue("select-item-action", "count", "count");
            alias.AddAttribue("select-item-action", "item", "item");
            alias.AddAttribue("select-item-action", "id", "id");
            alias.AddAttribue("select-item-action", "usePosition", "use-position");
            alias.AddAttribue("move-item-action", "x", "x");
            alias.AddAttribue("move-item-action", "y", "y");
            alias.AddAttribue("move-item-action", "item", "item");
            alias.AddAttribue("move-item-action", "toInventory", "to-inventory");
            alias.AddAttribue("drop-item-action", "item", "item");
            alias.AddAttribue("drop-item-action", "time", "time");
            alias.AddAttribue("create-inventory-action", "name", "name");
            alias.AddAttribue("create-inventory-action", "column", "column");
            alias.AddAttribue("create-inventory-action", "row", "row");
            alias.AddAttribue("create-inventory-action", "canDrop", "can-drop");
            alias.AddAttribue("condition-key", "condition", "condition");
            alias.AddAttribue("condition-key", "key", "key");
            alias.AddAttribue("condition-key", "ui", "ui");
            alias.AddAttribue("inventory-ui", "x", "x");
            alias.AddAttribue("inventory-ui", "y", "y");
            alias.AddAttribue("inventory-ui", "relative", "relative");
            alias.AddAttribue("inventory-ui", "width", "width");
            alias.AddAttribue("inventory-ui", "height", "height");
            alias.AddAttribue("inventory-ui", "itemWidth", "item-width");
            alias.AddAttribue("inventory-ui", "itemHeight", "item-height");
            alias.AddAttribue("inventory-ui", "nomouse", "nomouse");
            alias.AddAttribue("inventory-ui", "alwaysShow", "always-show");
            alias.AddAttribue("inventory-ui", "itemFixed", "item-fixed");
            alias.AddAttribue("back-ui", "background", "background");
            alias.AddAttribue("item-ui", "back", "back");
            alias.AddAttribue("item-ui", "item", "item");
            alias.AddAttribue("item-ui", "count", "count");
            alias.AddAttribue("item-ui", "hotkey", "hotkey");
            alias.AddAttribue("item-ui", "used", "used");
            alias.AddAttribue("item-ui", "notused", "not-used");
            alias.AddAttribue("open-inventory-action", "alwaysClose", "close");
            alias.AddAttribue("open-inventory-action", "alwaysOpen", "open");
            alias.AddAttribue("open-inventory-action", "name", "name");
            alias.AddAttribue("resort-inventory-action", "inventory", "inventory");
            alias.AddAttribue("resort-inventory-action", "order", "order");
        }

    }
}
