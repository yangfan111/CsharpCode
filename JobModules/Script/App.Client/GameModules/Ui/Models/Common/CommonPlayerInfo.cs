using System.Collections.Generic;
using App.Client.CastObjectUtil;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonPlayerInfo : ClientAbstractModel, IUiHfrSystem
    {
        private CommonPlayerInfoViewModel _viewModel = new CommonPlayerInfoViewModel();
        PlayerInfoUIAdapter playerInfoUIAdapter;

        private Dictionary<long, PlayerInfo> _playerInfoDic = new Dictionary<long, PlayerInfo>();
        private List<long> _garbageKey = new List<long>();
        private int selectedPlayerId;

        public CommonPlayerInfo(PlayerInfoUIAdapter playerInfoUIAdapter):base(playerInfoUIAdapter)
        {
            this.playerInfoUIAdapter = playerInfoUIAdapter;
        }
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitKeyBinding();

        }
       public override void Update(float interval)
        {

            var markTime = Time.deltaTime;
            var datas = playerInfoUIAdapter.TeamPlayerInfos;
            foreach (var info in datas)
            {
                if (info.IsPlayer) continue;

                PlayerInfo playerInfo;
                if (_playerInfoDic.TryGetValue(info.PlayerId, out playerInfo))
                {
                    if (info.Statue == MiniMapPlayStatue.DEAD)
                    {
                        playerInfo.MarkTime = markTime;
                        playerInfo.SetActive(false);
                        continue;
                    }

                    playerInfo.UpdateInfo(info.IsShow, info.Num, info.Color, info.PlayerName, info.EntityId, selectedPlayerId, info.CurHp * 1.0f / info.MaxHp);
                    playerInfo.UpdateLocation(info.TopPos);
                }
                else
                {
                    _playerInfoDic[info.PlayerId] = CreatePlayerInfo(info);
                }
                _playerInfoDic[info.PlayerId].MarkTime = markTime;

            }

            foreach (var piar in _playerInfoDic)
            {
                if (piar.Value.MarkTime.Equals(markTime) == false)
                {
                    piar.Value.Destroy();
                    _garbageKey.Add(piar.Key);
                }
            }

            if (_garbageKey.Count > 0)
            {
                foreach (var key in _garbageKey)
                {
                    _playerInfoDic.Remove(key);
                }

                _garbageKey.Clear();
            }
            //after UpdateUI clear Cast Data
           selectedPlayerId = 0;
        }

       public void InitKeyBinding()
       {
           var receiver = new PointerReceiver(UiConstant.maxMapWindowLayer, BlockType.None);
           receiver.BindPointAction(UserInputKey.PickUpTip, (data) =>
           {
               var pointerData = data as PointerData;
              
               var type = pointerData.IdList[0];
               switch ((ECastDataType)type)
               {

                   case ECastDataType.Player:
                       selectedPlayerId = PlayerCastData.EntityId(pointerData.IdList);
                       //Debug.Log(selectedPlayerId);
                       break;
                    default: selectedPlayerId = 0;
                        break;
               }
           });
           playerInfoUIAdapter.RegisterPointerReceive(receiver);
          
       }

        private PlayerInfo CreatePlayerInfo(MiniMapTeamPlayInfo PlayInfo)
        {
            var info = new PlayerInfo();
            info.Init(GameObject.Instantiate(FindChildGo("playerInfo").gameObject, FindChildGo("Show")));
            info.UpdateInfo(PlayInfo.IsShow, PlayInfo.Num, PlayInfo.Color, PlayInfo.PlayerName, PlayInfo.EntityId, 0,1);
            info.UpdateLocation(PlayInfo.TopPos);
            return info;
        }


        public override void OnDestory()
        {
            base.OnDestory();
            foreach (var paire in _playerInfoDic)
            {
                paire.Value.Destroy();
            }

            _playerInfoDic.Clear();
        }

       
    }

    class PlayerInfo
    {
        public float MarkTime;
        private int _playerId;
        private GameObject go;
        private RectTransform rect;
        private RectTransform parentRect;
        private Image _indexImage;
        private Text _indexText;
        private Text _nameText;
        private Text _bloodText;
        private Vector3 _position = Vector3.zero;
        private Camera camera;

        private bool isStartUp = true;

        private bool hasWait;
        public bool isActive = true;
        private ActiveSetter goActiveSetter;
        //private Vector2 canvasSize;
        public void Init(GameObject gameObject)
        {
            camera = Camera.main;
            //canvasSize = UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            //            if (UiCommon.UIManager.UICamera != null)
            //            {
            //                camera = UiCommon.UIManager.UICamera;
            //            }
            go = gameObject;
            goActiveSetter = new ActiveSetter(go);
            rect = gameObject.GetComponent<RectTransform>();
            parentRect = rect.GetComponentInParent<RectTransform>();
            goActiveSetter.Active = true;
            _indexImage = gameObject.transform.Find("IndexImage").GetComponent<Image>();
            var tests = gameObject.GetComponentsInChildren<Text>(true);
            foreach (var v in tests)
            {
                var realName = v.gameObject.name;
                switch (realName)
                {
                    case "IndexText":
                        _indexText = v;
                        break;
                    case "NameText":
                        _nameText = v;
                        break;
                    case "BloodText":
                        _bloodText = v;
                        break;
                }
            }
        }

        public void UpdateInfo(bool isShow, int index, Color color, string name,int playerID,int selectId,float percent)
        {
            SetActive(isShow);
            if (!isActive) return;
            _playerId = playerID;
          
            _indexImage.color = color;
            Color oriColor = _nameText.color;

            DoNormalSet(selectId, oriColor,percent);

            _indexText.text = index.ToString();
            _nameText.text = name;

            //LayoutRebuilder.MarkLayoutForRebuild(rect);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        private void DoNormalSet(int selectId, Color oriColor,float percent)
        {

            if (_playerId == selectId)
            {
                oriColor = new Color(oriColor.r, oriColor.g, oriColor.b, 1);
                _nameText.color = oriColor;
                _bloodText.gameObject.SetActive(true);
                _bloodText.text = ((int)(percent * 100)).ToString()+"%";
            }
            else
            {
                oriColor = new Color(oriColor.r, oriColor.g, oriColor.b, 0.5f);
                _nameText.color = oriColor;
                _bloodText.gameObject.SetActive(false);
            }
            
        }


        public void UpdateLocation(Vector3 pos)
        {
            if (!isActive) return;
            var viewPortPos = camera.WorldToViewportPoint(pos);
            if (!InView(pos, viewPortPos))
            {
                goActiveSetter.Active = false;
                return;
            }
            goActiveSetter.Active = true;
            _position = pos;

            Vector2 result = UIUtils.WorldPosToRect(pos, camera, parentRect);
            Vector2 screenPos = new Vector3(result.x , result.y, 0);
            rect.anchoredPosition3D = screenPos;
        }

        private bool InView(Vector3 pos, Vector2 viewPos)
        {
            var cam = Camera.main;
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);//判断物体是否在相机前面
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                return true;
            else
                return false;
        }

        public void Destroy()
        {
            GameObject.Destroy(go);
        }

        public void SetActive(bool flag)
        {
            go.SetActive(flag);
            isActive = flag;
        }
    }
}