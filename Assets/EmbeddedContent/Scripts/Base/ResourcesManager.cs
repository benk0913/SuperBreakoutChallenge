using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SuperBreakout
{
    public class ResourcesManager : MonoBehaviour
    {
        #region  Parameters

        public bool IsPrewarming
        {
            get
            {
                return _prewarmRoutineInstance != null;
            }
        }

        public static ResourcesManager Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("Resources Manager not yet initialized!");

                    return null;
                }

                return _instance;
            }
        }

        static ResourcesManager _instance;
        static bool _isInitialized;

        [SerializeField]
        bool _prewarmOnStart;

        [SerializeField]
        List<string> _prewarmAssets = new List<string>();

        Dictionary<string, object> _loadedAssets = new Dictionary<string, object>();

        Dictionary<string, List<GameObject>> _generalObjectPool = new Dictionary<string, List<GameObject>>();

        Coroutine _prewarmRoutineInstance;

        #endregion

        #region Frame Lifecycle
        void Awake()
        {
            _instance = this;
            _isInitialized = true;
        }
        void Start()
        {
            if (_prewarmOnStart)
            {
                if (_prewarmRoutineInstance != null) StopCoroutine(_prewarmRoutineInstance);

                _prewarmRoutineInstance = StartCoroutine(PrewarmRoutine());
            }
        }
        #endregion

        #region Public Interface

        public void LoadFromPool(string resourceKey, Action<GameObject> onComplete = null)
        {
            if (!_generalObjectPool.ContainsKey(resourceKey))
            {
                _generalObjectPool.Add(resourceKey, new List<GameObject>());
                LoadObject(resourceKey, (GameObject loadedObject) =>
                {
                    loadedObject = Instantiate(loadedObject);
                    _generalObjectPool[resourceKey].Add(loadedObject);
                    onComplete?.Invoke(loadedObject);
                });

                return;
            }

            GameObject foundObject = _generalObjectPool[resourceKey].Find(x => !x.activeInHierarchy);

            if (foundObject == null)
            {
                LoadObject(resourceKey, (GameObject loadedObject) =>
                  {
                      loadedObject = Instantiate(loadedObject);
                      _generalObjectPool[resourceKey].Add(loadedObject);
                      onComplete?.Invoke(loadedObject);

                  });

                return;
            }

            foundObject.SetActive(true);
            onComplete?.Invoke(foundObject);

        }

        public void LoadObject(string resourceKey, Action<GameObject> onComplete = null)
        {
            LoadAsset(resourceKey, (object asset) =>
            {
                onComplete?.Invoke((GameObject)asset);
            });
        }

        public void LoadSound(string resourceKey, Action<AudioClip> onComplete = null)
        {
            LoadAsset(resourceKey, (object asset) =>
            {
                onComplete?.Invoke((AudioClip)asset);
            });
        }


        public void LoadAsset(string resourceKey, Action<object> onComplete = null)
        {
            try
            {
                if (_loadedAssets.ContainsKey(resourceKey))
                {
                    onComplete?.Invoke(_loadedAssets[resourceKey]);
                    return;
                }

                StartCoroutine(LoadObjectAsyncRoutine(resourceKey, onComplete));
            }
            catch
            {
                Debug.LogError("Encountered an error during the load of asset: " + resourceKey);
                onComplete?.Invoke(null);
            }
        }


        #endregion

        #region Internal

        IEnumerator LoadObjectAsyncRoutine(string resourceKey, Action<object> onComplete = null)
        {
            // LoadingWindow.Instance.AddLoader(this);

            AsyncOperationHandle<object> objectOpHandle;
            objectOpHandle = Addressables.LoadAssetAsync<object>(resourceKey);

            yield return objectOpHandle;

            // LoadingWindow.Instance.RemoveLoader(this);

            if (!_loadedAssets.ContainsKey(resourceKey))
            {
                _loadedAssets.Add(resourceKey, objectOpHandle.Result);
                onComplete?.Invoke(objectOpHandle.Result);
            }
            else
            {
                onComplete?.Invoke(_loadedAssets[resourceKey]);
            }
        }


        IEnumerator PrewarmRoutine()
        {
            UILoadingWindow.Instance.AddLoader(UILoadingWindow.CommonLoadOperations.PREWARM_ASSETS);
            for (int i = 0; i < _prewarmAssets.Count; i++)
            {
                string resourceKey = _prewarmAssets[i];

                if (_loadedAssets.ContainsKey(resourceKey))
                {


                    AsyncOperationHandle<GameObject> objectOpHandle;
                    objectOpHandle = Addressables.LoadAssetAsync<GameObject>(resourceKey);

                    yield return objectOpHandle;



                    _loadedAssets.Add(resourceKey, objectOpHandle.Result);
                }
            }

            yield return 0;

            UILoadingWindow.Instance.RemoveLoader(UILoadingWindow.CommonLoadOperations.PREWARM_ASSETS);
        }

        #endregion
    }

}