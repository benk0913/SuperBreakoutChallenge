using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInstance : MonoBehaviour
{
    [SerializeField]
    AudioSource Source;

    Action OnComplete;

    public void SetInfo(AudioClip clip, Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        this.OnComplete = onComplete;
        Source.clip = clip;
        Source.Play();
        
    }

    void LateUpdate()
    {
        if(!Source.isPlaying)
        {
            this.OnComplete?.Invoke();
            this.gameObject.SetActive(false);
        }
    }
}
