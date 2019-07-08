using App.Client.CastObjectUtil;
using App.Client.GameModules.GamePlay.Free.UI;
using App.Client.Utility;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.App.Client.GameModules.GamePlay.Free;
using Assets.Sources.Free;
using Assets.Sources.Free.Auto;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Assets.XmlConfig;
using Utils.AssetManager;
using Core.Free;
using Core.Utils;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UIComponent.UI.Manager;
using App.Client.GameModules.GamePlay.Free.App;
using Utils.Singleton;
using Assets.Utils.Configuration;

namespace App.Client.GameModules.GamePlay.Free.Auto.Prefab
{
    public class AutoSimpleBag : IAutoValue
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(AutoSimpleBag));

        private bool started;
        private int radius;
        private int bagBlockKeyId;
        private int bagBlockPointerId;

        private long lastUpdateTime;

        public const string GroundParentPath = "GrounditemPanel/Scroll View/Viewport/Content";
        public const string BagParentPath = "SelfitemPanel/Scroll View/Viewport/Content";

        private static UIAddChildMessageHandler handler = new UIAddChildMessageHandler();
        private HashSet<int> groundEntitySet = new HashSet<int>();
        private Dictionary<int, ItemBar> groundDic = new Dictionary<int, ItemBar>();

        private HashSet<int> bagEntitySet = new HashSet<int>();
        private List<ItemBar> bagDic = new List<ItemBar>();

        private Dictionary<int, string> imgDic = new Dictionary<int, string>();

        private GameObject parentGround;
        private GameObject parentBag;

        public static SimpleProto CurrentBag;

        private SimpleProto lastBag;

        private bool isOpen;

        private bool initialed;

        public bool Started
        {
            get
            {
                return started;
            }
        }

        public object Frame(int frameTime)
        {
            if (parentGround == null)
            {
                parentGround = UnityUiUtility.FindUIObject(GroundParentPath);
            }

            if (parentBag == null)
            {
                parentBag = UnityUiUtility.FindUIObject(BagParentPath);
            }

            InitialBag();

            SimpleFreeUI bag = SingletonManager.Get<FreeUiManager>().GetUi("bagSystemUI");
            if (bag.Visible != isOpen)
            {
                if (!bag.Visible)
                {
                    SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.IsShowCrossHair = true;
                    SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.UnblockKey(bagBlockKeyId);
                    SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.UnblockPointer(bagBlockPointerId);
                }
                else
                {
                    SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.IsShowCrossHair = false;
                    bagBlockKeyId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockKey(UserInputManager.Lib.Layer.Ui);
                    bagBlockPointerId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockPointer(UserInputManager.Lib.Layer.Ui);
                }
                isOpen = bag.Visible;

                if (CursorLocker.SystemUnlock)
                {
                    //Unlock
                    SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.IsShowCrossHair = false;
                    CursorLocker.SystemBlockKeyId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockKey(UserInputManager.Lib.Layer.System);
                    CursorLocker.SystemBlockPointerId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockPointer(UserInputManager.Lib.Layer.System);
                }
            }

            if (bag.Visible)
            {
                FreePrefabLoader.CacheGameObject(new AssetInfo("ui/client/prefab/chicken", "ItemBar"), 50);
                FreePrefabLoader.CacheGameObject(new AssetInfo("ui/client/prefab/chicken", "CaseNameBar"), 5);

                if (parentGround != null && parentBag != null && DateTime.Now.Ticks - lastUpdateTime > 10000)
                {
                    lastUpdateTime = DateTime.Now.Ticks;

                    Contexts context = SingletonManager.Get<FreeUiManager>().Contexts1;

                    PlayerEntity player = context.player.flagSelfEntity;

                    RefreshGround(context, player);
                    RefreshBag(context, player);
                }
            }
            return null;
        }

        private void InitialBag()
        {
            if (!initialed)
            {
                if (parentBag != null && parentGround != null)
                {
                    Transform[] ts = parentBag.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in ts)
                    {
                        if (t.parent == parentBag.transform)
                        {
                            t.SetParent(null);
                        }
                    }

                    ts = parentGround.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in ts)
                    {
                        if (t.parent == parentGround.transform)
                        {
                            t.SetParent(null);
                        }
                    }

                    initialed = true;
                }
                else
                {
                    Debug.Log("wait ui to load");
                }

            }
        }

        private void ReturnObject(ItemBar bar)
        {
            FreePrefabComponent prefab = bar.prefab;
            GameObject obj = (GameObject)prefab.currentObject;
            if (obj != null)
            {
                obj.GetComponent<Transform>().parent = null;
                FreePrefabLoader.ReturnGameObject(obj, prefab.assetInfo);

                string imgUrl = "";
                string[] fieldValues = bar.value.Split(FreeMessageConstant.SpliterRecord);

                foreach (string fieldValue in fieldValues)
                {
                    string[] vs = fieldValue.Split(FreeMessageConstant.SpilterField);
                    if (vs.Length == 2)
                    {
                        if (vs[0].Trim() == "IMG_WeaponIcon")
                        {
                            imgUrl = vs[1].Trim();
                        }
                    }
                }

                GameObject img = UnityUiUtility.FindUIObject(obj, "IMG_WeaponIcon");
                if (img != null && !string.IsNullOrEmpty(imgUrl))
                {
                    int last = imgUrl.Trim().LastIndexOf("/");
                    string buddleName = imgUrl.Trim().Substring(0, last);
                    string assetName = imgUrl.Trim().Substring(last + 1);
                    AssetInfo info = new AssetInfo(buddleName, assetName);

                    Graphic gra = img.GetComponent<Graphic>();
                    if (gra is Image)
                    {
                        FreeUILoader.ReturnGameObject(((Image)gra).sprite, info);
                    }
                    if (gra is RawImage)
                    {
                        FreeUILoader.ReturnGameObject(((RawImage)gra).texture, info);
                    }
                }
            }
        }

        private void RefreshBag(Contexts context, PlayerEntity player)
        {
            if (CurrentBag != null && (lastBag == null || lastBag != CurrentBag))
            {
                lastBag = CurrentBag;

                foreach (ItemBar prefab in bagDic)
                {
                    ReturnObject(prefab);
                }

                bagDic.Clear();

                List<BagItem> items = new List<BagItem>();

                for (int i = 0; i < CurrentBag.Ins[0]; i++)
                {
                    string key = CurrentBag.Ss[i * 3 + 1];
                    string eventKey = CurrentBag.Ss[i * 3 + 2];
                    string count = CurrentBag.Ss[i * 3 + 3];

                    BagItem item = new BagItem();
                    item.key = key;
                    item.eventKey = eventKey;
                    item.count = count;

                    item.cat = CurrentBag.Ins[i * 2 + 1];
                    item.id = CurrentBag.Ins[i * 2 + 2];

                    items.Add(item);
                }

                items.Sort(new BagItemSorter());

                foreach (BagItem item in items)
                {
                    ItemBar prefab = AddItem(item, item.key, item.eventKey, item.count);
                    if (prefab != null)
                    {
                        bagDic.Add(prefab);
                    }
                }

                SetWeight();
            }
        }

        private void SetWeight()
        {
            GameObject weight = UnityUiUtility.FindUIObject("SelfitemPanel/TEXT_Weight");
            if (weight != null)
            {
                Text text = weight.GetComponent<Text>();
                if (text != null)
                {
                    text.text = CurrentBag.Ks[2] + "/" + CurrentBag.Ks[1];
                }
            }

            weight = UnityUiUtility.FindUIObject("GrounditemPanel/TEXT_CaseName");
            if (weight != null)
            {
                Text text = weight.GetComponent<Text>();
                if (text != null)
                {
                    text.text = "";
                }
            }
        }

        private bool HasNoObstacle(SceneObjectEntity item, PlayerEntity player)
        {
            if (item.hasUnityObject)
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.unityObject.UnityObject);
            }
            else if (item.hasMultiUnityObject)
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.multiUnityObject.FirstAsset);
            }
            else
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, null);
            }
        }

        private bool HasNoObstacle(FreeMoveEntity item, PlayerEntity player)
        {
            if (item.hasUnityGameObject)
            {
                var noObstacle = !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.unityGameObject.UnityObject);
                return noObstacle;
            }
            else
            {
                var noObstacle = !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, null);
                return noObstacle;
            }
        }


        private void RefreshGround(Contexts context, PlayerEntity player)
        {

            HashSet<int> current = new HashSet<int>();

            foreach (var item in context.sceneObject.GetEntities())
            {
                if (item.hasPosition
                    && IsNear(item.position.Value, player.position.Value)
                    && item.hasSimpleItem && item.simpleItem.Category > 0
                    && HasNoObstacle(item, player))
                {
                    current.Add(item.entityKey.Value.EntityId);
                }
            }

            foreach (var item in context.freeMove.GetEntities())
            {
                if (item.hasPosition
                    && item.hasFreeData
                    && IsNear(item.position.Value, player.position.Value)
                    && HasNoObstacle(item, player))
                {
                    current.Add(item.entityKey.Value.EntityId);
                }
            }

            if (!current.SetEquals(groundEntitySet))
            {
                Debug.LogFormat("当前列表{0}， 上次列表{1}", ToSetString(current), ToSetString(groundEntitySet));

                List<SceneObjectEntity> list = new List<SceneObjectEntity>();
                Dictionary<string, List<FreeMoveEntity>> deadList = new Dictionary<string, List<FreeMoveEntity>>();
                Dictionary<string, List<FreeMoveEntity>> dropList = new Dictionary<string, List<FreeMoveEntity>>();

                foreach (var id in current)
                {
                    var sceneEntity = context.sceneObject.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.SceneObject));
                    if (null != sceneEntity)
                    {
                        list.Add(sceneEntity);
                        continue;
                    }
                    var freeMoveEntity = context.freeMove.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.FreeMove));
                    {
                        if (freeMoveEntity.freeData.Cat == FreeEntityConstant.DeadBox)
                        {
                            if (!deadList.ContainsKey(freeMoveEntity.freeData.Key))
                            {
                                deadList.Add(freeMoveEntity.freeData.Key, new List<FreeMoveEntity>());
                            }
                            deadList[freeMoveEntity.freeData.Key].Add(freeMoveEntity);
                        }
                        if (freeMoveEntity.freeData.Cat == FreeEntityConstant.DropBox)
                        {
                            if (!dropList.ContainsKey(freeMoveEntity.freeData.Key))
                            {
                                dropList.Add(freeMoveEntity.freeData.Key, new List<FreeMoveEntity>());
                            }
                            dropList[freeMoveEntity.freeData.Key].Add(freeMoveEntity);
                        }
                    }
                }

                //foreach (var item in context.sceneObject.GetEntities())
                //{
                //    if (item.hasPosition && IsNear(item.position.Value, player.position.Value) && item.hasSimpleItem && item.simpleEquipment.Category > 0
                //        && HasNoObstacle(item, player))
                //    {
                //        list.Add(item);
                //    }
                //}

                //foreach (var item in context.freeMove.GetEntities())
                //{
                //    if (item.hasPosition && item.hasFreeData && IsNear(item.position.Value, player.position.Value) && HasNoObstacle(item, player))
                //    {
                //        if (item.freeData.Cat == FreeEntityConstant.DeadBox)
                //        {
                //            if (!deadList.ContainsKey(item.freeData.Key))
                //            {
                //                deadList.Add(item.freeData.Key, new List<FreeMoveEntity>());
                //            }
                //            deadList[item.freeData.Key].Add(item);
                //        }
                //        if (item.freeData.Cat == FreeEntityConstant.DropBox)
                //        {
                //            if (!dropList.ContainsKey(item.freeData.Key))
                //            {
                //                dropList.Add(item.freeData.Key, new List<FreeMoveEntity>());
                //            }
                //            dropList[item.freeData.Key].Add(item);
                //        }
                //    }
                //}

                foreach (ItemBar prefab in groundDic.Values)
                {
                    ReturnObject(prefab);
                }

                groundDic.Clear();

                FillBox(dropList, groundDic, false);
                FillBox(deadList, groundDic, true);

                list.Sort(new SceneObjectSorter());

                if (list.Count > 0)
                {
                    ItemBar caseName = AddCaseName("地面");
                    if (caseName != null)
                    {
                        groundDic.Add(-1, caseName);
                    }
                }
                foreach (var item in list)
                {
                    if (item.simpleItem.Category == (int)ECategory.Weapon)
                    {
                        WeaponConfigNs.WeaponResConfigItem weapon = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(item.simpleItem.Id);
                        if (weapon.Type != (int)EWeaponType_Config.ThrowWeapon)
                        {
                            item.simpleItem.Count = 0;
                        }
                    }
                    if (item.simpleItem.Category == (int)ECategory.Avatar)
                    {
                        item.simpleItem.Count = 0;
                    }

                    ItemBar prefab = AddChild(item.simpleItem.Category, item.simpleItem.Id, item.simpleItem.Count, item.entityKey.Value.EntityId);

                    if (prefab != null)
                    {
                        groundDic.Add(item.entityKey.Value.EntityId, prefab);
                    }
                }

                groundEntitySet = current;
            }
        }

        private void FillBox(Dictionary<string, List<FreeMoveEntity>> dic, Dictionary<int, ItemBar> cache, bool dead)
        {
            int index = 0;
            foreach (var name in dic.Keys)
            {
                ItemBar caseName = AddCaseName(name);
                if (caseName != null)
                {
                    if (dead)
                    {
                        cache.Add(-100 + index++, caseName);
                    }
                    else
                    {
                        cache.Add(-200 + index++, caseName);
                    }
                }

                List<SimpleItemInfo> infos = new List<SimpleItemInfo>();
                foreach (var item in dic[name])
                {
                    object obj = SingletonManager.Get<DeadBoxParser>().FromString(item.freeData.Value);
                    if (obj == null)
                    {
                        continue;
                    }

                    SimpleItemInfo info = (SimpleItemInfo)obj;
                    info.entityId = item.entityKey.Value.EntityId;

                    infos.Add(info);
                }

                infos.Sort(new ItemInfoSorter());

                foreach (SimpleItemInfo info in infos)
                {
                    int count = info.count;
                    if (info.cat == (int)ECategory.Weapon)
                    {
                        count = 0;
                    }
                    if (info.cat == (int)ECategory.Avatar)
                    {
                        count = 0;
                    }
                    ItemBar prefab = AddChild(info.cat, info.id, count, info.entityId);

                    if (prefab != null)
                    {
                        cache.Add(info.entityId, prefab);
                    }
                }
            }
        }

        private string ToSetString(HashSet<int> set)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            foreach (int i in set)
            {
                sb.Append(i);
                sb.Append(",");
            }
            sb.Append(']');

            return sb.ToString();
        }

        private ItemBar AddCaseName(string name)
        {
            ItemBar bar = new ItemBar();
            SimpleProto model = SingletonManager.Get<FreeUiManager>().GetUIData("caseName");
            if (model == null)
            {
                Debug.LogWarningFormat("item 'caseName' is not defined.");
                return null;
            }

            FreePrefabComponent ff = new FreePrefabComponent();
            ff.parentObject = parentGround;

            ff.Initial(model.Ss[2], model.Ss[3]);
            ff.SetValues("TEXT_CaseName" + FreeMessageConstant.SpilterField + name);
            ff.SetEvents("");

            bar.prefab = ff;
            bar.value = "";

            return bar;
        }

        private ItemBar AddItem(BagItem item, string key, string eventKey, string count)
        {
            ItemBar bar = new ItemBar();

            SimpleProto model = SingletonManager.Get<FreeUiManager>().GetUIData(key);
            if (model == null)
            {
                Debug.LogWarningFormat("item '{0}' is not defined.", key);
                return null;
            }

            FreePrefabComponent ff = new FreePrefabComponent();
            ff.parentObject = parentBag;
            int realCount = 1;
            if (!string.IsNullOrEmpty(count))
            {
                realCount = int.Parse(count);
            }
            TipUtil.AddTip(eventKey, new TipData(item.cat, item.id, realCount));

            ff.Initial(model.Ss[2], model.Ss[3]);
            ff.SetValues("TEXT_ItemNumber" + FreeMessageConstant.SpilterField + count);
            ff.SetEvents("");
            ff.SetAllEventKey(eventKey);

            bar.prefab = ff;
            bar.value = model.Ss[2].Split(FreeMessageConstant.SpliterStyle)[1];

            return bar;
        }

        private ItemBar AddChild(int cat, int id, int count, int entityId)
        {
            ItemBar bar = new ItemBar();
            string key = (cat * 10000 + id) + "_itemUI";

            SimpleProto model = SingletonManager.Get<FreeUiManager>().GetUIData(key);
            if (model == null)
            {
                Debug.LogWarningFormat("item '{0}' is not defined.", key);
                return null;
            }

            FreePrefabComponent ff = new FreePrefabComponent();
            ff.parentObject = parentGround;
            TipUtil.AddTip("ground,0," + entityId, new TipData(cat, id, count));

            ff.Initial(model.Ss[2], model.Ss[3]);
            if (count > 0)
            {
                ff.SetValues("TEXT_ItemNumber" + FreeMessageConstant.SpilterField + count);
            }
            else
            {
                ff.SetValues("TEXT_ItemNumber" + FreeMessageConstant.SpilterField + " ");
            }

            ff.SetEvents("");
            ff.SetAllEventKey("ground,0," + entityId);

            bar.prefab = ff;
            bar.value = model.Ss[2].Split(FreeMessageConstant.SpliterStyle)[1];

            return bar;
        }

        private void Handle(SimpleProto value, SimpleProto model, GameObject parentObj)
        {
            for (var i = 0; i < model.Ks.Count - 1; i++)
            {
                var newPo = FreeUIUtil.GetInstance().GetComponent(model.Ks[i + 1]);
                if (newPo == null)
                {
                    Logger.ErrorFormat("Free component not exist {0}", model.Ks[i + 1]);
                    continue;
                }

                newPo.Initial(model.Ss[i * 3 + 2], model.Ss[i * 3 + 3]);

                if (newPo is FreePrefabComponent)
                {
                    FreePrefabComponent ff = (FreePrefabComponent)newPo;
                    ff.SetValues(value.Ss[2]);
                    ff.SetEvents(value.Ss[3]);
                    ff.SetAllEventKey(value.Ss[4]);

                    ff.parentObject = parentObj;
                }
            }
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split('|');
            if (ss[0] == "bagauto" && ss.Length >= 2)
            {
                AutoSimpleBag auto = new AutoSimpleBag();
                auto.radius = int.Parse(ss[1].Trim());

                return auto;
            }

            return null;
        }

        private bool IsNear(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.x - v2.x) < radius && Math.Abs(v1.z - v2.z) < radius && Math.Abs(v1.y - v2.y) < 2;
        }

        public void SetValue(params object[] value)
        {

        }

        public void Start()
        {
            started = true;
        }

        public void Stop()
        {
            started = false;
        }

        public static int CompareItem(int cat, int id, int cat1, int id1)
        {
            return GetOrder(cat1, id1) - GetOrder(cat, id);
        }

        private static int GetOrder(int cat, int id)
        {
            if (cat == 13)
            {
                if (id >= 7 && id <= 12)
                {
                    return cat * 10000 + id * 100;
                }
                else
                {
                    return cat * 10000 + id;
                }
            }

            return cat * 10000 + id;
        }
    }

    class ItemBar
    {
        public FreePrefabComponent prefab;
        public string value;
    }

    class SceneObjectSorter : Comparer<SceneObjectEntity>
    {
        public override int Compare(SceneObjectEntity x, SceneObjectEntity y)
        {
            return AutoSimpleBag.CompareItem(x.simpleItem.Category, x.simpleItem.Id, y.simpleItem.Category, y.simpleItem.Id);
        }
    }

    public class BagItem
    {
        public int cat;
        public int id;
        public string eventKey;
        public string key;
        public string count;
    }

    class BagItemSorter : Comparer<BagItem>
    {
        public override int Compare(BagItem x, BagItem y)
        {
            return AutoSimpleBag.CompareItem(x.cat, x.id, y.cat, y.id);
        }
    }

    class ItemInfoSorter : Comparer<SimpleItemInfo>
    {
        public override int Compare(SimpleItemInfo x, SimpleItemInfo y)
        {
            return AutoSimpleBag.CompareItem(x.cat, x.id, y.cat, y.id);
        }
    }
}
