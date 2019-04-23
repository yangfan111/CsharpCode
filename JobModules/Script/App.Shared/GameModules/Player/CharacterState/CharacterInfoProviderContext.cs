using System.Collections.Generic;
using System.Security.Policy;
using Core.CharacterState;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterState
{


    public class CharacterInfoProvider : ICharacterInfoProvider
    {
        public float JumpSpeed;
        public CharacterControllerCapsule StandCapsule;
        public CharacterControllerCapsule CrouchCapsule;
        public CharacterControllerCapsule ProneCapsule;
        public float BigJumpHeight;

        #region ICharacterSpeedInfo

        public float GetJumpSpeed()
        {
            return JumpSpeed;
        }

        #endregion


        #region ICharacterBoundsInfo

        public CharacterControllerCapsule GetStandCapsule()
        {
            return StandCapsule;
        }

        public CharacterControllerCapsule GetCrouchCapsule()
        {
            return CrouchCapsule;
        }

        public CharacterControllerCapsule GetProneCapsule()
        {
            return ProneCapsule;
        }

        public float GetBigJumpHeight()
        {
            return BigJumpHeight;
        }

        #endregion
        
    }
    
    public class CharacterInfoProviderContext:ICharacterInfoProviderContext
    {
        private int _type = int.MinValue;
        private ICharacterInfoProvider _currentInfo;
        private static ICharacterInfoProvider _defaultInfo = new CharacterInfoProvider()
        {
            JumpSpeed = 3.4f,
            BigJumpHeight = 1.428f,
            StandCapsule = new CharacterControllerCapsule()
            {
                Height = 1.75f,
                Radius = 0.4f,
                Posture = PostureInConfig.Stand
            },
            CrouchCapsule = new CharacterControllerCapsule()
            {
                Height = 1.2f,
                Radius = 0.4f,
                Posture = PostureInConfig.Crouch
            },
            ProneCapsule = new CharacterControllerCapsule()
            {
                Height = 1.75f,
                Radius = 0.4f,
                Posture = PostureInConfig.Prone
            },
        };
        private Dictionary<int, ICharacterInfoProvider> _dict = new Dictionary<int, ICharacterInfoProvider>();
        private static readonly int DefaultId = 0;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterInfoProviderContext));

        public CharacterInfoProviderContext()
        {
            _dict.Clear();
            SetCurrentType(DefaultId);
            AssertUtility.Assert(_currentInfo != null);
        }


        public float GetJumpSpeed()
        {
            return _currentInfo.GetJumpSpeed();
        }

        public CharacterControllerCapsule GetStandCapsule()
        {
            return _currentInfo.GetStandCapsule();
        }

        public CharacterControllerCapsule GetCrouchCapsule()
        {
            return _currentInfo.GetCrouchCapsule();
        }

        public CharacterControllerCapsule GetProneCapsule()
        {
            return _currentInfo.GetProneCapsule();
        }

        public float GetBigJumpHeight()
        {
            return _currentInfo.GetBigJumpHeight();
        }

        public void SetCurrentType(int type)
        {
            if (type == _type)
            {
                return;
            }
            if (!_dict.ContainsKey(type))
            {
                var info = new CharacterInfoProvider();
                var item = SingletonManager.Get<CharacterInfoManager>().GetCharacterInfoByType(type);
                if (item == null)
                {
                    Logger.ErrorFormat("type:{0} is not exist in item use default config!!!", type);
                    _currentInfo = _defaultInfo;
                    _type = int.MinValue;
                    return;
                }
                else
                {
                    info.JumpSpeed = item.JumpSpeed;
                    info.StandCapsule = new CharacterControllerCapsule()
                    {
                        Height = item.StandHeight,
                        Radius = item.StandRadius,
                        Posture = PostureInConfig.Stand
                    };
                    info.CrouchCapsule = new CharacterControllerCapsule()
                    {
                        Height = item.CrouchHeight,
                        Radius = item.CrouchRadius,
                        Posture = PostureInConfig.Crouch
                    };
                    info.ProneCapsule = new CharacterControllerCapsule()
                    {
                        Height = item.ProneHeight,
                        Radius = item.ProneRadius,
                        Posture = PostureInConfig.Prone
                    };
                    info.BigJumpHeight = item.BigJumpHeight;
                    _dict.Add(type, info);
                }
            }

            _currentInfo = _dict[type];
            Logger.InfoFormat("set type from:{0} to:{1}!!!", _type, type);
            _type = type;
            
        }
    }
}