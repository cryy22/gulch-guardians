using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Initialization;
using GulchGuardians.Abilities;
using GulchGuardians.Classes;
using GulchGuardians.Common;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using UnityEngine;

namespace GulchGuardians.Units
{
    using Direction = UnitView.DamageDirection;

    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UnitView))]
    public class Unit : InitializationBehaviour<UnitInitParams>
    {
        private readonly HashSet<AbilityType> _abilities = new();

        private ClickReporter _clickReporter;
        private int _attack;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public event EventHandler Changed;

        public IEnumerable<AbilityType> Abilities => _abilities;
        public string FirstName => InitParams.FirstName;
        public Team Team => Squad != null ? Squad.Team : null;
        public UnitView View { get; private set; }
        public Squad Squad { get; set; }

        public int Attack
        {
            get => _attack;
            private set => _attack = Mathf.Max(a: 0, b: value);
        }

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public ClassType Class { get; private set; }

        private int ActionMagnitude
        {
            get
            {
                var divisor = 1;
                if (HasAbility(AbilityIndex.I.Archer)) divisor *= 2;
                if (Class == ClassIndex.I.Healer) divisor *= 2;

                return Attack / divisor;
            }
        }

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            View = GetComponent<UnitView>();
        }

        public static bool IsDefeated(Unit unit) { return unit == null || unit.Health <= 0; }

        public override void Initialize(UnitInitParams initParams)
        {
            base.Initialize(initParams);

            Attack = initParams.Attack;
            Health = initParams.Health;
            MaxHealth = Mathf.Max(a: Health, b: initParams.MaxHealth);

            _abilities.UnionWith(initParams.Abilities.Where(p => p.Value).Select(p => p.Key));
            Class = initParams.Class;

            View.Setup(spriteAssetMap: initParams.SpriteAssetMap, unit: this);
        }

        public IEnumerator AddAbility(AbilityType ability)
        {
            _abilities.Add(ability);

            View.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return View.AnimateStatsChange(animateAbilities: true);
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            View.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return View.AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator Heal(int amount = int.MaxValue)
        {
            Health += Mathf.Min(a: amount, b: MaxHealth - Health);

            View.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return View.AnimateStatsChange(animateHealth: true);
        }

        public void SetClass(ClassType @class)
        {
            Class = @class;
            View.SpriteView.UpdateSprite(Class);
        }

        public bool WillAct(int index)
        {
            return index switch
            {
                0 => true,
                1 => HasAbility(AbilityIndex.I.Archer),
                _ => false,
            };
        }

        public IEnumerator AttackUnit(Unit target)
        {
            if (Class == ClassIndex.I.Saguaro) yield break;

            yield return View.AnimateAttack(target);

            int damage = ActionMagnitude;
            Direction direction = transform.position.x < target.transform.position.x ? Direction.Left : Direction.Right;
            yield return target.TakeDamage(damage: damage, direction: direction);

            if (target.Class != ClassIndex.I.Saguaro) yield break;
            Direction spikeDirection = direction == Direction.Left ? Direction.Right : Direction.Left;
            yield return TakeDamage(damage: damage, direction: spikeDirection);
        }

        public IEnumerator HealSquad()
        {
            if (Class != ClassIndex.I.Healer) yield break;

            yield return View.AnimateHeal();
            yield return CoroutineWaiter.RunConcurrently(
                behaviours: Squad.Units!,
                u => u.Heal(amount: ActionMagnitude)
            );
        }

        public bool HasAbility(AbilityType ability) { return _abilities.Contains(ability); }

        public void SetHurtAnimation() { View.SpriteView.SetHurtAnimation(); }
        public void SetIdleAnimation() { View.SpriteView.SetIdleAnimation(); }

        private IEnumerator TakeDamage(int damage, Direction direction)
        {
            damage = HasAbility(AbilityIndex.I.Tough) ? damage / 2 : damage;
            var abilitiesChanged = false;
            if (HasAbility(AbilityIndex.I.Sturdy) && damage >= Health)
            {
                damage = Health - 1;
                _abilities.Remove(AbilityIndex.I.Sturdy);
                abilitiesChanged = true;
            }

            Health -= damage;

            View.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return View.AnimateDamage(damage: damage, direction: direction);
            yield return View.AnimateStatsChange(animateHealth: true, animateAbilities: abilitiesChanged);

            if (IsDefeated(this)) yield return HandleDefeat();
        }

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();
            yield return View.AnimateDefeat();

            Destroy(gameObject);
        }
    }
}
