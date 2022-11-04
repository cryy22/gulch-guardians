using UnityEngine;

namespace GulchGuardians
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AttackSounds;

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
            _audioSource.PlayOneShot(AttackSounds[Random.Range(minInclusive: 0, maxExclusive: AttackSounds.Length)]);
        }
    }
}
