using System;
using DG.Tweening;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public partial class CommonWeaponHudModel
    {
        private void InitTweener()
        {
            InitBulletChangeAnime();
            InitSlotChangeAnime(SlotChangePosAnimeTime);
        }

        private void InitSlotChangeAnime(int playAnimeCount)
        {
            if (playAnimeCount == 1)
            {
                InitSlotChangeAnime(SlotChangePosAnimeTime / playAnimeCount);
            }
            else if (playAnimeCount == 2)
            {
                Sequence tmp = DOTween.Sequence();
                InitSlotChangeAnime(SlotChangePosAnimeTime / playAnimeCount);
                _slotChangeAnime.onComplete = () =>
                {
                    //var choice = _weaponSlotChoice + 1;
                    var choice = StyleDict[(WeaponHudStyleType)GetSlotByIndex(_weaponSlotChoice)].index + 1;
                    ClipIndex(ref choice);
                    UpdateSlotPosIndex(choice);
                    UpdateStyle();
                };
                tmp.Append(_slotChangeAnime);
                InitSlotChangeAnime(SlotChangePosAnimeTime / playAnimeCount);
                tmp.Append(_slotChangeAnime);
                _slotChangeAnime = tmp;
            }
            else if (playAnimeCount == -1)
            {
                InitSlotChangeAnime(-SlotChangePosAnimeTime / playAnimeCount, false);
            }
            else if (playAnimeCount == -2)
            {
                Sequence tmp = DOTween.Sequence();
                InitSlotChangeAnime(-SlotChangePosAnimeTime / playAnimeCount,false);
                _slotChangeAnime.onComplete = () =>
                {
                    //var choice = _weaponSlotChoice - 1;
                    var choice = StyleDict[(WeaponHudStyleType) GetSlotByIndex(_weaponSlotChoice)].index - 1;
                    ClipIndex(ref choice);
                    UpdateSlotPosIndex(choice);
                    UpdateStyle();
                };
                tmp.Append(_slotChangeAnime);
                InitSlotChangeAnime(-SlotChangePosAnimeTime / playAnimeCount,false);
                tmp.Append(_slotChangeAnime);
                _slotChangeAnime = tmp;
            }
        }

        private void InitSlotChangeAnime(float time,bool isBackward = true)
        {
            _slotChangeAnime = DOTween.Sequence();
            int interval = 0;
            if (isBackward)
            {
                interval = 1;
            }
            else
            {
                interval = -1;
            }
            foreach (WeaponHudStyleType type in Enum.GetValues(typeof(WeaponHudStyleType)))
            {
                if ((type == WeaponHudStyleType.Bottom && isBackward) ||
                    (type == WeaponHudStyleType.Top && !isBackward))
                {
                    _slotChangeAnime.Insert(0, InitSlotChangeBorderAnime(type, time));
                }
                else
                {
                    _slotChangeAnime.Insert(0, InitSlotChangePosAnime(type, time, interval));
                    _slotChangeAnime.Insert(0, InitSlotChangeAlphaAnime(type, time, interval));
                    _slotChangeAnime.Insert(0, InitSlotChangeScaleAnime(type, time, interval));
                }
            }

            _slotChangeAnime.onComplete = ()=>
            {
                UpdateSlotPosIndex(_weaponSlotChoice);
                UpdateSlotStyle();
            };
            _slotChangeAnime.SetRecyclable(true);
            _slotChangeAnime.Pause();
        }

        private Tween InitSlotChangeBorderAnime(WeaponHudStyleType slot,float time)
        {
            return DOTween.To(() => GetSlotLocalScale(slot), x => SetSlotScale(slot, Vector3.zero),
                Vector3.zero, time);
        }

        private Tween InitSlotChangeScaleAnime(WeaponHudStyleType slot,float time,int interval = 1)
        {
            return DOTween.To(() => GetSlotLocalScale(slot), x => SetSlotScale(slot, x),
                GetSlotLocalScale(slot + interval), time);
        }

        private Tween InitSlotChangeAlphaAnime(WeaponHudStyleType slot,float time,int interval = 1)
        {
            return DOTween.To(() => GetSlotLocalAlpha(slot), x => SetSlotAlpha(slot, x),
                GetSlotLocalAlpha(slot + interval), time);
        }

        private Tween InitSlotChangePosAnime(WeaponHudStyleType slot,float time, int interval = 1)
        {
            return DOTween.To(() => GetSlotLocalPos(slot), x => SetSlotPos(slot, x),
                GetSlotLocalPos(slot + interval), time);

        }


        private void InitBulletChangeAnime()
        {
            float toAlpha = 0.5f;
            float toScale = 0.9f;
            float time = 0.1f;
            var alphaTweener = DOTween.To(() => _viewModel.BulletAlapha, x => _viewModel.BulletAlapha = x, toAlpha, time);

            float scale = 1f;
            var scaleTweener = DOTween.To(() => scale, x => _viewModel.BulletScale = x * Vector3.one, toScale, time);

            _bulletChangeAnime = DOTween.Sequence();
            _bulletChangeAnime.Insert(0, alphaTweener);
            _bulletChangeAnime.Insert(0, scaleTweener);
            _bulletChangeAnime.Pause();
            _bulletChangeAnime.SetAutoKill(false);
            _bulletChangeAnime.onComplete = () => { _bulletChangeAnime.Rewind(); };
            _bulletChangeAnime.onRewind = () => {
                _viewModel.BulletCountString = (_lastBulletCount.ToString());
                _lastBulletCount = _curBulletCount;
            };
        }

        private float _slotChangePosAnimeTime = 0;
        public float SlotChangePosAnimeTime
        {
            get { return _slotChangePosAnimeTime; }
            set { _slotChangePosAnimeTime = value; }
        }

        private void PlaySlotChangeAnime(int beforeChoice, int afterChoice)
        {
            if (_slotChangeAnime.IsPlaying())
            {
                return;
            }
            //if (beforeChoice * afterChoice == 0)
            if(afterChoice == 0 || (beforeChoice == 0 && !GetSlotTf(WeaponHudStyleType.Center).gameObject.activeSelf))
            {           
                UpdateSlotPosIndex(afterChoice);
                return;//收枪、拔枪，武器槽实际上没改变，不播放动画
            }
            int beforeSlot = 0;
            int afterSlot = 0;
            beforeSlot = GetSlotByIndex(beforeChoice);
            afterSlot = GetSlotByIndex(afterChoice);
            if (beforeSlot == 0)
            {
                beforeSlot = (int)WeaponHudStyleType.Center;
            }

            int playAnimeCount = (beforeSlot - afterSlot);
            InitSlotChangeAnime(playAnimeCount);
            _slotChangeAnime.Restart();
        }

        private void PlayBulletCountDecreaseAnime()
        {
            _bulletChangeAnime.Restart();
        }
    }
}
