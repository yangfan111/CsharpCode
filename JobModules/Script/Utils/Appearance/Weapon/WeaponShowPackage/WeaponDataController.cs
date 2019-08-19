using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class WeaponDataController
    {
        private const int InvalidId = UniversalConsts.InvalidIntId;
        
        private readonly int[] _weaponIds = new int[(int) WeaponInPackage.EndOfTheWorld];
        private readonly int[,] _attachmentIds = new int[(int) WeaponInPackage.EndOfTheWorld, (int)WeaponPartLocation.EndOfTheWorld];
        private WeaponInPackage _handWeaponType = WeaponInPackage.EmptyHanded;
        private OverrideControllerState _overrideControllerType = OverrideControllerState.Null;

        public void SetWeaponIdByType(WeaponInPackage type, int value)
        {
            _weaponIds[(int) type] = value;
        }
        
        public int[] GetWeaponIds()
        {
            return _weaponIds;
        }

        public void SetAttachmentIdByType(WeaponInPackage weaponType, WeaponPartLocation attachmentType, int id)
        {
            _attachmentIds[(int) weaponType, (int) attachmentType] = id;
        }

        public int[,] GetAttachmentIds()
        {
            return _attachmentIds;
        }
        
        public void SetHandWeaponType(WeaponInPackage type)
        {
            _handWeaponType = type;
        }

        public WeaponInPackage GetHandWeaponType()
        {
            return _handWeaponType;
        }

        public void SetChangeOverrideControllerType(OverrideControllerState type)
        {
            _overrideControllerType = type;
        }

        public OverrideControllerState GetChangeOverrideControllerType()
        {
            return _overrideControllerType;
        }

        public void Init()
        {
            for (var i = 0; i < _weaponIds.Length; ++i)
            {
                _weaponIds[i] = InvalidId;
            }

            for (var i = 0; i < _attachmentIds.GetLength(0); ++i)
            {
                for (var j = 0; j < _attachmentIds.GetLength(1); ++j)
                {
                    _attachmentIds[i, j] = InvalidId;
                }
            }

            _handWeaponType = WeaponInPackage.EmptyHanded;
            _overrideControllerType = OverrideControllerState.Null;
        }
    }
}
