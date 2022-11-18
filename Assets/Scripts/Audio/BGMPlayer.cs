using System;
using System.Collections;
using GulchGuardians.System;
using UnityEngine;

namespace GulchGuardians.Audio
{
    public class BGMPlayer : MonoBehaviour
    {
        public float Volume = 0.5f;

        public float CrossfadeDuration = 1f;

        [SerializeField] private AudioSource PreparationAudioSource;
        [SerializeField] private AudioSource CombatAudioSource;
        [SerializeField] private UserSettings Settings;
        private State _state = State.Preparation;

        private bool _isCrossfading;

        public static BGMPlayer Instance { get; private set; }

        private float TargetPreparationVolume => Settings.MusicOn && _state == State.Preparation ? Volume : 0f;
        private float TargetCombatVolume => Settings.MusicOn && _state == State.Combat ? Volume : 0f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            PreparationAudioSource.volume = TargetPreparationVolume;
            CombatAudioSource.volume = TargetCombatVolume;
        }

        private void OnEnable() { Settings.Changed += ChangedEventHandler; }
        private void OnDisable() { Settings.Changed -= ChangedEventHandler; }

        public void TransitionToPreparation()
        {
            _state = State.Preparation;
            StartCoroutine(CrossfadeVolumes());
        }

        public void TransitionToCombat()
        {
            _state = State.Combat;
            StartCoroutine(CrossfadeVolumes());
        }

        private void ChangedEventHandler(object sender, EventArgs e)
        {
            PreparationAudioSource.volume = TargetPreparationVolume;
            CombatAudioSource.volume = TargetCombatVolume;
        }

        private IEnumerator CrossfadeVolumes()
        {
            yield return new WaitUntil(() => !_isCrossfading);
            _isCrossfading = true;

            float t = 0;

            float preparationInitialVolume = PreparationAudioSource.volume;
            float combatInitialVolume = CombatAudioSource.volume;

            while (t < CrossfadeDuration)
            {
                t += Time.deltaTime / CrossfadeDuration;
                PreparationAudioSource.volume = Mathf.SmoothStep(
                    from: preparationInitialVolume,
                    to: TargetPreparationVolume,
                    t: t
                );
                CombatAudioSource.volume = Mathf.SmoothStep(from: combatInitialVolume, to: TargetCombatVolume, t: t);
                yield return null;
            }

            _isCrossfading = false;
        }

        private enum State
        {
            Preparation,
            Combat,
        }
    }
}
