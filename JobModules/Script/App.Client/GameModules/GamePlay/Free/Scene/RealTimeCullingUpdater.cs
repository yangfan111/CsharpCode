using App.Shared;
using Assets.Sources.Free;
using Core.SpatialPartition;
using Core.Utils;
using Sharpen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Scene
{
    public class RealTimeCullingUpdater : DisposableSingleton<RealTimeCullingUpdater>
    {
        private static LoggerAdapter _logger = new LoggerAdapter("FrameTest");

        private const string SceneName = "002";
        private Map2D<GameObject> map;
        private Map2D<GameObject> smallMap;
        private Map2D<GameObject> MidMap;

        private BitArray recSet;
        private BitArray lastRecSet;

        private BitArray lastSet;
        private BitArray currentSet;

        private BitArray lastSetS;
        private BitArray currentSetS;

        private BitArray lastSetM;
        private BitArray currentSetM;

        private long lastRenderTime;
        private long lastIniTime;

        private Contexts contexts;
        private int[] countDic;
        private int MaxLen;

        //private GameObject[] roots;

        public void Initial(Contexts contexts)
        {
            this.contexts = contexts;

            Bin2DConfig config = new Bin2DConfig(-4000, -4000, 4000, 4000, 50, 50);
            map = new Map2D<GameObject>(config);
            smallMap = new Map2D<GameObject>(config);
            MidMap = new Map2D<GameObject>(config);

            InternalBounds bouds = map.GetInternalBounds(new Rect(-4000, -4000, 8000, 8000));
            MaxLen = bouds.MaxX - bouds.MinX + 1;

            recSet = new BitArray(100);
            lastRecSet = new BitArray(100);
            countDic = new int[100];

            lastSet = new BitArray(MaxLen * MaxLen);
            currentSet = new BitArray(MaxLen * MaxLen);
            lastSetS = new BitArray(MaxLen * MaxLen);
            currentSetS = new BitArray(MaxLen * MaxLen);
            lastSetM = new BitArray(MaxLen * MaxLen);
            currentSetM = new BitArray(MaxLen * MaxLen);

            //roots = new GameObject[MaxLen * MaxLen];

            //for (int i = 0; i < MaxLen * MaxLen; i++)
            //{
            //   roots[i] = new GameObject();
            //   roots[i].name = "root" + i;
            // }
        }

        public void Update()
        {
            long startTime = DateTime.Now.Ticks;

            if (!SharedConfig.CullingOn)
            {
                return;
            }
            if (contexts.session.commonSession.RoomInfo.MapId!= 2)
            {
                return;
            }
            if (Runtime.CurrentTimeMillis() - lastIniTime > 10000)
            {
                InitialAllScene();

                lastIniTime = Runtime.CurrentTimeMillis();
            }

            if (Runtime.CurrentTimeMillis() - lastRenderTime > SharedConfig.CullingInterval)
            {
                Culling(smallMap, currentSetS, lastSetS, SharedConfig.CullingRangeSmall, "small");
                Culling(MidMap, currentSetM, lastSetM, SharedConfig.CullingRangeMid, "mid");
                Culling(map, currentSet, lastSet, SharedConfig.CullingRange, "bud");

                lastRenderTime = Runtime.CurrentTimeMillis();
            }

            if (DateTime.Now.Ticks - startTime > 10000)
            {
                _logger.InfoFormat("culling all time {0}", (DateTime.Now.Ticks - startTime) / 10000);
            }
        }

        private int showCount = 0;
        private int hideCount = 0;

        private void Culling(Map2D<GameObject> map, BitArray currentSet, BitArray lastSet, int range, string type)
        {
            long startTime = DateTime.Now.Ticks;

            showCount = 0;
            hideCount = 0;

            PlayerEntity player = contexts.player.flagSelfEntity;
            InternalBounds bounds = map.GetInternalBounds(new Rect(player.position.Value.x - range / 2,
                player.position.Value.z - range / 2, range, range));

            currentSet.SetAll(false);

            for (int i = bounds.MinX; i <= bounds.MaxX; i++)
            {
                for (int j = bounds.MinY; j <= bounds.MaxY; j++)
                {
                    currentSet.Set(GetIndex(i, j), true);
                }
            }

            //_logger.InfoFormat("culling all {4} rect {0}-{1},{2}-{3} at {5},{6}\nalready show:\n", bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY, MaxLen, player.position.Value.x, player.position.Value.z);
            //printNotEmpty(lastSet);

            for (int i = 0; i < currentSet.Length; i++)
            {
                if (currentSet.Get(i) && !lastSet.Get(i))
                {
                    ShowOne(map, i, true, type);
                }
                if (!currentSet.Get(i) && lastSet.Get(i))
                {
                    ShowOne(map, i, false, type);
                }
            }

            lastSet.SetAll(false);
            lastSet = lastSet.Or(currentSet);

            if (DateTime.Now.Ticks - startTime > 10000 && (showCount > 0 || hideCount > 0))
            {
                _logger.InfoFormat("culling time {0}, show count={1}, hide count={2}, center={3},{4}", (DateTime.Now.Ticks - startTime) / 10000,
                    showCount, hideCount, (bounds.MinX + bounds.MaxX) / 2, (bounds.MinY + bounds.MaxY) / 2);
            }
        }

        protected  override void OnDispose()
        {
            
        }

        private void printNotEmpty(BitArray set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (set.Get(i))
                {
                    int x = i / MaxLen;
                    int y = i % MaxLen;
                    _logger.InfoFormat("culling alread show {0},{1}", x, y);
                }
            }
        }

        private void ShowOne(Map2D<GameObject> map, int index, bool show, string type)
        {
            int x = index / MaxLen;
            int y = index % MaxLen;

            //GameObject root = roots[x * MaxLen + y];

            if (show)
            {
                //LinkedList<GameObject> list = map.Get(x, y);
                //if (list != null && list.Count > 0)
                //{
                //    StaticBatchingUtility.Combine(list.ToArray(), root);
                //}

                int count = map.HandleCell(x, y, ShowGameObject);
                //root.SetActive(true);
                if (count > 10)
                {
                    _logger.DebugFormat("culling show {0},{1} count={2}, type={3}", x, y, count, type);
                }

            }
            else
            {
                int count = map.HandleCell(x, y, HideGameObject);
                //root.SetActive(false);
                if (count > 10)
                {
                    _logger.DebugFormat("culling hide {0},{1} count={2}, type={3}", x, y, count, type);
                }
            }
        }

        private void ShowGameObject(GameObject obj)
        {
            try
            {
                if (obj != null && !obj.activeSelf)
                {
                    showCount++;
                    obj.SetActive(true);
                    Transform p = obj.transform.parent;
                    while (p != null)
                    {
                        p.gameObject.SetActive(true);
                        p = p.parent;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("culling show error {0}", e.Message);
            }

        }

        private void HideGameObject(GameObject obj)
        {
            try
            {
                if (obj != null && obj.activeSelf)
                {
                    hideCount++;
                    obj.SetActive(false);
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("culling hide error {0}", e.Message);
            }
        }


        private int GetIndex(int x, int y)
        {
            return x * MaxLen + y;
        }

        private void InitialAllScene()
        {
            bool same = true;
            recSet.SetAll(false);
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name.StartsWith(SceneName) && scene.isLoaded)
                {
                    int index = GetSceneIndex(scene.name);
                    recSet.Set(index, true);
                    //if (Math.Abs(countDic[index] - GetAllChildCount(scene)) > 100)
                    //{
                    //    same = false;
                    //}
                    //countDic[index] = GetAllChildCount(scene);
                }
            }

            if (same)
            {
                for (int i = 0; i < recSet.Length; i++)
                {
                    if (recSet.Get(i) && !lastRecSet.Get(i))
                    {
                        same = false;
                        break;
                    }
                }
            }

            if (!same)
            {
                map.Clear();
                smallMap.Clear();
                MidMap.Clear();
                lastSet.SetAll(true);
                lastSetM.SetAll(true);
                lastSetS.SetAll(true);
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.name.StartsWith(SceneName) && scene.isLoaded)
                    {
                        _logger.InfoFormat("culling ini scene {0}", scene.name);
                        InitialOneScene(scene);
                    }
                }
            }

            lastRecSet.SetAll(false);
            lastRecSet = lastRecSet.Or(recSet);
        }

        private int GetAllChildCount(UnityEngine.SceneManagement.Scene scene)
        {
            int c = 0;
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                GameObject obj = roots[i];
                c += obj.GetComponentsInChildren<Transform>(true).Length;
            }

            return c;
        }

        private Dictionary<string, int> sceneDic = new Dictionary<string, int>();

        private int GetSceneIndex(string scene)
        {
            if (!sceneDic.ContainsKey(scene))
            {
                string[] ss = scene.Replace(SceneName, "").Trim().Split('x');
                if (ss.Length == 2)
                {
                    int x = int.Parse(ss[0]);
                    int y = int.Parse(ss[1]);

                    sceneDic[scene] = x * 10 + y;
                }
            }

            return sceneDic[scene];
        }

        private void InitialOneScene(UnityEngine.SceneManagement.Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            int total = 0;
            for (int i = 0; i < roots.Length; i++)
            {
                GameObject obj = roots[i];
                UnityEngine.Terrain t = obj.GetComponent<UnityEngine.Terrain>();
                if (t == null)
                {
                    Transform[] trans = obj.GetComponentsInChildren<Transform>(true);
                    for (int j = 0; j < trans.Length; j++)
                    {
                        GameObject child = trans[j].gameObject;

                        Vector2 pos = new Vector2(trans[j].position.x, trans[j].position.z);

                        if (Math.Abs(pos.x) < 4000 && Math.Abs(pos.y) < 4000 && pos.magnitude > 1)
                        {
                            try
                            {
                                if (child.name.StartsWith("Prop") || child.name.ToLower().StartsWith("stone") || child.name.StartsWith("prop"))
                                {
                                    if (child.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.SmallthingNear))
                                    {
                                        smallMap.Insert(child, pos);
                                        c1++;
                                    }
                                    else
                                    {
                                        MidMap.Insert(child, pos);
                                        c2++;
                                    }
                                }
                                else
                                {
                                    map.Insert(child, pos);
                                    c3++;
                                }
                                //child.SetActive(false);
                                total++;
                            }
                            catch (Exception e)
                            {
                                _logger.ErrorFormat("culling insert error {0}", e.Message);
                            }
                        }
                    }
                }
            }

            _logger.InfoFormat("culling insert t={4}, s={0},m={1},b={2} at scene {3}", c1, c2, c3, scene.name, total);
        }
    }
}
