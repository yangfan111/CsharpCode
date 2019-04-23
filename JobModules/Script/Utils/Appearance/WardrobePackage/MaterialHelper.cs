using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace Utils.Appearance.WardrobePackage
{
    public static class MaterialHelper
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MaterialHelper));
        public static void SetParam(Material material, SkinParam param)
        {
            switch ((ShaderType)param.Type)
            {
                    case ShaderType.HairType:
                        SetHairTypeParam(material, param);
                        break;
                    
                    case ShaderType.EndOfWorld:
                        Logger.ErrorFormat("un support type");
                        break;
                    default:
                        Logger.ErrorFormat("un support type");
                        break;
            }
        }

        public static void SetHairTypeParam(Material material, SkinParam param)
        {
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", param.MainColor);
            }
            else
            {
                Logger.ErrorFormat("type:{0}, do not has property:{1}", (ShaderType)param.Type, "_MainColor");
            }
                            
            if (material.HasProperty("_SpecularColor"))
            {
                material.SetColor("_SpecularColor", param.SpecularColor);

            }
            else
            {
                Logger.ErrorFormat("type:{0}, do not has property:{1}", (ShaderType)param.Type, "_SpecularColor");
            }
       
            if (material.HasProperty("_SpecularColor2"))
            {
                material.SetColor("_SpecularColor2", param.SecondarySpecularColor);
            }
            else
            {
                Logger.ErrorFormat("type:{0}, do not has property:{1}", (ShaderType)param.Type, "_SecondarySpecularColor");
            }
        }
    }
}