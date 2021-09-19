using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSystem : MonoBehaviour {
    private static AudioSystem instance;
    public static AudioSystem Instance {
        get { return instance; }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private AudioSource sfxSource;

    private void Start() {
        sfxSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip clip) {
        sfxSource.clip = clip;
        sfxSource.Play();
    }
}
