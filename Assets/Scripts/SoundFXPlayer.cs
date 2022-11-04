using UnityEngine;

namespace GulchGuardians
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AttackSounds;

        private AudioSource _audioSource;

        private void Awake() { _audioSource = GetComponent<AudioSource>(); }

        public void PlayAttackSound()
        {
            _audioSource.PlayOneShot(AttackSounds[Random.Range(minInclusive: 0, maxExclusive: AttackSounds.Length)]);
        }
    }
}
