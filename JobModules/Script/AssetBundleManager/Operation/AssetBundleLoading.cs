using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManagement;

namespace AssetBundleManager.Operation
{
    abstract class AssetBundleLoading : PollingOperation
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public LoadedAssetBundle Content { get; protected set; }
        public string Error { get; protected set; }

        private HashSet<string> _dependencies;

        protected AssetBundleLoading(string assetBundleName, string url)
        {
            if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(url))
                throw new ArgumentNullException(string.Format("assetbundle: {0}, url: {1}", assetBundleName ?? "null", url ?? "null"));

            Name = assetBundleName;
            Url = url;
        }

        public abstract IEnumerator Cancel();

        public void AddDependency(string name)
        {
            if (_dependencies == null)
                _dependencies = new HashSet<string>();

            _dependencies.Add(name);
        }

        public void RemoveDependency(string name)
        {
            if (_dependencies != null)
                _dependencies.Remove(name);
        }

        public bool NoMoreDependencies()
        {
            return _dependencies == null || _dependencies.Count == 0;
        }
    }
}