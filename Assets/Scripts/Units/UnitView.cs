using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Presentation;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Units
{
    public class UnitView : MonoBehaviour, IArrangementElement
    {
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private GameObject Nametag;
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private Transform AbilityIcons;
        [SerializeField] private AbilityIconItem AbilityIconPrefab;
        [SerializeField] private AbilityIndex AbilityIndex;
        [SerializeField] private ParticleSystem AttackParticleSystem;
        [SerializeField] private List<GameObject> DetailElements;

        private readonly Dictionary<AbilityType, AbilityIconItem> _abilityIcons = new();
        private Quaternion _leftParticleRotation;
        private Quaternion _rightParticleRotation;
        private bool _shouldShowNametag = true;

        [field: SerializeField] public UnitSpriteView SpriteView { get; private set; }

        public UnitDraggable Draggable { get; private set; }
        public bool ShowTooltip { get; private set; } = true;

        private void Awake()
        {
            _leftParticleRotation = AttackParticleSystem.transform.rotation;
            _rightParticleRotation = Quaternion.Euler(x: 0, y: 0, z: 180) * _leftParticleRotation;

            Draggable = GetComponent<UnitDraggable>();
        }

        public void Setup(UnitSpriteAssetMap spriteAssetMap, Unit unit)
        {
            SpriteView.Setup(assetMap: spriteAssetMap, unit: unit);

            NameText.text = unit.FirstName;
            if (unit.HasAbility(AbilityIndex.Boss))
            {
                _shouldShowNametag = false;
                Nametag.SetActive(false);
            }

            UpdateAttributes(unit);
        }

        public void UpdateAttributes(Unit unit)
        {
            AttackText.text = unit.Attack.ToString();
            HealthText.text = unit.Health.ToString();
            HealthText.color = unit.Health == unit.MaxHealth ? Color.white : Color.red;

            UpdateAbilities(unit);
            SpriteView.UpdateSprite(unit);
        }

        public IEnumerator AnimateAttack(Unit target)
        {
            if (target == null) yield break;

            SpriteView.SetActAnimation();

            Vector3 start = transform.position;
            Vector3 end = target.transform.position;
            yield return Mover.MoveTo(transform: transform, end: end, duration: 0.0833f, isLocal: false);
            SoundFXPlayer.Instance.PlayAttackSound();

            transform.position = start;
            SpriteView.SetIdleAnimation();
        }

        public IEnumerator AnimateHeal()
        {
            Vector3 start = transform.localPosition;

            SpriteView.SetActAnimation();
            yield return AnimateSine(duration: 0.5f, period: 0.5f, magnitude: 0.25f);
            SpriteView.SetIdleAnimation();

            transform.localPosition = start;
        }

        public IEnumerator AnimateDamage(int damage, DamageDirection direction)
        {
            Vector3 start = transform.localPosition;

            AttackParticleSystem.transform.rotation =
                direction == DamageDirection.Left ? _leftParticleRotation : _rightParticleRotation;
            AttackParticleSystem.Play();

            yield return CoroutineWaiter.RunConcurrently(
                StartCoroutine(AnimateSine(duration: 0.08f, period: 0.08f, magnitude: 0.25f)),
                StartCoroutine(AnimateHurtAnimation()),
                StartCoroutine(SpriteView.AnimateFlash(damage > 0 ? Color.red : Color.gray))
            );

            transform.localPosition = start;
        }

        public IEnumerator AnimateDefeat()
        {
            AttackText.color = Color.red;
            HealthText.color = Color.red;

            return SpriteView.AnimateDefeat();
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

        public void SetShowDetails(bool show)
        {
            ShowTooltip = show;
            Nametag.SetActive(_shouldShowNametag && show);
            foreach (GameObject go in DetailElements) go.SetActive(show);
        }

        private void UpdateAbilities(Unit unit)
        {
            IEnumerable<AbilityType> activeAbilities = unit.Abilities.ToList();

            foreach (AbilityType ability in activeAbilities)
            {
                if (_abilityIcons.ContainsKey(ability)) continue;

                AbilityIconItem iconItem = Instantiate(original: AbilityIconPrefab, parent: AbilityIcons);
                iconItem.SetAbility(ability);

                _abilityIcons.Add(key: ability, value: iconItem);
            }

            foreach (AbilityType ability in _abilityIcons.Keys.Except(activeAbilities).ToList())
            {
                Destroy(_abilityIcons[ability].gameObject);
                _abilityIcons.Remove(key: ability);
            }
        }

        private IEnumerator AnimateSine(float duration = 0.08f, float period = 0.08f, float magnitude = 0.25f)
        {
            Vector3 delta = magnitude * Vector3.up;
            return Mover.MoveSine(transform: transform, delta: delta, duration: duration, period: period);
        }

        private IEnumerator AnimateHurtAnimation(float duration = 0.33f)
        {
            SpriteView.SetHurtAnimation();
            yield return new WaitForSeconds(duration);
            SpriteView.SetIdleAnimation();
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
