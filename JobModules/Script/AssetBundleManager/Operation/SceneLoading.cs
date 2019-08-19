using AssetBundleManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetBundleManager.Operation
{
   public class SceneLoading : AssetLoading
    {
        private bool _isAdditive;
        private SynchronizationMode _mode;
        private AsyncOperation _asyncLoadRequest;

        public SceneLoading(string bundleName, string assetName, SynchronizationMode mode, bool isAdditive)
            : base(AssetLoadingPattern.Scene, bundleName, assetName, null)
        {
            _mode = mode;
            _isAdditive = isAdditive;
            _asyncLoadRequest = null;

            AssetGroup = AssetGroup.Scene;
        }

        public override bool IsLoadFailed
        {
            get { return _asyncLoadRequest == null; }
        }

        public AsyncOperation AsyncLoadRequest
        {
            get
            {
                return _asyncLoadRequest;
            }

            set
            {
                _asyncLoadRequest = value;
            }
        }

        public override void SetAssetBundle(LoadedAssetBundle assetBundle)
        {
            switch (_mode)
            {
                case SynchronizationMode.Async:
                    _asyncLoadRequest =  SceneManager.LoadSceneAsync(Name, _isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                    break;
                case SynchronizationMode.Sync:
                    SceneManager.LoadScene(Name, _isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                    break;
#if UNITY_EDITOR
                case SynchronizationMode.Simulated:
                    string[] levelPaths =
                        UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(BundleName, Name);
                    if (levelPaths.Length != 0)
                    {
                        if (_isAdditive)
                            _asyncLoadRequest = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
                        else
                            _asyncLoadRequest = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
                    }
                    break;
#endif
            }
            
        }
        
        public override bool IsDone()
        {
            return _asyncLoadRequest == null || _asyncLoadRequest.isDone;
        }

        public override void Process()
        { }
    }
}