using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Initialization;
using GulchGuardians.Abilities;
using GulchGuardians.Common;
using GulchGuardians.Teams;
using UnityEngine;

namespace GulchGuardians.Units
{
    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UIUnitDisplayer))]
    [RequireComponent(typeof(UnitRegistrar))]
    [RequireComponent(typeof(Collider2D))]
    public class Unit : InitializationBehaviour<UnitInitParams>
    {
        [SerializeField] private AbilityType SturdyType;
        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType HealerType;

        private readonly Dictionary<AbilityType, bool> _abilities = new();

        private ClickReporter _clickReporter;
        private UIUnitDisplayer _displayer;
        private Collider2D _collider;

        private int _attack;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public event EventHandler Destroying;

        public bool IsDefeated => Health <= 0;

        public IEnumerable<AbilityType> ActiveAbilities =>
            _abilities.Where(pair => pair.Value).Select(pair => pair.Key);

        public string FirstName => InitParams.FirstName;
        public Bounds Bounds => _collider.bounds;

        public Team Team { get; set; }

        public int Attack
        {
            get => _attack;
            private set => _attack = Mathf.Max(a: 0, b: value);
        }

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public bool TooltipEnabled { get; set; } = true;

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            _displayer = GetComponent<UIUnitDisplayer>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnDestroy() { Destroying?.Invoke(sender: this, e: EventArgs.Empty); }

        public IEnumerator AddAbility(AbilityType ability)
        {
            _abilities[ability] = true;
            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateAbilities: true);
        }

        public override void Initialize(UnitInitParams initParams)
        {
            base.Initialize(initParams);

            Attack = initParams.Attack;
            Health = initParams.Health;
            MaxHealth = Mathf.Max(a: Health, b: initParams.MaxHealth);

            foreach (KeyValuePair<AbilityType, bool> pair in initParams.Abilities) _abilities[pair.Key] = pair.Value;

            _displayer.Setup(spriteAssetMap: initParams.SpriteAssetMap, initParams: BuildAttributes());
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator Heal(int amount = int.MaxValue)
        {
            Health += Mathf.Min(a: amount, b: MaxHealth - Health);

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateHealth: true);
        }

        public IEnumerator MoveToPosition(Vector3 position, float duration)
        {
            yield return _displayer.AnimateToPosition(position: position, duration: duration);
        }

        public bool WillAttack(int index) { return index == (HasAbility(ArcherType) ? 1 : 0); }

        public IEnumerator AttackUnit(Unit target, Team unitTeam)
        {
            if (HasAbility(HealerType))
            {
                yield return _displayer.AnimateHeal();
                yield return CoroutineWaiter.RunConcurrently(
                    behaviours: unitTeam.Units!,
                    u => u.Heal(amount: Attack / 2)
                );
                yield break;
            }

            yield return _displayer.AnimateAttack(target);

            UIUnitDisplayer.DamageDirection direction = transform.position.x < target.transform.position.x
                ? UIUnitDisplayer.DamageDirection.Left
                : UIUnitDisplayer.DamageDirection.Right;
            yield return target.TakeDamage(damage: Attack, direction: direction);
        }

        public bool HasAbility(AbilityType ability) { return _abilities.GetValueOrDefault(ability); }

        private IEnumerator TakeDamage(int damage, UIUnitDisplayer.DamageDirection direction)
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
            yield return _displayer.AnimateDamage(damage: damage, direction: direction);
            yield return _displayer.AnimateStatsChange(animateHealth: true, animateAbilities: abilitiesChanged);

            if (IsDefeated) StartCoroutine(HandleDefeat());
        }

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();
            yield return _displayer.AnimateDefeat();

            Destroy(gameObject);
        }

        private UnitInitParams BuildAttributes()
        {
            return new UnitInitParams
            {
                FirstName = FirstName,
                Attack = Attack,
                Health = Health,
                MaxHealth = MaxHealth,
                Abilities = _abilities,
            };
        }
    }
}
