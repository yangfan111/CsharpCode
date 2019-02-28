using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using DG.Tweening;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonTaskTipModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonTaskTipModel));
        private CommonTaskTipViewModel _viewModel = new CommonTaskTipViewModel();
        private ITaskTipUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public long SingleItemShowTime //毫秒
        {
            get { return 3000; }
        }

        public CommonTaskTipModel(ITaskTipUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
            InitAnim();
        }

        private void Test()
        {
            var list = _adapter.TaskTipDataList;
            var testData = new TaskTipData();
            testData.Title = I2.Loc.ScriptLocalization.client_common.word20;
            testData.OldProgress = 1;
            testData.NewProgress = 15;
            testData.TotalProgress = 16;

            if (DateTime.Now.Second ==  0)
            {
                list.Add(testData);
            }
        }

        private void InitAnim()
        {
            _closeViewAnim = DOTween.To(() => _viewModel.Alpha, (x) => _viewModel.Alpha = x, 0, CloseViewAnimTime);
            _closeViewAnim.SetAutoKill(false);
            _closeViewAnim.Pause();
            _closeViewAnim.onComplete = () => { isNeedUpdateQueue = true; };
        }

        private void InitGui()
        {
        }

        private void InitVariable()
        {
            _viewModel.Alpha = 1;
            
        }


        public override void Update(float intervalTime)
        {
            UpdateTipQueue();
            UpdateRemainTime();
            UpdateCurShowTip();
            //Test();
        }

        private void UpdateCurShowTip()
        {
            if (!isNeedUpdateShowTip)
            {
                return;
            }

            beginTime = DateTime.Now.Ticks / 10000;
            if (tipList.Count > 0)
            {
                var item = tipList[0];
                _viewModel.TitleText = item.Title;
                _viewModel.ScheduleText = string.Format("{0}/{1}", item.NewProgress, item.TotalProgress);
                if (item.NewProgress == item.TotalProgress)
                {
                    isNeedShowCompleteAnim = true;
                }
                PlayScheduleChangeAnime(item.OldProgress, item.NewProgress, item.TotalProgress);
                tipList.RemoveAt(0);
            }
            isNeedUpdateShowTip = false;
        }

        private void CancelCloseViewAnime()
        {
            _closeViewAnim.Rewind();
        }

        private void PlayCloseViewAnime()
        {
            if(!_closeViewAnim.IsPlaying() && !_closeViewAnim.IsComplete())
            _closeViewAnim.Restart();
        }

        private void PlayScheduleChangeAnime(int oldProgress, int newProgress, int totalProgress)
        {
            if (_playScheduleChangeAnime != null)
            {
                _playScheduleChangeAnime.Kill();
            }
            var oldVal = (float) oldProgress / totalProgress;
            var newVal = (float) newProgress / totalProgress;
            var time = ScheduleChangeAnimeTime * (newVal - oldVal);
            _viewModel.ProgressVal = oldVal;
            var newAnim = DOTween.To(() => oldVal, (x) => _viewModel.ProgressVal = x, newVal, time);
            newAnim.SetRecyclable(true);
            if (isNeedShowCompleteAnim)
            {
                newAnim.onComplete = ShowCompleteAnim;
            }
            else
            {
                newAnim.onComplete = null;
            }
               
            _playScheduleChangeAnime = newAnim;
        }

        private void ShowCompleteAnim()
        {
            Debug.Log("ShowCompleteAnim,mission art resource");
        }

        private float ScheduleChangeAnimeTime
        {
            get { return 2f; }
        }

        private float CloseViewAnimTime
        {
            get { return 0.4f; }
        }

        private void UpdateRemainTime()
        {
            var curTime = DateTime.Now.Ticks / 10000;
            if (curTime - beginTime > SingleItemShowTime)
            {
                if (tipList.Count > 0)
                {
                    isNeedUpdateShowTip = true;
                }
                else
                {
                    if (_adapter.TaskTipDataList.Count == 0)
                    {
                        PlayCloseViewAnime();
                    }
                    else
                    {
                        isNeedUpdateQueue = true;
                    }
                }
            }

        }

        private void UpdateTipQueue()
        {
            if (!isNeedUpdateQueue)
            {
                return;
            }

            var list = _adapter.TaskTipDataList;
            var count = list.Count;
            if (count <= 0)
            {
                return;
            }
            CancelCloseViewAnime();
            for (int i = 0; i < list.Count; i++)
            {
                tipList.Add(list[i]);
            }

            list.Clear();

        }

        bool isNeedUpdateQueue = true;
        bool isNeedUpdateShowTip = true;
        private bool isNeedCloseView = false;
        long beginTime;//单条任务开始播放时间,毫秒
        private List<ITaskTipData> tipList = new List<ITaskTipData>();
        private Tween _closeViewAnim;
        private Tween _playScheduleChangeAnime;
        private bool isNeedShowCompleteAnim = false;
    }
}
