using App.Shared;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using DG.Tweening;
using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Shared.Components.Ui;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace App.Client.GameModules.Ui.Common.MaxMap
{
    public class DuquanCommon : AbstractModel
    {
        public DuquanCommon(IMiniMapUiAdapter _adapter) { this.adapter = _adapter; }
        private IMiniMapUiAdapter adapter = null;

        public DuQuanInfo curDuquan = null;
        public DuQuanInfo safeDuquan = null;
        public BombAreaInfo curBombAreaInfo = null;
        private float lineWidth = 1.5f;                 //毒圈、安全区的线条宽度

        private Action UpdateSafe = null;          
        private Action UpdateDuquan = null;
        private Action UpdataMiniDis = null;
        private Action UpdateBombArea = null;

        public void SetActions(Action _UpdateSafe, Action _UpdateDuquan, Action _UpdataMiniDis, Action _UpdateBombArea)
        {
            this.UpdateSafe = _UpdateSafe;
            this.UpdateDuquan = _UpdateDuquan;
            this.UpdataMiniDis = _UpdataMiniDis;
            this.UpdateBombArea = _UpdateBombArea;
        }

        public void UpdateDuQuanAndSafe(Transform curDuquanRoot, Transform safeDuquanRoot, Transform miniDisTran, Transform curBombAreaRoot)
        { 
            var data = adapter;
            if (data != null)
            {
                curDuquan = data.CurDuquan;
                safeDuquan = data.NextDuquan;

                if (SharedConfig.IsOffline)
                {
                    UIUtils.SetActive(curDuquanRoot, true);
                    UIUtils.SetActive(safeDuquanRoot, true);
                    UIUtils.SetActive(miniDisTran, true);
                    if(UpdateSafe != null)
                        UpdateSafe();
                    if(UpdateDuquan != null)
                        UpdateDuquan();
                    if(UpdataMiniDis != null)
                    UpdataMiniDis();
                }
                else
                {
                    if (curDuquan.Level != adapter.OffLineNum && curDuquan.Radius != 0)
                    {
                        UIUtils.SetActive(curDuquanRoot, true);
                        if (UpdateDuquan != null)  UpdateDuquan();
                    }
                    else
                        UIUtils.SetActive(curDuquanRoot, false);

                    if (safeDuquan.Level != adapter.OffLineNum && safeDuquan.Radius != 0)
                    {
                        UIUtils.SetActive(safeDuquanRoot, true);
                        if (UpdateSafe != null) UpdateSafe();
                    }
                    else
                        UIUtils.SetActive(safeDuquanRoot, false);

                    if (safeDuquan.Level != adapter.OffLineNum && safeDuquan.Radius != 0)
                    {
                        UIUtils.SetActive(miniDisTran, true);
                        if(UpdataMiniDis!= null) UpdataMiniDis();
                    }
                    else
                        UIUtils.SetActive(miniDisTran, false);
                }

                //轰炸区
                if (curBombAreaInfo == null || data.BombArea.Num != curBombAreaInfo.Num)
                    curBombAreaInfo = data.BombArea;

                if (SharedConfig.IsOffline)
                {
                    UIUtils.SetActive(curBombAreaRoot, true);
                    if(UpdateBombArea!= null) UpdateBombArea();
                }
                else
                {
                    if (curBombAreaInfo.Num != -1 && curBombAreaInfo.Radius != 0)
                    {
                        UIUtils.SetActive(curBombAreaRoot, true);
                        if (UpdateBombArea != null)  UpdateBombArea();
                    }
                    else
                    {
                        UIUtils.SetActive(curBombAreaRoot, false);
                    }
                }
            }
        }
        public void UpdateSafeCommon(bool isMiniMap, Transform safeDuquanRoot, Vector2 refePosByRice, Vector2 referPosByPixel, float windowWidthByRice, float rate)
        {
            if (safeDuquanRoot == null)
                return;
            var rectT = safeDuquanRoot.GetComponent<RectTransform>();
            var imgCom = safeDuquanRoot.GetComponent<Image>();
            if (rectT == null || imgCom == null)
                return;

            if (UnityEngine.Vector2.Distance(refePosByRice, safeDuquan.Center) > 1.414f * windowWidthByRice / 2 + safeDuquan.Radius) //不在地图视野内
            {
                UIUtils.SetActive(safeDuquanRoot, false);
            }
            else
            {
                UIUtils.SetActive(safeDuquanRoot, true);
                var material = imgCom.material;
                //设置大小
                float beishu = (safeDuquan.Radius * rate) / (rectT.rect.width / 2);
                float beishuDaosu = (rectT.rect.width / 2) / (safeDuquan.Radius * rate);
                var tilingX = Mathf.Max(beishuDaosu, 0);
                float tilingY = tilingX;
                material.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));

                //设置位置    
                Vector2 startPoint = referPosByPixel + (safeDuquan.Center - refePosByRice) * rate;
                var halfW = rectT.rect.width / 2;
                Vector2 endPoint = referPosByPixel - new Vector2(halfW, halfW) + new Vector2(beishu * halfW, beishu * halfW);
                var deltaX = (endPoint.x - startPoint.x) / (beishu * rectT.rect.width);
                var deltaY = (endPoint.y - startPoint.y) / (beishu * rectT.rect.width);
                var safeDelat = new Vector2(deltaX, deltaY);
                material.SetTextureOffset("_MainTex", safeDelat);

                UnityEngine.Color boundColor = UnityEngine.Color.white;
                if (isMiniMap)
                    boundColor = new Color(boundColor.r, boundColor.g, boundColor.b, 0.58f);
               
                material.SetColor("_BoundColor", boundColor);
                material.SetFloat("_BoundWidth", lineWidth * beishuDaosu);
                material.SetFloat("_ComponentWidth", rectT.rect.width);
            }
        }
        public void UpateDuquanCommon(bool isMiniMap, Transform curDuquanRoot, Vector2 refePosByRice, Vector2 referPosByPixel, float windowWidthByRice, float rate)
        {
            if (curDuquanRoot == null)
                return;
            var rectT = curDuquanRoot.GetComponent<RectTransform>();
            var imgCom = curDuquanRoot.GetComponent<Image>();
            if (rectT == null || imgCom == null)
                return;

            if (Vector2.Distance(refePosByRice, curDuquan.Center) > 1.414f * windowWidthByRice / 2 + curDuquan.Radius) //不在小地图视野内
            {
                UIUtils.SetActive(curDuquanRoot, false);
            }
            else
            {
                UIUtils.SetActive(curDuquanRoot, true);
                var material = imgCom.material;

                {
                    //设置大小
                    float beishu = (curDuquan.Radius * rate) / (rectT.rect.width / 2);
                    float beishuDaosu = (rectT.rect.width / 2) / (curDuquan.Radius * rate);
                    var tilingX = Mathf.Max(beishuDaosu, 0);
                    float tilingY = tilingX;
                    var temper = new Vector2(tilingX, tilingY);
                    material.SetTextureScale("_MainTex", temper);

                    //设置位置
                    Vector2 startPoint = referPosByPixel + (curDuquan.Center - refePosByRice) * rate;
                    var halfW = rectT.rect.width / 2;
                    Vector2 endPoint = referPosByPixel - new Vector2(halfW, halfW) + new Vector2(beishu * halfW, beishu * halfW);
                    var deltaX = (endPoint.x - startPoint.x) / (beishu * rectT.rect.width);
                    var deltaY = (endPoint.y - startPoint.y) / (beishu * rectT.rect.width);
                    var curDuquanDelat = new Vector2(deltaX, deltaY);
                    material.SetTextureOffset("_MainTex", curDuquanDelat);


                    UnityEngine.Color boundColor = Color.white;
                    if (isMiniMap)
                    {
                        boundColor = new Color(91 / 255f, 255 / 255f, 20 / 255f, 0.58f);
                    }
                    else
                    {
                        boundColor = new Color(91 / 255f, 255 / 255f, 20 / 255f, 255 / 255f);
                    }
                    boundColor = new Color(91 / 255f, 255 / 255f, 20 / 255f, 0.58f);
                    material.SetColor("_BoundColor", boundColor);
                    material.SetFloat("_BoundWidth", lineWidth * beishuDaosu);
                    material.SetFloat("_ComponentWidth", rectT.rect.width);
                }
            }
        }
        public void UpdateBombAreaCommon(bool isMiniMap, Transform curBombAreaRoot, Vector2 refePosByRice, Vector2 referPosByPixel, float windowWidthByRice, float rate)
        {
            if (curBombAreaRoot == null)
                return;
            var rectT = curBombAreaRoot.GetComponent<RectTransform>();
            var imgCom = curBombAreaRoot.GetComponent<Image>();
            if (rectT == null || imgCom == null)
                return;

            var temperVec = new Vector2(curBombAreaInfo.Center.x, curBombAreaInfo.Center.z);
            if (UnityEngine.Vector2.Distance(refePosByRice, temperVec) > 1.414f * windowWidthByRice / 2 + curBombAreaInfo.Radius) //不在小地图视野内
            {
                UIUtils.SetActive(curBombAreaRoot, false);
            }
            else
            {
                UIUtils.SetActive(curBombAreaRoot, true);
                var material = imgCom.material;
                //设置大小
                float beishu = (curBombAreaInfo.Radius * rate) / (rectT.rect.width / 2);
                float beishuDaosu = (rectT.rect.width / 2) / (curBombAreaInfo.Radius * rate);
                var tilingX = Mathf.Max(beishuDaosu, 0);
                float tilingY = tilingX;
                material.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));

                //设置位置
                Vector2 startPoint = referPosByPixel + (temperVec - refePosByRice) * rate;
                var halfW = rectT.rect.width / 2;
                Vector2 endPoint = referPosByPixel - new Vector2(halfW, halfW) + new Vector2(beishu * halfW, beishu * halfW);
                var deltaX = (endPoint.x - startPoint.x) / (beishu * rectT.rect.width);
                var deltaY = (endPoint.y - startPoint.y) / (beishu * rectT.rect.width);
                material.SetTextureOffset("_MainTex", new Vector2(deltaX, deltaY));

                UnityEngine.Color bColor = Color.white;
                if(isMiniMap)
                    bColor = new Color(255 / 255f, 9 / 255f, 5 / 255f, 102 / 255f);
                else
                    bColor = new Color(255 / 255f, 9 / 255f, 5 / 255f, 102 / 255f);
                material.SetColor("_BoundColor", bColor);
                material.SetFloat("_BoundWidth", rectT.rect.width);
                material.SetFloat("_ComponentWidth", rectT.rect.width);
            }
        }
    }

    public class GridCommon
    {
        public GridCommon(){}

        public int gridHunderdInterval = 100;
        public int gridThousandInterval = 1000;
        private string[] GridDataVertical = { "A", "B", "C", "D", "E", "F", "G", "H" };
        private string[] GridDataHorizontal = { "I", "J", "K", "L", "M", "N", "O", "P" };
        private string[] GridNumData = { "", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
        private Action<int, bool> func = null;
        public void SetAction(Action<int, bool> _func)
        {
            this.func = _func;
        }
        public string GetVerticalStr(int index)
        {
            if (index < GridDataVertical.Length)
                return GridDataVertical[index];
            return "";
        }
        public string GetHorizontalStr(int index)
        {
            if (index < GridDataHorizontal.Length)
                return GridDataHorizontal[index];
            return "";
        }

        public string GetGridNumStr(int index)
        {
            if (index < GridNumData.Length)
                return GridNumData[index];
            return "";
        }

        public void UpdateGrid(int maxW, int visibleW, Vector2 visibleCenterPos)
        {
            int columnByThousand = (int)visibleCenterPos.x / gridThousandInterval;
            int columnByHundred = (int)(visibleCenterPos.x % gridThousandInterval) / gridHunderdInterval;
            int lineNum = (int)visibleW / gridHunderdInterval;
            int maxNum = (int)maxW / gridHunderdInterval;

            //找到距离中心点 左右可视范围内的线（米单位）
            for (int i = 0; i <= lineNum / 2; i++)
            {
                var temperIndex = columnByThousand * 10 + columnByHundred - i;
                if (temperIndex >= 0 && temperIndex <= maxNum - 1)
                {
                    if(func != null)
                        func(temperIndex, true);
                }
            }

            for (int i = 0; i <= lineNum / 2; i++)
            {
                int temperIndex = columnByThousand * 10 + columnByHundred + 1 + i;
                if (temperIndex >= 0 && temperIndex <= maxNum - 1)
                {
                    if (func != null)
                        func(temperIndex, true);
                }
            }

            //找到距离中心点 上下可视范围内的线（米单位）
            int rowByThousand = (int)visibleCenterPos.y / gridThousandInterval;
            int rowByHundred = (int)(visibleCenterPos.y % gridThousandInterval) / gridHunderdInterval;
            for (int i = 0; i <= lineNum / 2; i++)
            {
                var temperIndex = rowByThousand * 10 + rowByHundred - i;
                if (temperIndex >= 0 && temperIndex <= maxNum - 1)
                {
                    if (func != null)
                        func(temperIndex, false);
                }
            }

            for (int i = 0; i <= lineNum / 2; i++)
            {
                int temperIndex = rowByThousand * 10 + rowByHundred + 1 + i;
                if (temperIndex >= 0 && temperIndex <= maxNum - 1)
                {
                    if (func != null)
                        func(temperIndex, false);
                }
            }
        }
    }

    public class PlayMarkCommn : AbstractModel
    {
        public PlayMarkCommn(IMiniMapUiAdapter _context) { this.adapter = _context; }
        private IMiniMapUiAdapter adapter = null;
        public bool isNoTeamPlayer = false;
        private Dictionary<Transform, Tween> tranCTween = new Dictionary<Transform, Tween>(); // Transform与之关联的Tween

        private Action<object> playFunc = null;
        public void SetPlayAction(Action<object> _playFunc)
        {
            playFunc = _playFunc;
        }
        public void UpdatePlayList()
        {
            var datas = adapter.TeamInfos;
            if (datas.Count > 0)
            {
                isNoTeamPlayer = (datas.Count == 1) ? true : false;
                for (int i = 0; i < datas.Count; i++)
                {
                    if(playFunc != null)
                        playFunc(datas[i]);
                }
            }
        }
        public void UpdatePlayNumAndColor(Transform tran, ref Vector2 referPos, MiniMapTeamPlayInfo data, bool isMimiMap)
        {
            var numberBg = tran.Find("bg");
            if (numberBg)
            {
                //刷新编号
                var number = numberBg.Find("number");
                if (number)
                {
                    {
                        number.gameObject.SetActive(true);
                        var numberText = number.GetComponent<Text>();
                        if (numberText)
                        {
                            if (data.IsPlayer)
                            {
                                if (isNoTeamPlayer)
                                {
                                    numberText.text = "";
                                }
                                else
                                {
                                    numberText.text = data.Num.ToString();
                                }
                            }
                            else
                                numberText.text = data.Num.ToString();
                        }
                    }
                }

                var numberBgCom = numberBg.GetComponent<Image>();
                var stateIcon = numberBg.Find("icon");
                var loftIcon = numberBg.Find("loftIcon");

                if (numberBgCom == null || stateIcon == null || loftIcon == null)
                    return;
                var stateIconCom = stateIcon.GetComponent<Image>();
                var loftIconCom = loftIcon.GetComponent<Image>();

                //刷新编号背景图
                numberBgCom.color = data.Color;

                switch (data.Statue)
                {
                    case MiniMapPlayStatue.NORMAL:    //常态
                        {
                            UIUtils.SetActive(number, !MapLevel.Min.Equals(adapter.MapLevel));
                            UIUtils.SetActive(stateIcon, false);
                            if (data.IsPlayer)
                            {
                                UIUtils.SetActive(loftIcon, false);
                            }
                            else
                            {
                                UIUtils.SetActive(loftIcon, true);
                                var temperSprite = SpriteComon.GetInstance().GetSpriteByName("Loft_icon");
                                if (temperSprite != null && loftIconCom.sprite != temperSprite)
                                {
                                    loftIconCom.sprite = temperSprite;
                                }

                                if (data.Pos.y > referPos.y)       //上方
                                {
                                    if (loftIcon.transform.localScale != Vector3.one)
                                        loftIcon.transform.localScale = Vector3.one;
                                }
                                else if (data.Pos.y <= referPos.y)   //下方
                                {
                                    if (loftIcon.transform.localScale != new UnityEngine.Vector3(1, -1, 1))
                                        loftIcon.transform.localScale = new UnityEngine.Vector3(1, -1, 1);
                                }
                            }

                            if (data.ShootingCount > 0)   //在射击状态下
                            {
                               
                                if (!tranCTween.ContainsKey(tran) || tranCTween[tran] == null)
                                {
                                    var temperTween = UIUtils.CallTween(1, 1.5f, (value) => 
                                    {
                                        numberBg.GetComponent<RectTransform>().localScale = new UnityEngine.Vector3((float)value, (float)value, 1.0f);
                                    }, 
                                    (value) => 
                                    {
                                        numberBg.GetComponent<RectTransform>().localScale = Vector3.one;
                                        data.ShootingCount--;
                                        tranCTween[tran].Kill();
                                        tranCTween[tran] = null;
                                        
                                    }, 
                                    0.1f);

                                    if (!tranCTween.ContainsKey(tran))
                                    {
                                        tranCTween.Add(tran, temperTween);
                                    }
                                    else
                                        tranCTween[tran] = temperTween;
                                }
                            }
                            else
                            {
                                if (tranCTween.ContainsKey(tran) && tranCTween[tran] != null)
                                {
                                    tranCTween[tran].Kill();
                                    tranCTween[tran] = null;
                                }
                                if (numberBg.GetComponent<RectTransform>().localScale != Vector3.one)
                                    numberBg.GetComponent<RectTransform>().localScale = Vector3.one;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.TIAOSAN:    //跳伞
                        {
                            UIUtils.SetActive(number, false);
                            UIUtils.SetActive(loftIcon, false);
                            UIUtils.SetActive(stateIcon, true);

                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_parachute");
                            if (temperSprite != null && stateIconCom.sprite != temperSprite)
                            {
                                stateIconCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.ZAIJU:    //载具
                        {
                            UIUtils.SetActive(number, false);
                            UIUtils.SetActive(loftIcon, false);
                            UIUtils.SetActive(stateIcon, true);

                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_drive");
                            if (temperSprite != null && stateIconCom.sprite != temperSprite)
                            {
                                stateIconCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.HURTED:    //受伤
                        {
                            UIUtils.SetActive(number, false);
                            UIUtils.SetActive(loftIcon, false);
                            UIUtils.SetActive(stateIcon, true);

                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_hurt");
                            if (temperSprite != null && stateIconCom.sprite != temperSprite)
                            {
                                stateIconCom.sprite = temperSprite;
                            }
                        }
                        break;
                    case MiniMapPlayStatue.DEAD:    //死亡
                        {
                            UIUtils.SetActive(number, false);
                            UIUtils.SetActive(loftIcon, false);
                            UIUtils.SetActive(stateIcon, true);

                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_die");
                            if (temperSprite != null && stateIconCom.sprite != temperSprite)
                            {
                                stateIconCom.sprite = temperSprite;
                            }
                        }
                        break;
                    default:
                        {
                            UIUtils.SetActive(number, false);
                            UIUtils.SetActive(loftIcon, false);
                            UIUtils.SetActive(stateIcon, false);
                        }
                        break;
                }
            }
        }
        public void UpdatePlayFaceDirection(Transform tran, MiniMapTeamPlayInfo data)
        {
            var faceDirection = tran.Find("faceDireciton");
            if (faceDirection)
            {
                if (data.Statue != MiniMapPlayStatue.DEAD) //非死亡状态
                {
                    UIUtils.SetActive(faceDirection, true);

                    float angular = data.FaceDirection % 360;
                    angular = angular < 0 ? 360 + angular : angular;

                    faceDirection.localEulerAngles = new UnityEngine.Vector3(0, 0, -angular);
                }
                else
                {
                    UIUtils.SetActive(faceDirection, false);
                }
            }
        }

        private Action<object> markFunc = null;
        public void SetMarkAction(Action<object> _markFunc)
        {
            markFunc = _markFunc;
        }
        public void UpdateMarkList()
        {
            var datas = adapter.TeamInfos;
            if (datas.Count > 0)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    if (datas[i].MarkList.Count > 0)
                    {
                        foreach (var mark in datas[i].MarkList)
                        {
                            if(markFunc!= null)
                                markFunc(mark);
                        }
                    }
                }
            }
        }
        public void UpdateMarkItemCommon(UIUtils.SimplePool markPool, Transform tran, float rate, MiniMapPlayMarkInfo data, Vector2 centerPoint, float centerWidth, Vector2 refePosByPixel)
        {
            if (adapter == null)
                return;

            var tranRT = tran.GetComponent<RectTransform>();
            var offset = new Vector2(tranRT.sizeDelta.x, tranRT.sizeDelta.y) / (2 * rate);
            var result = UIUtils.MathUtil.IsInSquare(centerPoint, centerWidth, centerWidth, offset, true, data.Pos);
            if (!result.IsContact)  //地图内
            {
                MiniMapPlayStatue statue = MiniMapPlayStatue.NONE;
                Color color = Color.white;
                foreach (var item in adapter.TeamInfos)
                {
                    if (data.Num == item.Num)
                    {
                        statue = item.Statue;
                        color = item.Color;
                        break;
                    }
                }

                if (statue == MiniMapPlayStatue.DEAD) //死亡
                {
                    markPool.DespawnGo(tran);
                }
                else
                {
                    var img = tran.GetComponent<Image>();
//                    var temperSprite = SpriteComon.GetInstance().GetSpriteByName("fix_00");
//                    if (temperSprite != null && temperSprite != img.sprite)
//                        img.sprite = temperSprite;
                    img.color = color;

                    var pos = result.ContactPoint * rate;
                    tran.GetComponent<RectTransform>().anchoredPosition = refePosByPixel + pos;                  //更新标记位置
                }
            }
            else
            {
                markPool.DespawnGo(tran);
            }
        }
    }

    public class AirPlaneCommon : AbstractModel
    {
        private static AirPlaneCommon instance = null;
        public static AirPlaneCommon GetInstance()
        {
            if (instance == null)
            {
                instance = new AirPlaneCommon();
                instance.PreParedKTouSprite();
            }
            return instance;
        }

        //空投点 通用
        private Dictionary<string, Sprite> kTouSpriteDic = new Dictionary<string, Sprite>();
        private const string uiIconsBundleName = "ui/icons";
        public int spriteSum = 13;
        public float intervalTime = 0.03f;

        private void PreParedKTouSprite()
        {
            kTouSpriteDic.Clear();
            for (int i = 1; i <= spriteSum; i++)
            {
                var name = GetSpriteNameByNum(i);
                Loader.RetriveSpriteAsync(uiIconsBundleName, name,
                            (sprite) =>
                            {
                                kTouSpriteDic.Add(name, sprite);
                            });
            }
        }
        private string GetSpriteNameByNum(int i)  /*i >=1  <= 13*/
        {
            var name = "000";
            if (i < 10)
            {
                name = name + "0" + i.ToString();
            }
            else
            {
                name = name + i.ToString();
            }
            return name;
        }
        private Sprite GetSpriteByName(string name)
        {
            if (kTouSpriteDic.ContainsKey(name))
            {
                return kTouSpriteDic[name];
            }
            else
                return null;
        }
        public Sprite GetSpriteByNum(int i)
        {
            return GetSpriteByName(GetSpriteNameByNum(i));
        }
        public int CaculateSprteIndex(int index)
        {
            int temIndex = -1;
            temIndex = Mathf.Abs((index + 1) % (spriteSum + 1));
            return temIndex;
        }
    }

    public class RouteLineCommon : AbstractModel
    {
        private static RouteLineCommon instance = null;
        IMiniMapUiAdapter adapter;
        public static RouteLineCommon GetInstance()
        {
            if (instance == null)
            {
                instance = new RouteLineCommon();
            }
            return instance;
        }

        public void SetMiniMapAdapter(IMiniMapUiAdapter _context)
        {
            this.adapter = _context;
        }

        public void UpdataRouteLineCommon(Vector2 referPosByRice, RectTransform transform, float rate)
        {
            if (!adapter.IsShowRouteLine)
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                var startPosByRice = adapter.RouteLineStartPoint;
                var endPosByRice = adapter.RouteLineEndPoint;

                var startPosByPixel = (startPosByRice - referPosByRice) * rate;
                var endPosByPixel = (endPosByRice - referPosByRice) * rate;

                //更新角度
                Vector2 from = new Vector2(0, 1);   //因为prefab上 默认图片是向右方向
                Vector2 temper = endPosByPixel - startPosByPixel;
                Vector2 to = new Vector3(temper.x * -1, temper.y);  // 以玩家为中心的坐标系 转化unity的3d坐标系
                {
                    float angle = Vector2.Angle(from, to); //求出两向量之间的夹角  
                    Vector3 normal = Vector3.Cross(from, to);//叉乘求出法线向量  
                    if (normal.z < 0)
                    {
                        angle = 360 - angle;
                    }
                    transform.localEulerAngles = new Vector3(0, 0, -angle);
                }

                //更新位置
                Vector2 middlePos = (startPosByPixel + endPosByPixel) / 2;
                transform.anchoredPosition = middlePos;

                //更新大小
                float height = Vector2.Distance(startPosByPixel, endPosByPixel);
                transform.sizeDelta = new Vector2(transform.sizeDelta.x, height);
            }
        }
    }    

    public class SpriteComon: AbstractModel
    {
        private static SpriteComon instance = null;
        public static SpriteComon GetInstance()
        {
            if (instance == null)
            {
                instance = new SpriteComon();
            }
            return instance;
        }
        private Dictionary<string, Sprite> playerMarkSpriteDic = new Dictionary<string, Sprite>();
        private const string uiIconsBundleName = "ui/icons";
        public void PreparedSprites()
        {
            playerMarkSpriteDic.Clear();

            Loader.RetriveSpriteAsync(uiIconsBundleName, "Loft_icon", (sprite) =>
            {
                playerMarkSpriteDic.Add("Loft_icon", sprite);
            });

            Loader.RetriveSpriteAsync(uiIconsBundleName, "icon_parachute", (sprite) =>
            {
                playerMarkSpriteDic.Add("icon_parachute", sprite);
            });

            Loader.RetriveSpriteAsync(uiIconsBundleName, "icon_drive", (sprite) =>
            {
                playerMarkSpriteDic.Add("icon_drive", sprite);
            });

            Loader.RetriveSpriteAsync(uiIconsBundleName, "icon_hurt", (sprite) =>
            {
                playerMarkSpriteDic.Add("icon_hurt", sprite);
            });

            Loader.RetriveSpriteAsync(uiIconsBundleName, "icon_die", (sprite) =>
            {
                playerMarkSpriteDic.Add("icon_die", sprite);
            });
        }
        public Sprite GetSpriteByName(string name)
        {
            if (playerMarkSpriteDic.ContainsKey(name))
            {
                return playerMarkSpriteDic[name];
            }
            return null;
        }
    }        
}
