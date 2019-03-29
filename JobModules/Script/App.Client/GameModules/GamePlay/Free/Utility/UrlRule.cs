namespace Assets.Sources.Free.Utility
{
    public class UrlRule
    {
        public static string GetEffectUrl(string url)
        {
            url = url.Replace(".atf", ".png");
            url = url.Replace(".wmdl", ".DAE");
            return url;

//            if (url.IndexOf("/effect") == 0)
//            {
//                return url.Substring(7);
//            }
//            else if (url.IndexOf("common/") >= 0 ||
//               url.IndexOf("noload/") >= 0 ||
//               url.IndexOf("pvecommon/") >= 0 ||
//               url.IndexOf("spary/") >= 0 ||
//               url.IndexOf("halfloadeffect/") >= 0)
//            {
//
//                return url;
//            }
//
//            return GameModelLocator.GetInstance().GameModel.RoomData.FreeType + "/" + url;
        }
    }
}
