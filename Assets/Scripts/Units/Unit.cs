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
        public int Attack { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsBoss { get; private set; }
        public bool IsSturdy { get; private set; }
        public bool IsArcher { get; private set; }

        public bool TooltipEnabled { get; set; } = true;

        private void Awake()
        {
            _clickReporter = GetComponent<ClickReporter>();
            _displayer = GetComponent<UnitDisplayer>();
        }

        public IEnumerator AddSturdy()
        {
            IsSturdy = true;
            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateStatsChange(animateAbilities: true);
        }

        public IEnumerator AddArcher()
        {
            IsArcher = true;
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
            IsBoss = attributes.IsBoss;
            IsSturdy = attributes.IsSturdy;
            IsArcher = attributes.IsArcher;

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

        public bool WillAttack(int index) { return index == (IsArcher ? 1 : 0); }

        public IEnumerator AttackUnit(Unit target)
        {
            yield return _displayer.AnimateAttack(target);
            yield return target.TakeDamage(Attack);
        }

        private IEnumerator TakeDamage(int damage)
        {
            var abilitiesChanged = false;
            if (IsSturdy && damage >= Health)
            {
                damage = Health - 1;
                IsSturdy = false;
                abilitiesChanged = true;
            }

            Health -= damage;

            _displayer.UpdateAttributes(BuildAttributes());
            yield return _displayer.AnimateDamage();
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
                IsBoss = IsBoss,
                IsSturdy = IsSturdy,
                IsArcher = IsArcher,
            };
        }

        [Serializable]
        public struct Attributes
        {
            public string FirstName;
            public int Attack;
            public int Health;
            public int MaxHealth;
            public bool IsBoss;
            public bool IsSturdy;
            public bool IsArcher;
        }
    }
}
