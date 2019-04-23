using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;


namespace XmlConfig.BootConfig
{
    [Serializable]
    public class ResourceConfig
    {
        public string BaseUrl;
        public AssetBundleLoadingPattern BasePattern;
        public string Manifests;
        public List<SupplementaryResource> Supplement;

        public override string ToString()
        {
            return string.Format("BaseUrl: {0}, BasePattern: {1}, Manifests: {2}, Supplement: {3}", BaseUrl, BasePattern, Manifests, Supplement);
        }
    }
    [Serializable]
    public class SupplementaryResource
    {
        public string BundleName;
        public AssetBundleLoadingPattern Pattern;
        public string Manifests;
        public string Url;

        public override string ToString()
        {
            return string.Format("BundleName: {0}, Pattern: {1}, Manifests: {2}, Url: {3}", BundleName, Pattern, Manifests, Url);
        }
    }
}
