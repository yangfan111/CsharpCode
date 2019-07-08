using UnityEngine;
using XmlConfig;


namespace App.Shared.Audio
{
    internal class AudioDispatcher
    {
        //     internal AudioRegulator Regulator { get; private set; }
        private readonly AudioBankLoader bankLoader;

        private readonly AKTypesController typesController = new AKTypesController();

        internal void Free()
        {
            typesController.Free();
        }
        internal AudioDispatcher(AudioBankLoader loader)
        {
            bankLoader = loader;
        }

        internal void PrepareEvent(int eventId, AkGameObj target)
        {
        }

        internal void PostEvent(AudioEventItem econfig, AkGameObj target, bool skipSwitchSetting = false)
        {
            bankLoader.LoadAtom(econfig.BankRef, false, (result) =>
            {
                if (AudioUtil.VerifyAKResult(result, "Audio load atom:" + econfig.BankRef))
                {
                    LoadBankResultHandler(econfig, target, skipSwitchSetting);
                }
            });
        }

        private void LoadBankResultHandler(AudioEventItem econfig, AkGameObj target, bool skipSwitchSetting = false)
        {
            var audioEmitter = typesController.GetEmitter(target);
            audioEmitter.PostEvent(econfig, skipSwitchSetting);
        }

        internal void StopEvent(AudioEventItem econfig, AkGameObj target)
        {
            var result = AkSoundEngine.ExecuteActionOnEvent(econfig.Event, AkActionOnEventType.AkActionOnEventType_Stop,
                target.gameObject, 0, AkCurveInterpolation.AkCurveInterpolation_Linear);
            AudioUtil.VerifyAKResult(result, "StopEvent:" + econfig.Event);
        }

        #region overrride method for switchgroup


        internal void SetSwitch(AkGameObj target, AudioGrp_ShotMode shotModelGrpIndex)
        {
            SetSwitch(target, (int) AudioGrp_ShotMode.Id, (int) shotModelGrpIndex);
        }

        internal void SetSwitch(AkGameObj target, AuidoGrp_RefShotMode shotModelGrpIndex)
        {
            SetSwitch(target, (int) AuidoGrp_RefShotMode.Id, (int) shotModelGrpIndex);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_MeleeAttack meleeAttack)
        {
            SetSwitch(target, (int) AudioGrp_MeleeAttack.Id, (int) meleeAttack);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_BulletType bulletType)
        {
            SetSwitch(target, (int) AudioGrp_BulletType.Id, (int) bulletType);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_HitMatType hitMatType)
        {
            SetSwitch(target, (int) AudioGrp_HitMatType.Id, (int) hitMatType);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_FootMatType footMatType)
        {
            SetSwitch(target, (int) AudioGrp_FootMatType.Id, (int) footMatType);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_Magazine magIndex)
        {
            SetSwitch(target, (int) AudioGrp_Magazine.Id, (int) magIndex);
        }

        internal void SetSwitch(AkGameObj target, AudioGrp_Footstep stepIndex)
        {
            SetSwitch(target, (int) AudioGrp_Footstep.Id, (int) stepIndex);
        }

        private void SetSwitch(AkGameObj target, int grpId, int index)
        {
            var emitterData = typesController.GetEmitter(target);
            emitterData.SetSwitch(grpId, index);
        }
        #endregion

        #region todo method

        

        internal void PrepareEvent(int eventId)
        {
        }

        internal void PrepareBank(string bankName)
        {
        }

        void OnAsyncBnkLoadSucess(string bankName)
        {
        }

        void OnAsyncBnkALoadRefUpdate(string bankName)
        {
        }

        void OnAsyncBnkALoadFail(string bankName)
        {
        }
        #endregion

    }
}