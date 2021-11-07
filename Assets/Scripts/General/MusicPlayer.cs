using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GameJam
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;
        
        [SerializeField]
        private AudioSource audioSource;

        private void Start()
        {
            this.audioSource.Play();
        }
    }
}
