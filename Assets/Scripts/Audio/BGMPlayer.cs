using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    public class BGMPlayer : MonoBehaviour
    {
        public float Volume = 0.5f;

        public float CrossfadeDuration = 1f;

        [SerializeField] private AudioSource PreparationAudioSource;
        [SerializeField] private AudioSource CombatAudioSource;

        public static BGMPlayer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CombatAudioSource.volume = 0;
        }

        public void TransitionToCombat()
        {
            StartCoroutine(Crossfade(from: PreparationAudioSource, to: CombatAudioSource));
        }

        public void TransitionToPreparation()
        {
            StartCoroutine(Crossfade(from: CombatAudioSource, to: PreparationAudioSource));
        }

        private IEnumerator Crossfade(AudioSource from, AudioSource to)
        {
            float t = 0;
            while (t < CrossfadeDuration)
            {
                t += Time.deltaTime / CrossfadeDuration;
                from.volume = Mathf.SmoothStep(from: Volume, to: 0, t: t);
                to.volume = Mathf.SmoothStep(from: 0, to: Volume, t: t);
                yield return null;
            }
        }
    }
}
