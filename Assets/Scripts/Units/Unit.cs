using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    [RequireComponent(typeof(ClickReporter))]
    [RequireComponent(typeof(UnitDisplayer))]
    public class Unit : MonoBehaviour
    {
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
        public bool IsBoss { get; private set; }
        public int Attack { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public bool TooltipEnabled { get; set; } = true;

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            _displayer = GetComponent<UnitDisplayer>();
        }

        public void Initialize(
            SpriteLibraryAsset spriteLibraryAsset,
            int attack,
            int health,
            string firstName = "",
            bool isBoss = false
        )
        {
            if (_isInitialized) throw new Exception("Unit is already initialized");

            Attack = attack;
            Health = health;
            MaxHealth = health;
            FirstName = firstName;
            IsBoss = isBoss;

            _displayer.Setup(spriteLibraryAsset: spriteLibraryAsset, attributes: BuildAttributes());

            _isInitialized = true;
        }

        public IEnumerator AttackUnit(Unit target)
        {
            yield return _displayer.AnimateAttack(target);
            yield return target.TakeDamage(Attack);
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

        public IEnumerator MoveToPosition(Vector3 position, float duration = 0.25f)
        {
            yield return _displayer.AnimateToPosition(position: position, duration: duration);
        }

        private IEnumerator TakeDamage(int damage)
        {
            Health -= damage;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateDamage();
            yield return _displayer.AnimateStatsChange(animateHealth: true);

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
                IsBoss = IsBoss,
            };
        }

        public struct Attributes
        {
            public string FirstName;
            public int Attack;
            public int Health;
            public int MaxHealth;
            public bool IsBoss;
        }
    }
}
