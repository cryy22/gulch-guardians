using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Initialization;
using GulchGuardians.Abilities;
using GulchGuardians.Common;
using GulchGuardians.Constants;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using UnityEngine;

namespace GulchGuardians.Units
{
    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UIUnit))]
    public class Unit : InitializationBehaviour<UnitInitParams>
    {
        [SerializeField] private AbilityType SturdyType;
        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType HealerType;

        [SerializeField] private GameObject Nametag;

        private readonly HashSet<AbilityType> _abilities = new();

        private ClickReporter _clickReporter;
        private UIUnit _ui;
        private int _attack;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public event EventHandler Changed;

        public bool IsDefeated => Health <= 0;

        public IEnumerable<AbilityType> Abilities => _abilities;

        public string FirstName => InitParams.FirstName;
        public Team Team => Squad != null ? Squad.Team : null;
        public Squad Squad { get; set; }

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
            _ui = GetComponent<UIUnit>();
        }

        public override void Initialize(UnitInitParams initParams)
        {
            base.Initialize(initParams);

            Attack = initParams.Attack;
            Health = initParams.Health;
            MaxHealth = Mathf.Max(a: Health, b: initParams.MaxHealth);

            _abilities.UnionWith(initParams.Abilities.Where(p => p.Value).Select(p => p.Key));
            _ui.Setup(spriteAssetMap: initParams.SpriteAssetMap, unit: this);
        }

        public void SetNametagActive(bool active) { Nametag.SetActive(value: active); }

        public IEnumerator AddAbility(AbilityType ability)
        {
            _abilities.Add(ability);

            _ui.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return _ui.AnimateStatsChange(animateAbilities: true);
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            _ui.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return _ui.AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator Heal(int amount = int.MaxValue)
        {
            Health += Mathf.Min(a: amount, b: MaxHealth - Health);

            _ui.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return _ui.AnimateStatsChange(animateHealth: true);
        }

        public IEnumerator MoveToPosition(Vector3 position, float duration)
        {
            yield return _ui.AnimateToPosition(position: position, duration: duration);
        }

        public bool WillAttack(int index) { return index == (HasAbility(ArcherType) ? 1 : 0); }

        public IEnumerator AttackUnit(Unit target)
        {
            if (HasAbility(HealerType))
            {
                yield return _ui.AnimateHeal();
                yield return CoroutineWaiter.RunConcurrently(
                    behaviours: Squad.Units!,
                    u => u.Heal(amount: Attack / 2)
                );
                yield break;
            }

            yield return _ui.AnimateAttack(target);

            UIUnit.DamageDirection direction = transform.position.x < target.transform.position.x
                ? UIUnit.DamageDirection.Left
                : UIUnit.DamageDirection.Right;
            yield return target.TakeDamage(damage: Attack, direction: direction);
        }

        public bool HasAbility(AbilityType ability) { return _abilities.Contains(ability); }

        public void SetHurtAnimation() { _ui.SetAnimationTrigger(AnimatorProperties.OnHurtTrigger); }
        public void SetIdleAnimation() { _ui.SetAnimationTrigger(AnimatorProperties.OnIdleTrigger); }

        private IEnumerator TakeDamage(int damage, UIUnit.DamageDirection direction)
        {
            var abilitiesChanged = false;
            if (HasAbility(SturdyType) && damage >= Health)
            {
                damage = Health - 1;
                _abilities.Remove(SturdyType);
                abilitiesChanged = true;
            }

            Health -= damage;

            _ui.UpdateAttributes(this);
            Changed?.Invoke(sender: this, e: EventArgs.Empty);

            yield return _ui.AnimateDamage(damage: damage, direction: direction);
            yield return _ui.AnimateStatsChange(animateHealth: true, animateAbilities: abilitiesChanged);

            if (IsDefeated) yield return HandleDefeat();
        }

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();
            yield return _ui.AnimateDefeat();

            Destroy(gameObject);
        }
    }
}
