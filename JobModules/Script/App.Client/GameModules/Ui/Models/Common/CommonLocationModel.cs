using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonLocationModel : ClientAbstractModel, IUiSystem
    {
        ILocationUiAdapter adapter;
        private bool isGameObjectCreated = false;
        private struct LocationItem
        {
            Transform trans;
            float angel;
            public List<Transform> markList;
            public LocationItem(Transform tr = null, float an = 0)
            {
                trans = tr;
                angel = an;
                this.markList = new List<Transform>();
            }
            public Transform getTran() { return trans; }
            public float GetAngel() { return angel; }           
            public void SetAngel(float value) { angel = value; }
            public void SetTran(Transform tran) { trans = tran; }
        }
        private List<LocationItem> itemList = new List<LocationItem>();
        private List<LocationItem> removedList = new List<LocationItem>();
        private Transform itemModel;
        private float itemModelWidth = 0;
        private float itemModelAngel = 15f; //每个item的对应的角度是15度
        private Transform root;
        private RectTransform rootRT = null;
        private float leftRightEffectAngel = 150f;
        private float lastCurFaceDirection = 0; // 0到360度
        private Transform markRoot;
        //刻度条移动方向
        private enum RotateDirection
        {
            NONE,
            LEFT,
            RIGTH
        }
        private enum MovingDirection
        {
            NONE,
            LEFT,
            RIGTH
        }
        private MovingDirection locationMoveDierction = MovingDirection.NONE;
        private double locationMoveIntegretNum = 0;  //待补充的数目 向上取整
        private double locationMoveDistance = 0;
        private double locationMovedIntergetNum = 0; //待移除的数目 向下取整
        private bool isOnceCompletMoving = false;
        //刻度配置相关变量
        struct LocationInfo
        {
            public string name;
            public string  lineIconName;
            public LocationInfo(string n, string iconN)
            {
                name = n;
                lineIconName = iconN;
            }
        }
        private Dictionary<float, LocationInfo> locationConfig = new Dictionary<float, LocationInfo>();
        //标记 相关变量
        List<Transform> RemoveMarkList = new List<Transform>();
        Transform markModel = null;

        private Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

        private CommonLocationViewModel _viewModel = new CommonLocationViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonLocationModel(ILocationUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            PreparedLocationConfig();
            PreparedSprite();
            DynamicCreateGameObject();
            InitLocationGroup();
        }

       public override void Update(float interval)
        {
            if (!isVisible) return;

            UpdateLocationGroup();
        }

        //自定义函数
        private void PreparedSprite()
        {
            spriteDic.Clear();
            Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "guid_1", (sprite) =>
            {
                spriteDic.Add("guid_1", sprite);
            });

            Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_UiIcons, "guid_2", (sprite) =>
            {
                spriteDic.Add("guid_2", sprite);
            });
        }
        private Sprite GetSpriteByName(string name)
        {
            if (spriteDic.ContainsKey(name))
            {
                return spriteDic[name];
            }
            return null;
        }

        private void PreparedLocationConfig()
        {
            locationConfig.Clear();         
            locationConfig.Add(0, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word5, "guid_1"));
            locationConfig.Add(15, new LocationInfo("15", "guid_2"));
            locationConfig.Add(30, new LocationInfo("30", "guid_2"));
            locationConfig.Add(45, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word6, "guid_1"));
            locationConfig.Add(60, new LocationInfo("60", "guid_2"));
            locationConfig.Add(75, new LocationInfo("75", "guid_2"));
            locationConfig.Add(90, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word7, "guid_1"));
            locationConfig.Add(105, new LocationInfo("105", "guid_2"));
            locationConfig.Add(120, new LocationInfo("120", "guid_2"));
            locationConfig.Add(135, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word8, "guid_1"));
            locationConfig.Add(150, new LocationInfo("150", "guid_2"));
            locationConfig.Add(165, new LocationInfo("165", "guid_2"));
            locationConfig.Add(180, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word9, "guid_1"));
            locationConfig.Add(195, new LocationInfo("195", "guid_2"));
            locationConfig.Add(210, new LocationInfo("210", "guid_2"));
            locationConfig.Add(225, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word10, "guid_1"));
            locationConfig.Add(240, new LocationInfo("240", "guid_2"));
            locationConfig.Add(255, new LocationInfo("255", "guid_2"));
            locationConfig.Add(270, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word11, "guid_1"));
            locationConfig.Add(285, new LocationInfo("285", "guid_2"));
            locationConfig.Add(300, new LocationInfo("300", "guid_2"));
            locationConfig.Add(315, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word12, "guid_1"));
            locationConfig.Add(330, new LocationInfo("330", "guid_2"));
            locationConfig.Add(345, new LocationInfo("345", "guid_2"));
            locationConfig.Add(360, new LocationInfo(I2.Loc.ScriptLocalization.client_common.word5, "guid_1"));
        }
        private void DynamicCreateGameObject()
        {
            itemModel = FindChildGo("locaItem");
            root = FindChildGo("root");
            markModel = FindChildGo("mark");
            markRoot = FindChildGo("markRoot");
            if (root == null || itemModel == null || markModel == null || markRoot == null)
                return;
            rootRT = root.GetComponent<RectTransform>();
            itemModel.gameObject.SetActive(false);
            markModel.gameObject.SetActive(false);
            itemList.Clear();
            for (int i = 0; i < 13; i++)               //默认有13个 标度条
            {
                var item = GameObject.Instantiate(itemModel, root);
                if (item)
                {
                    item.gameObject.SetActive(true);
                    var rectTran = item.GetComponent<RectTransform>();
                    if (rectTran)
                    {
                        if(itemModelWidth == 0)
                        {
                            itemModelWidth = rectTran.sizeDelta.x;
                        }
                        rectTran.anchoredPosition = new Vector2(i * rectTran.sizeDelta.x, 0);
                        itemList.Add(new LocationItem(item, (270 + i* itemModelAngel) % 360));
                    }
                }
            }         
        }
        private void InitLocationGroup() 
        {
            //默认只有13个方位能看的到 这里默认人朝向是正北方向 角度从  270 到 360(0) 到90
            lastCurFaceDirection = 0;        
            RefreshLocationDetail();
        }
        private void UpdateLocationGroup()
        {
            if (adapter != null && isGameObjectCreated)
            {
                MovingControl(adapter._CurFaceDirection);
            }
        }

        private void MovingControl(float curPlayerFaceDirection)  //curPlayerFaceDirection 《0，360》 顺时针方向
        {          
            if(curPlayerFaceDirection > 0)
            {
                curPlayerFaceDirection = 180 + Math.Abs(curPlayerFaceDirection - 180);
            }
            else if(curPlayerFaceDirection < 0)
            {
                curPlayerFaceDirection = -curPlayerFaceDirection;
            }

            RecycleNoVisibleItems();

            //计算与最后一次的朝向的角度偏差
            float deltaAngel = 0;
            RotateDirection rotateDiret = RotateDirection.NONE;
            if (curPlayerFaceDirection == lastCurFaceDirection)
            {
                rotateDiret = RotateDirection.NONE;
                deltaAngel = 0;
                //return;
            }
            else if (curPlayerFaceDirection > lastCurFaceDirection)
            {
                float temperDistance = curPlayerFaceDirection - lastCurFaceDirection;
                deltaAngel = temperDistance;
                rotateDiret = RotateDirection.RIGTH;
            }
            else if(curPlayerFaceDirection < lastCurFaceDirection)
            {
                float temperDistance = lastCurFaceDirection - curPlayerFaceDirection;
                deltaAngel = temperDistance;
                rotateDiret = RotateDirection.LEFT;
            }

            //转换成  定位条的移动
            locationMoveIntegretNum = Math.Ceiling((double)(deltaAngel / itemModelAngel));
            locationMovedIntergetNum = Math.Floor((double)(deltaAngel / itemModelAngel));
            locationMoveDistance = locationMovedIntergetNum * itemModelWidth + ((deltaAngel % itemModelAngel) / itemModelAngel * itemModelWidth);
            locationMoveDierction = (rotateDiret == RotateDirection.LEFT? MovingDirection.RIGTH: MovingDirection.LEFT);

            //添补ItemList 为移动动画做准备
            if (locationMoveDierction == MovingDirection.LEFT)  
            {
                List<LocationItem> temperList = new List<LocationItem>();
                temperList.Clear();               
                for (int i = 0; i < locationMoveIntegretNum; i++)
                {
                    LocationItem item = SpawnFromRemovedList();
                    var newAngel = itemList[itemList.Count - 1].GetAngel() + (i + 1) * itemModelAngel;
                    item.SetAngel(newAngel > 360 ? newAngel % 360 : newAngel);    //设置角度
                    temperList.Add(item);
                }

                float temperX = itemList[itemList.Count - 1].getTran().GetComponent<RectTransform>().anchoredPosition.x;
                float temperY = itemList[itemList.Count - 1].getTran().GetComponent<RectTransform>().anchoredPosition.y;
                for (int i = 0; i < temperList.Count; i++)
                {
                    temperList[i].getTran().GetComponent<RectTransform>().anchoredPosition = new Vector2(temperX + (i + 1) * itemModelWidth, temperY);  //设置位置
                    itemList.Add(temperList[i]);
                }               
                temperList.Clear();
            }
            else if(locationMoveDierction == MovingDirection.RIGTH)
            {
                List<LocationItem> temperList = new List<LocationItem>();
                temperList.Clear();
                for (int i = 0; i < locationMoveIntegretNum; i++)
                {                   
                    LocationItem item = SpawnFromRemovedList();
                    var newAngel = itemList[0].GetAngel() - (i + 1) * itemModelAngel;
                    item.SetAngel(newAngel < 0 ? 360 + newAngel : newAngel);   //设置角度
                    temperList.Add(item);
                }

                float temperX = itemList[0].getTran().GetComponent<RectTransform>().anchoredPosition.x;
                float temperY = itemList[0].getTran().GetComponent<RectTransform>().anchoredPosition.y;
                for (int i = 0; i < temperList.Count; i++)
                {
                    temperList[i].getTran().GetComponent<RectTransform>().anchoredPosition = new Vector2(temperX - (i + 1) * itemModelWidth, temperY);   //设置位置
                    itemList.Insert(0, temperList[i]);
                }
                temperList.Clear();
            }

            //更新位置
            isOnceCompletMoving = true;
            for (int i = 0; i < itemList.Count; i++)
            {
                Transform item = itemList[i].getTran();
                RectTransform itemRT = item.GetComponent<RectTransform>();
                double startPos = itemRT.anchoredPosition.x;
                double endPos = 0;
                double number = startPos;
                if (locationMoveDierction == MovingDirection.LEFT)
                {
                    endPos = startPos - locationMoveDistance;
                }
                else if(locationMoveDierction == MovingDirection.RIGTH)
                {
                    endPos = startPos + locationMoveDistance;
                }                
                itemRT.anchoredPosition = new Vector2((float)endPos, itemRT.anchoredPosition.y);
            }

            //更新标志位
            lastCurFaceDirection = curPlayerFaceDirection;
            RefreshLocationDetail();
        }


        private void RecycleNoVisibleItems()
        {
            //if (locationMoveDierction == MovingDirection.LEFT)
            {
                //从左到右判断 找到所有不在可见范围的 移除到removeList中
                int temperIndex = 0;
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].getTran().GetComponent<RectTransform>().anchoredPosition.x + itemModelWidth <= 0)
                    {
                        temperIndex++;
                        var temItem = itemList[i];
                        DespawnRemoveList(temItem);
                    }
                }
                for (int j = 0; j < temperIndex; j++)
                {
                    itemList.RemoveAt(0);
                }
            }
            //else if (locationMoveDierction == MovingDirection.RIGTH)
            {
                int temperIndex = itemList.Count - 1;
                for (int j = temperIndex; j >= 0 ; j--)
                {
                    var temItem = itemList[j];
                    if(temItem.getTran().gameObject.GetComponent<RectTransform>().anchoredPosition.x >= root.GetComponent<RectTransform>().sizeDelta.x) 
                    {
                        DespawnRemoveList(temItem);
                        itemList.RemoveAt(j);
                    }                  
                }
            }     
        }
        private void RefreshLocationDetail()
        {
            if (itemList.Count == 0 || adapter == null)
                return;

            int index = 0;
            float miniDis = rootRT.sizeDelta.x / 2;
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                float itemFaceDirection = item.GetAngel();
                var itemTran = item.getTran();
                var itemTranRT = itemTran.GetComponent<RectTransform>();
                LocationInfo localInfo;
                if (locationConfig.TryGetValue(itemFaceDirection, out localInfo))
                {
                    var line = itemTran.Find("line");
                    var name = itemTran.Find("name");
                    if (line && name)  
                    {
                        //线
                        var lineImg = line.GetComponent<Image>();
                        var lineRt = line.GetComponent<RectTransform>();
                        if (lineImg)
                        {
                            var temperSprite = GetSpriteByName(localInfo.lineIconName); 
                            if (temperSprite != null && temperSprite != lineImg.sprite)
                            {
                                lineImg.sprite = temperSprite;
                                lineImg.SetNativeSize();
                            }
                        }

                        //名字
                        var nameText = name.GetComponent<Text>();
                        var nameRt = name.GetComponent<RectTransform>();
                        if (nameText)
                        {
                            nameText.text = localInfo.name;
                        }

                        if (itemFaceDirection % 45 == 0)  //八个方向
                        {
                            nameRt.anchoredPosition = new Vector2(0, -(lineRt.sizeDelta.y/2 + nameRt.sizeDelta.y/2));
                            nameText.fontSize = 14;
                        }
                        else
                        {
                            nameRt.anchoredPosition = new Vector2(0, -(lineRt.sizeDelta.y/2 + nameRt.sizeDelta.y/2));
                            nameText.fontSize = 12;
                        }

                        //更新每根方位条的透明度 <0, 100>  <rootW-100 rootW>
                        if (itemTranRT.anchoredPosition.x <= leftRightEffectAngel)
                        {
                            float fAlpha = 0;
                            var rate = Math.Abs(itemTranRT.anchoredPosition.x - leftRightEffectAngel) / leftRightEffectAngel;
                            //if (rate == 0)
                            //    fAlpha = 1f;
                            //else if (rate > 0 && rate < 0.25)
                            //    fAlpha = 0.75f;
                            //else if (rate > 0.25 && rate < 0.5)
                            //    fAlpha = 0.5f;
                            //else if (rate > 0.5 && rate < 0.75)
                            //    fAlpha = 0.25f;
                            //else if (rate > 0.75)
                            //    fAlpha = 0f;
                            fAlpha =  1 - rate;
                            fAlpha = Mathf.Clamp01(fAlpha);
                            lineImg.color = new Color(lineImg.color.r, lineImg.color.g, lineImg.color.b, fAlpha);
                            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, fAlpha);

                        }
                        else if(itemTranRT.anchoredPosition.x >= rootRT.sizeDelta.x - leftRightEffectAngel)
                        {
                            float fAlpha = 0;
                            var rate = Math.Abs(itemTranRT.anchoredPosition.x - (rootRT.sizeDelta.x - leftRightEffectAngel)) / leftRightEffectAngel;
                            //if (rate == 0)
                            //    fAlpha = 1f;
                            //else if (rate > 0 && rate < 0.25)
                            //    fAlpha = 0.75f;
                            //else if (rate > 0.25 && rate < 0.5)
                            //    fAlpha = 0.5f;
                            //else if (rate > 0.5 && rate < 0.75)
                            //    fAlpha = 0.25f;
                            //else if (rate > 0.75)
                            //    fAlpha = 0f;
                            fAlpha = 1 - rate;
                            fAlpha = Mathf.Clamp01(fAlpha);
                            lineImg.color = new Color(lineImg.color.r, lineImg.color.g, lineImg.color.b, fAlpha);
                            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, fAlpha);
                        }
                        else
                        {
                            lineImg.color = new Color(lineImg.color.r, lineImg.color.g, lineImg.color.b, 1);
                            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 1);
                        }

                        //强化最近的一根方位条
                        if (Math.Abs((itemTranRT.anchoredPosition.x + itemModelWidth / 2) - rootRT.sizeDelta.x / 2) <= miniDis)
                        {
                            miniDis = Math.Abs(itemTranRT.anchoredPosition.x - rootRT.sizeDelta.x / 2);
                            index = i;
                        }

                        // 改变 mark   找到所有顺时针方向上 比自己大15度范围内的 所有的标记信息
                        for (int m = 0; m < item.markList.Count; m++)
                        {
                            DespawnRemoveMarkList(item.markList[m]);
                        }
                        item.markList.Clear();

                        var teamMarkInfos = adapter.TeamPlayerMarkInfos;
                        for (int j = 0; teamMarkInfos.Count > 0 && j < teamMarkInfos.Count; j++)
                        {
                            var markInfo = teamMarkInfos[j];

                            if ((markInfo.Angel >= itemFaceDirection && Math.Abs(markInfo.Angel - itemFaceDirection) < 15))
                            {
                                Transform markItem = null;
                                markItem = SpawnFromRemoveMarkList(itemTran);
                                var markItemRT = markItem.GetComponent<RectTransform>();
                                item.markList.Add(markItem);

                                float deltaDistance = Math.Abs(markInfo.Angel - itemFaceDirection);                            //设置位置                         
                                markItemRT.anchoredPosition =
                                    new Vector2((deltaDistance / itemModelAngel) * itemModelWidth, itemTranRT.sizeDelta.y/2 + 5);

                                var img = markItem.GetComponent<Image>();

                                //颜色和透明度
                                img.color = new Color(markInfo.MarkColor.r, markInfo.MarkColor.g, markInfo.MarkColor.b, lineImg.color.a);
                            }
                        }
                    }                   
                }
            }

            var needEnhanceItem = itemList[index];
            var needEnhanceItemTran = needEnhanceItem.getTran();
            var needEnhanceLine = needEnhanceItemTran.Find("line");
            var needEnhanceName = needEnhanceItemTran.Find("name");
            if (needEnhanceLine && needEnhanceName)
            {
                var lineImg = needEnhanceLine.GetComponent<Image>();
                var lineRt = needEnhanceLine.GetComponent<RectTransform>();

                var nameText = needEnhanceName.GetComponent<Text>();
                var nameRt = needEnhanceName.GetComponent<RectTransform>();
                if (lineImg && lineRt && nameText && nameRt)
                {
                    nameText.fontSize = (int)Math.Round(nameText.fontSize * 1.3f);
                }
            }
        }

        private LocationItem SpawnFromRemovedList()
        {
            if (removedList.Count > 0)
            {
                var item = removedList[removedList.Count - 1];
                removedList.RemoveAt(removedList.Count - 1);
                item.SetAngel(0f);
                item.getTran().gameObject.SetActive(true);
                return item;
            }
            else
            {
                var newTran = GameObject.Instantiate(itemModel, root);
                LocationItem item = new LocationItem(newTran, 0f);               
                item.getTran().gameObject.SetActive(true);
                return item;
            }
        }
        private void DespawnRemoveList(LocationItem item)
        {
            var markList = item.markList;
            if (markList != null)
            {
                for (int i = 0; i < markList.Count; i++)
                {
                    DespawnRemoveMarkList(markList[i]);
                }
                markList.Clear();
            }
            item.getTran().gameObject.SetActive(false);
            removedList.Add(item);
        }
        private Transform SpawnFromRemoveMarkList(Transform parent)
        {
            if(RemoveMarkList.Count > 0)
            {
                Transform item = RemoveMarkList[RemoveMarkList.Count - 1];
                RemoveMarkList.RemoveAt(RemoveMarkList.Count - 1);
                item.SetParent(parent);
                item.gameObject.SetActive(true);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
                return item;
            }
            else
            {
                Transform item = GameObject.Instantiate(markModel, parent);
                item.gameObject.SetActive(true);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
                return item;
            }
        }
        private void DespawnRemoveMarkList(Transform item)
        {
            item.SetParent(markRoot);
            item.gameObject.SetActive(false);
            RemoveMarkList.Add(item);
        }      
    }    
}    
