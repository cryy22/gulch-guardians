using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UnitDisplayer))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private AbilityType SturdyType;
        [SerializeField] private AbilityType ArcherType;

        private readonly Dictionary<AbilityType, bool> _abilities = new();

        private ClickReporter _clickReporter;
        private UnitDisplayer _displayer;

        private bool _isInitialized;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public bool IsDefeated => Health <= 0;

        public string FirstName { get; private set; }
        public int Attack { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public bool TooltipEnabled { get; set; } = true;

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            _displayer = GetComponent<UnitDisplayer>();
        }

        public IEnumerator AddAbility(AbilityType ability)
        {
            _abilities[ability] = true;
            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateAbilities: true);
        }

        public void Initialize(
            SpriteLibraryAsset spriteLibraryAsset,
            Attributes attributes
        )
        {
            if (_isInitialized) throw new Exception("Unit is already initialized");

            FirstName = attributes.FirstName;
            Attack = attributes.Attack;
            Health = attributes.Health;
            MaxHealth = Mathf.Max(a: Health, b: attributes.MaxHealth);

            foreach (KeyValuePair<AbilityType, bool> pair in attributes.Abilities) _abilities[pair.Key] = pair.Value;

            _displayer.Setup(spriteLibraryAsset: spriteLibraryAsset, attributes: BuildAttributes());

            _isInitialized = true;
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator FullHeal()
        {
            Health = MaxHealth;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateHealth: true);
        }

        public IEnumerator MoveToPosition(Vector3 position, float duration)
        {
            yield return _displayer.AnimateToPosition(position: position, duration: duration);
        }

        public bool WillAttack(int index) { return index == (HasAbility(ArcherType) ? 1 : 0); }

        public IEnumerator AttackUnit(Unit target)
        {
            yield return _displayer.AnimateAttack(target);
            yield return target.TakeDamage(Attack);
        }

        public bool HasAbility(AbilityType ability) { return _abilities.GetValueOrDefault(ability); }

        private IEnumerator TakeDamage(int damage)
        {
            var abilitiesChanged = false;
            if (HasAbility(SturdyType) && damage >= Health)
            {
                damage = Health - 1;
                _abilities.Remove(SturdyType);
                abilitiesChanged = true;
            }

            Health -= damage;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateDamage(damage);
            yield return _displayer.AnimateStatsChange(animateHealth: true, animateAbilities: abilitiesChanged);

            if (IsDefeated) StartCoroutine(HandleDefeat());
        }

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();
            yield return _displayer.AnimateDefeat();

            Destroy(gameObject);
        }

        private Attributes BuildAttributes()
        {
            return new Attributes
            {
                FirstName = FirstName,
                Attack = Attack,
                Health = Health,
                MaxHealth = MaxHealth,
                Abilities = _abilities,
            };
        }

        [Serializable]
        public struct Attributes
        {
            public string FirstName;
            public int Attack;
            public int Health;
            public int MaxHealth;
            public IReadOnlyDictionary<AbilityType, bool> Abilities;

            public IEnumerable<AbilityType> ActiveAbilities =>
                Abilities.Where(pair => pair.Value).Select(pair => pair.Key);

            public bool HasAbility(AbilityType ability) { return Abilities.GetValueOrDefault(ability); }
        }
    }
}
