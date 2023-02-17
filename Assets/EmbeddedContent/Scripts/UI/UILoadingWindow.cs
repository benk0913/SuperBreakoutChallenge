using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{

    public class UILoadingWindow : MonoBehaviour
    {
        #region  Parameters
        public static UILoadingWindow Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("Loading window not yet initialized!");

                    return null;
                }

                return _instance;
            }
        }
        static UILoadingWindow _instance;
        static bool _isInitialized;

        List<string> _operationsNowLoading = new List<string>();
        #endregion

        #region Lifecycle
        void Awake()
        {
            _instance = this;
            _isInitialized = true;
            this.gameObject.SetActive(false);
        }

        #endregion

        #region  Public Interface

        public void AddLoader(string operationKey)
        {
            if(_operationsNowLoading.Contains(operationKey)) 
            {
                Debug.LogError("Already loading operation "+operationKey+"! ");
                return;
            }

            _operationsNowLoading.Add(operationKey);

            this.gameObject.SetActive(true);
        }

        public void RemoveLoader(string operationKey)
        {
            _operationsNowLoading.Remove(operationKey);

            if (_operationsNowLoading.Count == 0) this.gameObject.SetActive(false);
        }

        public class CommonLoadOperations
        {
            public const string LOAD_SCENE = "Load Scene";
            public const string PREWARM_ASSETS = "Prewarm Assets";
        }

        #endregion
    }
}