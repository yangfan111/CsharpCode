using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AssetBundleManagement;
using AssetBundleManager.Operation;
using AssetBundles;
using Core.Utils;
using UnityEngine;


namespace AssetBundleManager.Warehouse
{
    abstract class AssetBundleWarehouse
    {
        private readonly string _manifestBundleName;
        private Dictionary<string, HashSet<string>> _bundlesWithVariant = new Dictionary<string, HashSet<string>>();
        private string[] _activeVariants = { };
        private readonly bool _isSceneQuantityLevel;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AssetBundleWarehouse));
        private Dictionary<string, string> _bundlesEndofLQ = new Dictionary<string, string>();

        protected AssetBundleWarehouse(string manifestBundleName, bool isLow)
        {
            _manifestBundleName = manifestBundleName;
            _isSceneQuantityLevel = isLow;
        }

        public virtual InitRetValue Init()
        {
            return new InitRetValue()
            {
                AssetBundleLoadingOperation = LoadAssetBundle(_manifestBundleName),
                ManifestLoadingOperation =
                    OperationFactory.CreateManifestLoading(_manifestBundleName, "AssetBundleManifest")
            };
        }

        public virtual void SetManifest(AssetBundleManifest obj)
        {
            foreach (var v in obj.GetAllAssetBundlesWithVariant())
            {
                var splits = v.Split('.');
                if (!_bundlesWithVariant.ContainsKey(splits[0]))
                    _bundlesWithVariant.Add(splits[0], new HashSet<string>());

                _bundlesWithVariant[splits[0]].Add(splits[1]);
            }

            var _allAssetBundle = obj.GetAllAssetBundles();
            if (_allAssetBundle.Length == 0)
            {
                _logger.InfoFormat("!!!!!!!!!!!! the _allAssetBundle is null !!!!!!!!!!!!!");
            }
            else
            {
                _logger.InfoFormat("!*!*!*!*!*!*!*!* _allAssetBundle.Length={0} ********", _allAssetBundle.Length);
            }

            HashSet<string> bundles = new HashSet<string>();
            for (int i = 0; i < _allAssetBundle.Length; i++)
            {
                bundles.Add(_allAssetBundle[i]);
            }


            for (int j = 0; j < _allAssetBundle.Length; j++)
            {
                var temp = _allAssetBundle[j];
                //_logger.InfoFormat("!*!*!*!*!*!*!*!* {0}   {1} !!!!!!!!",temp,temp1);
                if (bundles.Contains(string.Format("{0}_lq", temp)))
                {
                    if (!_bundlesEndofLQ.ContainsKey(temp))
                    {
                        _bundlesEndofLQ.Add(temp, string.Format("{0}_lq", temp));
                        _logger.InfoFormat("********{0}******{1}", temp, string.Format("{0}_lq", temp));
                    }
                }
            }

            if (_bundlesEndofLQ.ContainsKey("level"))
            {
                _bundlesEndofLQ.Remove("level");
            }
        }

        public static FailLoading LoadFailed(AssetLoadingPattern LoadingPattern,bool isSceneLoading, string bundleName, string name, Type objectType)
        {
            return new FailLoading(LoadingPattern,isSceneLoading, bundleName, name, objectType);
        }

        public abstract AssetBundleLoading LoadAssetBundle(string name);

        public abstract AssetLoading LoadAsset(string bundleName, string name, Type objectType);

        public abstract SceneLoading LoadScene(string bundleName, string name, bool isAdditive);

        public virtual string RemapBundleName(string name)
        {
            var baseName = Utility.GetNameWithoutVariant(name);

            if (_bundlesWithVariant.ContainsKey(baseName))
            {
                for (int i = 0; i < _activeVariants.Length; i++)
                {
                    if (_bundlesWithVariant[baseName].Contains(_activeVariants[i]))
                        return string.Format("{0}.{1}", baseName, _activeVariants[i]);
                }

                // If there is no active variant found. We still want to use the first
                return string.Format("{0}.{1}", baseName, _bundlesWithVariant[baseName].First());
            }


            if (_isSceneQuantityLevel && _bundlesEndofLQ.ContainsKey(name))
            {
                _logger.InfoFormat("********* low qulityasset {0} load  ********* ", name);
                return _bundlesEndofLQ[name];
            }

            return name;
        }

        public void SetActiveVariants(string[] activeVariants)
        {
            _activeVariants = activeVariants;
        }


        // [PPAN] ½âÎöMD5 AssetBundlesÏà¹ØÂß¼­
        private Dictionary<string, string> dictAssetBundlesMD5 = new Dictionary<string, string>();
        protected void LoadAssetBundlesXML(string strOriMD5Path, string strAssetBundleXML)
        {
            string strAssetBundlePath = strOriMD5Path + strAssetBundleXML;

            using (StreamReader reader = File.OpenText(strAssetBundlePath))
            {
                XmlDocument xmlAssetBundles = new XmlDocument();
                xmlAssetBundles.Load(reader);

                XmlElement rootElem = xmlAssetBundles.DocumentElement;
                for (int i = 0; i < rootElem.ChildNodes.Count; i++)
                {
                    XmlElement buildElem = (XmlElement)rootElem.ChildNodes[i];
                    string abName = buildElem.GetAttribute("name");
                    string md5Num = buildElem.GetAttribute("md5");
                    dictAssetBundlesMD5.Add(abName, md5Num);
                }
            }
        }

        protected string ReadMD5FromXL(string strOriABName)
        {
            if (dictAssetBundlesMD5.ContainsKey(strOriABName))
                return dictAssetBundlesMD5[strOriABName];
            else
                return string.Empty;
        }
    }
}