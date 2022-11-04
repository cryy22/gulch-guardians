using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GulchGuardians
{
    public class Unit : MonoBehaviour
    {
        public SpriteRenderer Renderer;

        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text NameText;

        private bool _isInitialized;
        private ClickReporter _clickReporter;

        public event EventHandler Clicked
        {
            add => _clickReporter.Clicked += value;
            remove => _clickReporter.Clicked -= value;
        }

        public bool IsDefeated => Health <= 0;

        public bool IsBoss { get; private set; }
        private int Attack { get; set; }
        private int Health { get; set; }
        private int InitialHealth { get; set; }
        private string FirstName { get; set; }

        private void Awake() { _clickReporter = GetComponent<ClickReporter>(); }

        private void Update() { UpdateStats(); }

        public void Initialize(int attack, int health, string firstName = "", bool isBoss = false)
        {
            if (_isInitialized) throw new Exception("Unit is already initialized");

            Attack = attack;
            Health = health;
            InitialHealth = health;
            FirstName = firstName;
            IsBoss = isBoss;

            NameText.text = FirstName;

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
            InitialHealth += health;

            yield return AnimateStatsChange(animateAttack: attack != 0, animateHealth: health != 0);
        }

        public IEnumerator FullHeal()
        {
            Health = InitialHealth;
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

            Renderer.color = Color.red;
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
            HealthText.color = Health < InitialHealth ? Color.red : Color.white;
        }
    }
}
