using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.Utils.MiniMaxMapCommon;
using App.Shared.Components.Ui;
using Core.ObjectPool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class MapPlayer
    {
        private Transform tran;
        private Transform playItemModel = null;
        private Dictionary<long, PlayerItem> playerItemDic;
        public bool isNoTeamPlayer = false;
        public MapPlayer(Transform tran)
        {
            this.tran = tran;
            playItemModel = tran.Find("playitem");
            playerItemDic = new Dictionary<long, PlayerItem>();
        }

        public void Update(List<MiniMapTeamPlayInfo> TeamInfos, Vector2 selfPlayPos, float rate, float miniMapRepresentWHByRice, MapLevel mapLevel)
        {
            if (TeamInfos.Count > 0)
            {
                isNoTeamPlayer = (TeamInfos.Count == 1) ? true : false;
                for (int i = 0; i < TeamInfos.Count; i++)
                {
                    MiniMapTeamPlayInfo data = TeamInfos[i];
                    PlayerItem playerItem;
                    playerItemDic.TryGetValue(data.PlayerId, out playerItem);
                    if (playerItem == null)
                    {
                        playerItem = new PlayerItem(GameObject.Instantiate(playItemModel, tran, true));
                        playerItemDic[data.PlayerId] = playerItem;
                    }

                    playerItem.IsOutDate = false;
                    playerItem.Update(data, rate, selfPlayPos, miniMapRepresentWHByRice, mapLevel, isNoTeamPlayer);
                }
            }

            ClearOutDateItem();
        }

        private void ClearOutDateItem()
        {
            List<long> list = null;
            foreach (var playerItem in playerItemDic)
            {
                if (playerItem.Value.IsOutDate)
                {
                    if (list == null) list = new List<long>();
                    list.Add(playerItem.Key);
                }
                playerItem.Value.IsOutDate = true;
            }

            if (list != null)
            {
                foreach (var playerId in list)
                {
                    var playerItem = playerItemDic[playerId];
                    playerItem.Dispose();
                    playerItemDic.Remove(playerId);
                }
            }
        }
    }

    public class PlayerItem
    {
        private Transform tran;
        private RectTransform rectTransform;

        private Transform direction;
        private RectTransform directionRectTf;

        private Transform bgTf;
        private RectTransform bgRectTf;
        private Transform number;
        private Text numberText;
        private Image bgImage;
        private Transform stateIcon;
        private Transform loftIcon;
        private Image stateIconImage;
        private Image loftIconImage;

        private Transform faceDirectionTf;

        private Dictionary<Transform, Tween> tranCTween = new Dictionary<Transform, Tween>(); // Transform与之关联的Tween

        public bool IsOutDate;

        public PlayerItem(Transform tran)
        {
            UIUtils.SetActive(tran, true);
            this.tran = tran;
            rectTransform = tran.GetComponent<RectTransform>();
            direction = tran.Find("direction");
            directionRectTf = direction.GetComponent<RectTransform>();

            bgTf = tran.Find("bg");
            bgRectTf = bgTf.GetComponent<RectTransform>();
            number = bgTf.Find("number");
            numberText = number.GetComponent<Text>();
            bgImage = bgTf.GetComponent<Image>();
            stateIcon = bgTf.Find("icon");
            loftIcon = bgTf.Find("loftIcon");
            stateIconImage = stateIcon.GetComponent<Image>();
            loftIconImage = loftIcon.GetComponent<Image>();

            faceDirectionTf = tran.Find("faceDireciton");
        }

        public void Update(MiniMapTeamPlayInfo data, float rate, Vector2 selfPlayPos, float miniMapRepresentWHByRice, MapLevel mapLevel, bool isNoTeamPlayer)
        {
            UodateLocation(data, rate, ref selfPlayPos, miniMapRepresentWHByRice);
            UpdatePlayNum(data, mapLevel, isNoTeamPlayer);
            UpdatePlayStatue(ref selfPlayPos, data, mapLevel);
            UpdatePlayFaceDirection(data);
        }

        private bool _isPlayer;
        private Vector2 _pos = Vector2.zero;
        private Vector2 _selfPlayPos = Vector2.zero;
        private float _rate;
        private float _mapWidth;
        private void UodateLocation(MiniMapTeamPlayInfo data, float rate, ref Vector2 selfPlayPos, float miniMapRepresentWHByRice)
        {
            var shiftVec = data.Pos.ShiftedUIVector2();
            if (_isPlayer.Equals(data.IsPlayer) && _pos.Equals(shiftVec) && _selfPlayPos.Equals(selfPlayPos) &&
                _rate.Equals(rate) && _mapWidth.Equals(miniMapRepresentWHByRice)) return;

            _isPlayer = data.IsPlayer;
            _pos = shiftVec;
            _selfPlayPos = selfPlayPos;
            _rate = rate;
            _mapWidth = miniMapRepresentWHByRice;

            if (data.IsPlayer == true)
            {
                rectTransform.anchoredPosition = _pos * rate;
                UIUtils.SetActive(direction, false);
            }
            else
            {
                var offset = new Vector2(rectTransform.sizeDelta.y + directionRectTf.sizeDelta.y, rectTransform.sizeDelta.y + directionRectTf.sizeDelta.y) / (2 * rate);
                var result = UIUtils.MathUtil.IsInSquare(selfPlayPos, miniMapRepresentWHByRice, miniMapRepresentWHByRice, offset, true, _pos);
                tran.GetComponent<RectTransform>().anchoredPosition = (selfPlayPos + result.ContactPoint) * rate;
                if (!result.IsContact)
                {
                    UIUtils.SetActive(direction, false);
                }
                else
                {
                    UIUtils.SetActive(direction, true);
                    {
                        Vector2 fromVector = new Vector2(0, 1);
                        Vector2 toVector = tran.GetComponent<RectTransform>().anchoredPosition.normalized;

                        float angle = UnityEngine.Vector2.Angle(fromVector, toVector); //求出两向量之间的夹角  
                        UnityEngine.Vector3 normal = UnityEngine.Vector3.Cross(fromVector, toVector);//叉乘求出法线向量  
                        if (normal.z < 0)
                        {
                            angle = 360 - angle;
                        }
                        direction.localEulerAngles = new UnityEngine.Vector3(0, 0, (angle + 180) % 360);
                    }
                }
            }
        }

        private Color _color;
        private int _num;
        private bool _isNoTeamPlayer;
        private void UpdatePlayNum(MiniMapTeamPlayInfo data, MapLevel mapLevel, bool isNoTeamPlayer)
        {
            if (data.Num.Equals(_num) && data.Color.Equals(_color) && isNoTeamPlayer.Equals(_isNoTeamPlayer)) return;
            _isNoTeamPlayer = isNoTeamPlayer;
            _num = data.Num;
            _color = data.Color;
            //刷新编号

            if(number.gameObject.activeSelf)
            {
                if (numberText)
                {
                    if (data.IsPlayer)
                    {
                        if (isNoTeamPlayer)
                        {
                            numberText.text = string.Empty;
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

            //刷新编号背景图
//            var temperBgSprite = SpriteComon.GetInstance().GetSpriteByName("mark");
//            if (bgImage.sprite != temperBgSprite)
//            {
//                bgImage.sprite = temperBgSprite;
//            }
            bgImage.color = data.Color;
        }

        private Vector2 _referPos = Vector2.zero;
        private MiniMapPlayStatue _miniMapPlayStatue = MiniMapPlayStatue.NONE;
        private Vector2 _infoPos = Vector2.zero;
        private int _shootingCount = -1;
        private void UpdatePlayStatue(ref Vector2 referPos, MiniMapTeamPlayInfo data, MapLevel mapLevel)
        {
            var shiftVec = data.Pos.ShiftedUIVector2();
            if (referPos.Equals(_referPos) && data.Statue.Equals(_miniMapPlayStatue) &&
                shiftVec.Equals(_infoPos) && data.ShootingCount.Equals(_shootingCount)) return;
            _referPos = referPos;
            _miniMapPlayStatue = data.Statue;
            _infoPos = shiftVec;
            _shootingCount = data.ShootingCount;
            
           switch (data.Statue)
            {
                case MiniMapPlayStatue.NORMAL:    //常态
                    {
                        UIUtils.SetActive(number,!MapLevel.Min.Equals(mapLevel));
                        UIUtils.SetActive(stateIcon, false);
                        if (data.IsPlayer)
                        {
                            UIUtils.SetActive(loftIcon, false);
                        }
                        else
                        {
//                            UIUtils.SetActive(loftIcon, true);
//                            var temperSprite = SpriteComon.GetInstance().GetSpriteByName("Loft_icon");
//                            if (temperSprite != null && loftIconImage.sprite != temperSprite)
//                            {
//                                loftIconImage.sprite = temperSprite;
//                            }
//
//                            if (data.Pos.y > referPos.y)       //上方
//                            {
//                                if (loftIcon.transform.localScale != Vector3.one)
//                                    loftIcon.transform.localScale = Vector3.one;
//                            }
//                            else if (data.Pos.y <= referPos.y)   //下方
//                            {
//                                if (loftIcon.transform.localScale != new UnityEngine.Vector3(1, -1, 1))
//                                    loftIcon.transform.localScale = new UnityEngine.Vector3(1, -1, 1);
//                            }
                        }

                        if (data.ShootingCount > 0)   //在射击状态下
                        {

                            if (!tranCTween.ContainsKey(tran) || tranCTween[tran] == null)
                            {
                                var temperTween = UIUtils.CallTween(1, 1.5f, (value) =>
                                {
                                    bgRectTf.localScale = new UnityEngine.Vector3((float)value, (float)value, 1.0f);
                                },
                                (value) =>
                                {
                                    bgRectTf.localScale = Vector3.one;
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
                            if (bgRectTf.localScale != Vector3.one)
                                bgRectTf.localScale = Vector3.one;
                        }
                    }
                    break;
                case MiniMapPlayStatue.TIAOSAN:    //跳伞
                    {
                        UIUtils.SetActive(number, false);
                        UIUtils.SetActive(loftIcon, false);
                        UIUtils.SetActive(stateIcon, true);

                        var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_parachute");
                        if (temperSprite != null && stateIconImage.sprite != temperSprite)
                        {
                            stateIconImage.sprite = temperSprite;
                        }
                    }
                    break;
                case MiniMapPlayStatue.ZAIJU:    //载具
                    {
                        UIUtils.SetActive(number, false);
                        UIUtils.SetActive(loftIcon, false);
                        UIUtils.SetActive(stateIcon, true);

                        var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_drive");
                        if (temperSprite != null && stateIconImage.sprite != temperSprite)
                        {
                            stateIconImage.sprite = temperSprite;
                        }
                    }
                    break;
                case MiniMapPlayStatue.HURTED:    //受伤
                    {
                        UIUtils.SetActive(number, false);
                        UIUtils.SetActive(loftIcon, false);
                        UIUtils.SetActive(stateIcon, true);

                        var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_hurt");
                        if (temperSprite != null && stateIconImage.sprite != temperSprite)
                        {
                            stateIconImage.sprite = temperSprite;
                        }
                    }
                    break;
                case MiniMapPlayStatue.DEAD:    //死亡
                    {
                        UIUtils.SetActive(number, false);
                        UIUtils.SetActive(loftIcon, false);
                        UIUtils.SetActive(stateIcon, true);

                        var temperSprite = SpriteComon.GetInstance().GetSpriteByName("icon_die");
                        if (temperSprite != null && stateIconImage.sprite != temperSprite)
                        {
                            stateIconImage.sprite = temperSprite;
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

        private MiniMapPlayStatue _playStatue = MiniMapPlayStatue.NONE;
        private float _faceDirection;
        private void UpdatePlayFaceDirection(MiniMapTeamPlayInfo data)
        {
            if (data.Statue.Equals(_playStatue) && data.FaceDirection.Equals(_faceDirection)) return;
            _playStatue = data.Statue;
            _faceDirection = data.FaceDirection;

            if (data.Statue != MiniMapPlayStatue.DEAD) //非死亡状态
            {
                UIUtils.SetActive(faceDirectionTf, true);

                float angular = data.FaceDirection % 360;
                angular = angular < 0 ? 360 + angular : angular;

                faceDirectionTf.localEulerAngles = new UnityEngine.Vector3(0, 0, -angular);
            }
            else
            {
                UIUtils.SetActive(faceDirectionTf, false);
            }
        }
       
        public void Dispose()
        {
            GameObject.Destroy(tran.gameObject);
        }
    }
}