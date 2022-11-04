using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GulchGuardians
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private SpriteRenderer Renderer;
        [SerializeField] private Animator Animator;

        private bool _isInitialized;
        private ClickReporter _clickReporter;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public bool IsDefeated => Health <= 0;

        public bool TooltipEnabled { get; set; } = true;

        public bool IsBoss { get; private set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string FirstName { get; set; }

        private void Awake() { _clickReporter = GetComponent<ClickReporter>(); }

        private void Update() { UpdateStats(); }

        public void Initialize(
            RuntimeAnimatorController runtimeAnimatorController,
            int attack,
            int health,
            string firstName = "",
            bool isBoss = false
        )
        {
            if (_isInitialized) throw new Exception("Unit is already initialized");

            Animator.runtimeAnimatorController = runtimeAnimatorController;
            Animator.Play(stateNameHash: 0, layer: 0, normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f));

            Attack = attack;
            Health = health;
            MaxHealth = health;
            FirstName = firstName;
            IsBoss = isBoss;

            NameText.text = FirstName;

            if (IsBoss) Renderer.transform.localScale *= 1.33f;

            _isInitialized = true;
        }

        public IEnumerator AttackUnit(Unit target)
        {
            yield return AnimateAttack(target);
            yield return target.TakeDamage(Attack);
        }

        public IEnumerator Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            MaxHealth += health;

            yield return AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator FullHeal()
        {
            Health = MaxHealth;
            yield return AnimateStatsChange(animateHealth: true);
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

        private IEnumerator HandleDefeat()
        {
            yield return new WaitForEndOfFrame();

            AttackText.color = Color.red;
            HealthText.color = Color.red;

            SoundFXPlayer.Instance.PlayDefeatSound();
            yield return AnimateDefeat();

            Destroy(gameObject);
        }

        private IEnumerator TakeDamage(int damage)
        {
            Health -= damage;
            yield return AnimateDamage();
            yield return AnimateStatsChange(animateHealth: true);

            if (IsDefeated) StartCoroutine(HandleDefeat());
        }

        private IEnumerator AnimateAttack(Unit target)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = target.transform.position;
            var duration = 0.0833f;
            var time = 0f;

            while (time < duration)
            {
                transform.position = Vector3.Lerp(a: startPosition, b: endPosition, t: time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            SoundFXPlayer.Instance.PlayAttackSound();
            transform.position = startPosition;
        }

        private IEnumerator AnimateDamage()
        {
            Vector3 startPosition = transform.position;
            var duration = 0.08f;
            var period = 0.04f;
            var time = 0f;

            while (time < duration)
            {
                transform.position = startPosition + (Vector3.up * (0.2f * Mathf.Sin((time / period) * Mathf.PI)));
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = startPosition;
        }

        private IEnumerator AnimateDefeat()
        {
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

        private IEnumerator AnimateStatsChange(bool animateAttack = false, bool animateHealth = false)
        {
            if (!animateAttack && !animateHealth) yield break;

            Vector3 attackCurrentScale = AttackText.transform.localScale;
            Vector3 healthCurrentScale = HealthText.transform.localScale;

            if (animateAttack) AttackText.transform.localScale = attackCurrentScale * 1.5f;
            if (animateHealth) HealthText.transform.localScale = healthCurrentScale * 1.5f;

            yield return new WaitForSeconds(0.5f);

            AttackText.transform.localScale = attackCurrentScale;
            HealthText.transform.localScale = healthCurrentScale;
        }

        private void UpdateStats()
        {
            AttackText.text = Attack.ToString();

            HealthText.text = Health.ToString();
            HealthText.color = Health < MaxHealth ? Color.red : Color.white;
        }
    }
}
