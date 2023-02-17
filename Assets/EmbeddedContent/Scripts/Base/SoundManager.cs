using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    public class SoundManager : MonoBehaviour
    {

        #region  Parameters
        public static SoundManager Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("Sound Manager not yet initialized!");

                    return null;
                }

                return _instance;
            }
        }
        static SoundManager _instance;
        static bool _isInitialized;

        [SerializeField]
        GameObject AudioInstancePrefab;

        #endregion

        #region Lifecycle
        void Awake()
        {
            _instance = this;
            _isInitialized = true;
        }

        #endregion

        #region Public Interface

        public void PlaySound(string clipKey)
        {
            ResourcesManager.Instance.LoadSound(clipKey, (AudioClip clip) =>
            {
                PlaySound(clip);
            });
        }

        public void PlaySound(AudioClip clip)
        {
            GameObject soundInstance = ObjectPoolManager.GetObjectPool(AudioInstancePrefab);
            soundInstance.GetComponent<SoundInstance>().SetInfo(clip, () => soundInstance.SetActive(false));
        }

        #endregion

    }
}