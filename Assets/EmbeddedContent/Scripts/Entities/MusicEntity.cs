using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBreakout
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicEntity : MonoBehaviour
    {
        [SerializeField]
        string _track_key;

        [SerializeField]
        AudioSource _audiosource;

        void Reset()
        {
            _audiosource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            ResourcesManager.Instance.LoadSound(_track_key, (AudioClip loadedClip) =>
            {
                _audiosource.clip = loadedClip;
                _audiosource.Play();
            });
        }
    }
}
