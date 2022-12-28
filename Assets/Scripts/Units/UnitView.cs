using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.UI;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using GulchGuardians.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians.Units
{
    public class UnitView : MonoBehaviour, IArrangementElement
    {
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private SpriteRenderer Renderer;
        [SerializeField] private SpriteLibrary SpriteLibrary;
        [SerializeField] private Animator Animator;
        [SerializeField] private Transform AbilityIcons;
        [SerializeField] private UIAbilityIconItem AbilityIconPrefab;
        [SerializeField] private AbilityIndex AbilityIndex;
        [SerializeField] private ParticleSystem AttackParticleSystem;
        [SerializeField] private List<GameObject> UIElements;

        private readonly Dictionary<AbilityType, UIAbilityIconItem> _abilityIcons = new();
        private Quaternion _leftParticleRotation;
        private Quaternion _rightParticleRotation;

        private UnitSpriteAssetMap _spriteAssetMap;
        private bool _hasHealerSpriteAsset;

        private void Awake()
        {
            _leftParticleRotation = AttackParticleSystem.transform.rotation;
            _rightParticleRotation = Quaternion.Euler(x: 0, y: 0, z: 180) * _leftParticleRotation;
        }

        public void Setup(UnitSpriteAssetMap spriteAssetMap, Unit unit)
        {
            _spriteAssetMap = spriteAssetMap;

            SpriteLibrary.spriteLibraryAsset = _spriteAssetMap.Default;
            _hasHealerSpriteAsset = _spriteAssetMap.Healer != null;
            Animator.Play(stateNameHash: 0, layer: 0, normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f));

            NameText.text = unit.FirstName;
            if (unit.HasAbility(AbilityIndex.Boss))
            {
                Renderer.transform.localScale *= 2f;
                unit.SetNametagActive(false);
            }

            UpdateAttributes(unit);
        }

        public void UpdateAttributes(Unit unit)
        {
            AttackText.text = unit.Attack.ToString();
            HealthText.text = unit.Health.ToString();
            HealthText.color = unit.Health == unit.MaxHealth ? Color.white : Color.red;

            UpdateAbilities(unit);
            UpdateSpriteLibraryAsset(unit);
        }

        public IEnumerator AnimateAttack(Unit target)
        {
            if (target == null) yield break;

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
            Renderer.color = Color.red;

            SoundFXPlayer.Instance.PlayDefeatSound();
            Animator.SetTrigger(AnimatorProperties.OnHurtTrigger);

            const float spinDuration = 0.33f;
            const float spinPeriod = 0.33f;
            const float shrinkDuration = 0.2f;

            Vector3 initialScale = transform.localScale;

            var time = 0f;
            while (time < spinDuration)
            {
                time += Time.deltaTime;
                transform.localScale = new Vector3(
                    x: Mathf.Cos((time / spinPeriod) * Mathf.PI * 2) * initialScale.x,
                    y: initialScale.y,
                    z: initialScale.z
                );

                yield return null;
            }

            time = 0f;
            while (time < shrinkDuration)
            {
                time += Time.deltaTime;

                transform.localScale = Vector3.Slerp(a: initialScale, b: Vector3.zero, t: time / shrinkDuration);
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

        public void SetAnimationTrigger(string animationName) { Animator.SetTrigger(animationName); }

        public void ShowUI(bool show)
        {
            foreach (GameObject go in UIElements) go.SetActive(show);
        }

        private void UpdateSpriteLibraryAsset(Unit unit)
        {
            if (_hasHealerSpriteAsset && unit.HasAbility(AbilityIndex.Healer))
                SpriteLibrary.spriteLibraryAsset = _spriteAssetMap.Healer;
            else
                SpriteLibrary.spriteLibraryAsset = _spriteAssetMap.Default;
        }

        private void UpdateAbilities(Unit unit)
        {
            IEnumerable<AbilityType> activeAbilities = unit.Abilities.ToList();

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

        // IArrangementElement
        public Transform Transform => transform;
        public Vector2 SizeMultiplier => Vector2.one;
        public Vector2 Pivot => Vector2.one * 0.5f;

        public enum DamageDirection
        {
            Left,
            Right,
        }
    }
}
