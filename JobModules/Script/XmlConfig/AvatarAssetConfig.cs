using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class AvatarAssetItem
    {
        public int Id;
        public string Name;
        public int NameId;
        public bool IsSkinned = true;
        public int AvatarType = 0;
        public string BundleName;
        public string AssetName;
        public string SecondRes;
        public bool HaveSecond = false;
        public int SecondType;
        public bool HaveHideAvatar = false;
        public List<int> HideAvatars;
        public string BundleNameInside;
        public bool HaveP1Avatar = false;
        public bool NeedMapping = false;
        public int Default;
        public int Sex;
    }

    [XmlRoot("root")]
    public class AvatarAssetConfig
    {
        public AvatarAssetItem[] Items;
    }
    
    #region original config

    [Serializable]
    public class SkinParam
    {
        public int SourceId;
        public int Type;
        public Color32 MainColor;
        public Color32 SpecularColor;
        public Color32 SecondarySpecularColor;
    }

    public enum ShaderType
    {
        HairType = 0,
        EndOfWorld
    }
    #endregion

    #region avatarskin for design

    [XmlRoot("root")]
    public class AvatarSkinConfig
    {
        public AvatarSkinItem[] Items;
    }

    [XmlType("child")]
    public class AvatarSkinItem
    {
        public int Id;
        public int TintId;
        public int SourceId;
        public int Type;
        public List<int> MainColor;
        public List<int> SpecularColor;
        public List<int> SecondarySpecularColor;

        public override string ToString()
        {
            return string.Format("Id: {0}, TintId: {1}, SourceId: {2}, Type: {3}, MainColor: {4}, SpecularColor: {5}, SecondarySpecularColor: {6}", Id, TintId, SourceId, Type, GetArrayString(MainColor), GetArrayString(SpecularColor), GetArrayString(SecondarySpecularColor));
        }

        private string GetArrayString(List<int> array)
        {
            return string.Join(",", array.Select(i => i.ToString()).ToArray());
        }
    }
    

    #endregion
    
    
}
