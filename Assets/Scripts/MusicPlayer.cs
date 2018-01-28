using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    private AudioSource src;
    private AudioClip defaultClip;
    private float targetVolume;

    public float fadeTime = 1.0f;

    public static MusicPlayer inst { get; private set; }

    private void Awake() {
        inst = this;
        src = GetComponent<AudioSource>();
        targetVolume = src.volume;
        defaultClip = src.clip;
    }

    public void ChangeTrack(AudioClip clip) {
        if (clip != src.clip) {
            StartCoroutine(_ChangeTrack(clip));
        }
    }

    public void ChangeToDefault() {
        ChangeTrack(defaultClip);
    }

    private IEnumerator _ChangeTrack(AudioClip clip) {
        while (src.volume > 0) {
            src.volume -= Time.deltaTime * targetVolume / fadeTime;

            yield return null;
        }

        src.Stop();
        src.clip = clip;
        src.Play();

        while (src.volume < targetVolume) {
            src.volume += Time.deltaTime * targetVolume / fadeTime;

            yield return null;
        }
    }
}
