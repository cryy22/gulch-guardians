using UnityEngine;

namespace GulchGuardians
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AttackSounds;
        [SerializeField] private AudioClip[] DefeatSounds;

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

        public void PlayAttackSound() { PlayRandom(AttackSounds); }

        public void PlayDefeatSound() { PlayRandom(DefeatSounds); }

        private void PlayRandom(AudioClip[] clips)
        {
            _audioSource.PlayOneShot(clips[Random.Range(minInclusive: 0, maxExclusive: clips.Length)]);
        }
    }
}
