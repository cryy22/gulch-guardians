using GulchGuardians.System;
using UnityEngine;

namespace GulchGuardians.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AttackSounds;
        [SerializeField] private AudioClip[] DefeatSounds;
        [SerializeField] private UserSettings Settings;

        private AudioSource _audioSource;

        public static SoundFXPlayer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayAttackSound()
        {
            if (Settings.SoundFXOn) PlayRandom(AttackSounds);
        }

        public void PlayDefeatSound()
        {
            if (Settings.SoundFXOn) PlayRandom(DefeatSounds);
        }

        private void PlayRandom(AudioClip[] clips)
        {
            _audioSource.PlayOneShot(clips[Random.Range(minInclusive: 0, maxExclusive: clips.Length)]);
        }
    }
}
