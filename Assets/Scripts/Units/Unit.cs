using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common;
using Crysc.Helpers;
using Crysc.Initialization;
using Crysc.UI;
using GulchGuardians.Abilities;
using GulchGuardians.Common;
using GulchGuardians.Constants;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using UnityEngine;

namespace GulchGuardians.Units
{
    using Direction = UIUnit.DamageDirection;

    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UIUnit))]
    public class Unit : InitializationBehaviour<UnitInitParams>, IArrangementElement
    {
        [SerializeField] private AbilityIndex AbilityIndex;
        [SerializeField] private GameObject Nametag;

        private readonly HashSet<AbilityType> _abilities = new();

        private ClickReporter _clickReporter;
        private int _attack;
        private BoundsCalculator _boundsCalculator;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public event EventHandler Changed;

        public IEnumerable<AbilityType> Abilities => _abilities;
        public string FirstName => InitParams.FirstName;
        public Team Team => Squad != null ? Squad.Team : null;
        public UIUnit UI { get; private set; }
        public Squad Squad { get; set; }

        public int Attack
        {
            get => _attack;
            private set => _attack = Mathf.Max(a: 0, b: value);
        }

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool TooltipEnabled { get; set; } = true;

        private int ActionMagnitude
        {
            get
            {
                var divisor = 1;
                if (HasAbility(AbilityIndex.Archer)) divisor *= 2;
                if (HasAbility(AbilityIndex.Healer)) divisor *= 2;

                return Attack / divisor;
            }
        }

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            UI = GetComponent<UIUnit>();
            _boundsCalculator = new BoundsCalculator(transform);
        }

        public static bool IsDefeated(Unit unit) { return unit == null || unit.Health <= 0; }

        public override void Initialize(UnitInitParams initParams)
        {
            base.Initialize(initParams);

            Attack = initParams.Attack;
            Health = initParams.Health;
            MaxHealth = Mathf.Max(a: Health, b: initParams.MaxHealth);

            _abilities.UnionWith(initParams.Abilities.Where(p => p.Value).Select(p => p.Key));
            UI.Setup(spriteAssetMap: initParams.SpriteAssetMap, unit: this);
        }

        public void SetNametagActive(bool active) { Nametag.SetActive(value: active); }

        public IEnumerator AddAbility(AbilityType ability)
        {
            _abilities.Add(ability);

            UI.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return UI.AnimateStatsChange(animateAbilities: true);
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            UI.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return UI.AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator Heal(int amount = int.MaxValue)
        {
            Health += Mathf.Min(a: amount, b: MaxHealth - Health);

            UI.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return UI.AnimateStatsChange(animateHealth: true);
        }

        public bool WillAct(int index)
        {
            return index switch
            {
                0 => true,
                1 => HasAbility(AbilityIndex.Archer),
                _ => false,
            };
        }

        public IEnumerator AttackUnit(Unit target)
        {
            yield return UI.AnimateAttack(target);

            int damage = ActionMagnitude;
            Direction direction = transform.position.x < target.transform.position.x ? Direction.Left : Direction.Right;
            yield return target.TakeDamage(damage: damage, direction: direction);

            if (!target.HasAbility(AbilityIndex.Spiky)) yield break;
            Direction spikeDirection = direction == Direction.Left ? Direction.Right : Direction.Left;
            yield return TakeDamage(damage: damage, direction: spikeDirection);
        }

        public IEnumerator HealSquad()
        {
            if (!HasAbility(AbilityIndex.Healer)) yield break;

            yield return UI.AnimateHeal();
            yield return CoroutineWaiter.RunConcurrently(
                behaviours: Squad.Units!,
                u => u.Heal(amount: ActionMagnitude)
            );
        }

        public bool HasAbility(AbilityType ability) { return _abilities.Contains(ability); }

        public void SetHurtAnimation() { UI.SetAnimationTrigger(AnimatorProperties.OnHurtTrigger); }
        public void SetIdleAnimation() { UI.SetAnimationTrigger(AnimatorProperties.OnIdleTrigger); }

        private IEnumerator TakeDamage(int damage, Direction direction)
        {
            damage = HasAbility(AbilityIndex.Tough) ? damage / 2 : damage;
            var abilitiesChanged = false;
            if (HasAbility(AbilityIndex.Sturdy) && damage >= Health)
            {
                damage = Health - 1;
                _abilities.Remove(AbilityIndex.Sturdy);
                abilitiesChanged = true;
            }

            Health -= damage;

            UI.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return UI.AnimateDamage(damage: damage, direction: direction);
            yield return UI.AnimateStatsChange(animateHealth: true, animateAbilities: abilitiesChanged);

            if (IsDefeated(this)) yield return HandleDefeat();
        }

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();
            yield return UI.AnimateDefeat();

            Destroy(gameObject);
        }

        // IArrangementElement
        public Transform Transform => transform;
    }
}
