using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using I2.Loc;
using Loxodon.Framework.ViewModels;
using UIComponent.UI;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonBlastTipsModel : ClientAbstractModel, IUiHfrSystem
    {

        enum ABType
        {
            A,
            B,
            C4
        }


        private IBlastTipsUiAdapter adapter = null;

        private CommonBlastTipsViewModel _viewModel = new CommonBlastTipsViewModel();
        private bool isGameObjectCreated;
        private RectTransform rootRect;

        TipsView AView;
        TipsView BView;
        TipsView C4View;



        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonBlastTipsModel(IBlastTipsUiAdapter adapter) : base(adapter)
        {
            this.adapter = adapter;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
            InitKeyBinding();
        }
        private void InitGui()
        {

            rootRect = UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>();
            AView = new TipsView(_viewModel.subrootA);
            BView = new TipsView(_viewModel.subrootB);
            C4View = new TipsView(_viewModel.subrootC4);
            AView.InitAB();
            BView.InitAB();
            C4View.InitC4();
        }


        public void RefreshGui()
        {
            if (!adapter.IsGameRulePass())
            {
                HideAll();
                return;
            }

            var comp = adapter.GetBlastData();
            UpdateAB(comp.BlastAPosition.ShiftedVector3(), ABType.A);
            UpdateAB(comp.BlastBPosition.ShiftedVector3(), ABType.B);
            if (adapter.IsCampPass() && comp.IsC4Droped)

                UpdateAB(comp.C4DropPosition.ShiftedVector3(), ABType.C4);
            else
                _viewModel.subrootC4.gameObject.SetActive(false);

            if (comp.C4SetStatus == 0)
            {

                AView.SetFlash(false);
                BView.SetFlash(false);

            }
            else if (comp.C4SetStatus == 1)
            {

                if (adapter.IsCampPass())
                {
                    AView.SetFlash(true);
                    BView.SetFlash(false);
                }
                else
                {
                    AView.SetFlash(true);
                    BView.SetFlash(true);
                }
            }
            else if (comp.C4SetStatus == 2)
            {

                if (adapter.IsCampPass())
                {
                    AView.SetFlash(false);
                    BView.SetFlash(true);
                }
                else
                {
                    AView.SetFlash(true);
                    BView.SetFlash(true);
                }
            }


            if (adapter.IsCampPass())
            {
                AView.SetAttack(true);
                BView.SetAttack(true);
            }
            else
            {
                AView.SetAttack(false);
                BView.SetAttack(false);
            }
            //Debug.Log("C4"+comp.IsC4Droped);
        }

        private void UpdateAB(Vector3 targetPos, ABType type)
        {


            if (InView(targetPos))
            {

                var selfPlayer = adapter.GetPlayerEntity();
                var playerTrans = selfPlayer.characterContoller.Value.transform;

                var distance = Mathf.FloorToInt(Vector3.Distance(targetPos, playerTrans.position));
                var mi = "m";


                var camera = Camera.main;
                var result = UIUtils.WorldPosToRect(targetPos, camera, rootRect);

                switch (type)
                {
                    case ABType.A:
                        _viewModel.subrootA.gameObject.SetActive(true);
                        AView.tittle.text = distance + mi;
                        AView.rect.anchoredPosition = new Vector3(result.x, result.y, 0);
                        break;
                    case ABType.B:
                        _viewModel.subrootB.gameObject.SetActive(true);
                        BView.tittle.text = distance + mi;
                        BView.rect.anchoredPosition = new Vector3(result.x, result.y, 0);
                        break;
                    case ABType.C4:
                        _viewModel.subrootC4.gameObject.SetActive(true);
                        C4View.tittle.text = distance + mi;
                        C4View.rect.anchoredPosition = new Vector3(result.x, result.y, 0);
                        break;
                    default: break;
                }


            }
            else
            {
                switch (type)
                {
                    case ABType.A:
                        _viewModel.subrootA.gameObject.SetActive(false);
                        break;
                    case ABType.B:
                        _viewModel.subrootB.gameObject.SetActive(false);
                        break;
                    case ABType.C4:
                        _viewModel.subrootC4.gameObject.SetActive(false);
                        break;
                    default: break;
                }

            }

        }



        public override void Update(float interval)
        {
            base.Update(interval);

            RefreshGui();

        }


        private bool InView(Vector3 pos)
        {
            var cam = Camera.main;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(pos);
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InitKeyBinding()
        {

        }

        void HideAll()
        {
            _viewModel.subrootA.gameObject.SetActive(false);
            _viewModel.subrootB.gameObject.SetActive(false);
            _viewModel.subrootC4.gameObject.SetActive(false);
        }

        class TipsView
        {
            public UIText tittle;
            UIText action;
            GameObject go;
            GameObject redicon;
            public RectTransform rect;

            string attackWord = LocalizationManager.GetTranslation("client/blast/word13");
            string guardWord = LocalizationManager.GetTranslation("client/blast/word14");

            public TipsView(GameObject go)
            {
                this.go = go;
            }

            public void InitAB()
            {
                action = go.transform.Find("action").gameObject.GetComponent<UIText>();
                tittle = go.transform.Find("tittle").gameObject.GetComponent<UIText>();
                redicon = go.transform.Find("iconred").gameObject;
                rect = go.GetComponent<RectTransform>();
            }
            public void InitC4()
            {
                tittle = go.transform.Find("tittle").gameObject.GetComponent<UIText>();
                redicon = go.transform.Find("iconred").gameObject;
                rect = go.GetComponent<RectTransform>();
            }

            public void SetAttack(bool flag)
            {
                if (flag)
                {
                    action.text = attackWord;
                    Color c;
                    ColorUtility.TryParseHtmlString("#fd444499", out c);
                    action.color = c;
                }
                else
                {
                    action.text = guardWord;
                    Color c;
                    ColorUtility.TryParseHtmlString("#48b8ff99", out c);
                    action.color = c;
                }
            }

            public void SetFlash(bool flag)
            {
                redicon.SetActive(flag);
            }

        }

    }
}
