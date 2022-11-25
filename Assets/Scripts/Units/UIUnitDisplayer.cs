using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using GulchGuardians.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians.Units
{
    public class UIUnitDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private SpriteRenderer Renderer;
        [SerializeField] private SpriteLibrary SpriteLibrary;
        [SerializeField] private Animator Animator;
        [SerializeField] private Transform AbilityIcons;
        [SerializeField] private UIAbilityIconItem AbilityIconPrefab;
        [SerializeField] private AbilityType BossType;
        [SerializeField] private AbilityType HealerType;
        [SerializeField] private ParticleSystem AttackParticleSystem;

        private readonly Dictionary<AbilityType, UIAbilityIconItem> _abilityIcons = new();
        private Quaternion _leftParticleRotation;
        private Quaternion _rightParticleRotation;
        private UnitSpriteAssetMap _spriteAssetMap;

        private bool _isHealer;

        private void Awake()
        {
            _leftParticleRotation = AttackParticleSystem.transform.rotation;
            _rightParticleRotation = Quaternion.Euler(x: 0, y: 0, z: 180) * _leftParticleRotation;
        }

        public void Setup(UnitSpriteAssetMap spriteAssetMap, UnitInitParams initParams)
        {
            _spriteAssetMap = spriteAssetMap;
            SpriteLibrary.spriteLibraryAsset = _spriteAssetMap.Default;
            Animator.Play(stateNameHash: 0, layer: 0, normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f));

            NameText.text = initParams.FirstName;
            UpdateAttributes(initParams);

            if (initParams.HasAbility(BossType)) Renderer.transform.localScale *= 1.33f;
        }

        public void UpdateAttributes(UnitInitParams unitInitParams)
        {
            AttackText.text = unitInitParams.Attack.ToString();
            HealthText.text = unitInitParams.Health.ToString();
            HealthText.color = unitInitParams.Health == unitInitParams.MaxHealth ? Color.white : Color.red;

            UpdateAbilities(unitInitParams);
            UpdateHealerStatus(unitInitParams);
        }

        public IEnumerator AnimateToPosition(Vector3 position, float duration = 0.25f)
        {
            Vector3 startPosition = transform.position;
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(a: startPosition, b: position, t: t);
                yield return null;
            }

            transform.position = position;
        }

        public IEnumerator AnimateAttack(Unit target)
        {
            const float duration = 0.0833f;
            Animator.SetTrigger(AnimatorProperties.OnActTrigger);

            Vector3 startPosition = transform.position;
            Vector3 endPosition = target.transform.position;
            var t = 0f;

            while (t <= 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(a: startPosition, b: endPosition, t: t);
                yield return null;
            }

            SoundFXPlayer.Instance.PlayAttackSound();
            transform.position = startPosition;
            Animator.SetTrigger(AnimatorProperties.OnIdleTrigger);
        }

        public IEnumerator AnimateHeal()
        {
            Animator.SetTrigger(AnimatorProperties.OnActTrigger);
            yield return AnimateSine(duration: 0.5f, period: 0.5f, magnitude: 0.25f);
            Animator.SetTrigger(AnimatorProperties.OnIdleTrigger);
        }

        public IEnumerator AnimateDamage(int damage, DamageDirection direction)
        {
            AttackParticleSystem.transform.rotation =
                direction == DamageDirection.Left ? _leftParticleRotation : _rightParticleRotation;
            AttackParticleSystem.Play();

            yield return CoroutineWaiter.RunConcurrently(
                StartCoroutine(AnimateSine()),
                StartCoroutine(AnimateHurtAnimation()),
                StartCoroutine(AnimateFlash(damage > 0 ? Color.red : Color.gray))
            );
        }

        public IEnumerator AnimateDefeat()
        {
            AttackText.color = Color.red;
            HealthText.color = Color.red;

            SoundFXPlayer.Instance.PlayDefeatSound();
            var duration = 0.2f;
            var time = 0f;
            Vector3 initialScale = transform.localScale;

            while (time < duration)
            {
                transform.localScale = Vector3.Slerp(a: initialScale, b: Vector3.zero, t: time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator AnimateStatsChange(
            bool animateAttack = false,
            bool animateHealth = false,
            bool animateAbilities = false
        )
        {
            if ((animateAttack || animateHealth || animateAbilities) == false) yield break;

            Vector3 attackCurrentScale = AttackText.transform.localScale;
            Vector3 healthCurrentScale = HealthText.transform.localScale;
            Vector3 abilityIconsCurrentScale = AbilityIcons.localScale;

            if (animateAttack) AttackText.transform.localScale = attackCurrentScale * 1.5f;
            if (animateHealth) HealthText.transform.localScale = healthCurrentScale * 1.5f;
            if (animateAbilities) AbilityIcons.localScale = abilityIconsCurrentScale * 1.5f;

            yield return new WaitForSeconds(0.5f);

            AttackText.transform.localScale = attackCurrentScale;
            HealthText.transform.localScale = healthCurrentScale;
            AbilityIcons.localScale = abilityIconsCurrentScale;
        }

        private void UpdateHealerStatus(UnitInitParams initParams)
        {
            _isHealer = initParams.HasAbility(HealerType);
            SpriteLibrary.spriteLibraryAsset = _isHealer ? _spriteAssetMap.Healer : _spriteAssetMap.Default;
        }

        private IEnumerator AnimateSine(float duration = 0.08f, float period = 0.04f, float magnitude = 0.25f)
        {
            Vector3 startPosition = transform.position;
            var t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                transform.position = startPosition + (Vector3.up * (magnitude * Mathf.Sin((t / period) * Mathf.PI)));
                yield return null;
            }

            transform.position = startPosition;
        }

        private IEnumerator AnimateFlash(Color flashColor, float duration = 0.125f)
        {
            Color startColor = Renderer.color;
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                Renderer.color = Color.Lerp(a: flashColor, b: startColor, t: t);
                yield return null;
            }

            Renderer.color = startColor;
        }

        private IEnumerator AnimateHurtAnimation(float duration = 0.33f)
        {
            Animator.SetTrigger(AnimatorProperties.OnHurtTrigger);

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

            Animator.SetTrigger(AnimatorProperties.OnIdleTrigger);
        }

        private void UpdateAbilities(UnitInitParams initParams)
        {
            IEnumerable<AbilityType> activeAbilities = initParams.ActiveAbilities.ToList();

            foreach (AbilityType ability in activeAbilities)
            {
                if (_abilityIcons.ContainsKey(ability)) continue;

                UIAbilityIconItem iconItem = Instantiate(original: AbilityIconPrefab, parent: AbilityIcons);
                iconItem.SetAbility(ability);

                _abilityIcons.Add(key: ability, value: iconItem);
            }

            foreach (AbilityType ability in _abilityIcons.Keys.Except(activeAbilities).ToList())
            {
                Destroy(_abilityIcons[ability].gameObject);
                _abilityIcons.Remove(key: ability);
            }
        }

        public enum DamageDirection
        {
            Left,
            Right,
        }
    }
}
