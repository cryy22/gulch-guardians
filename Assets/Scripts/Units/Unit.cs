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
        private int _initialHealth;

        public int Attack { get; private set; }
        public int Health { get; private set; }
        public string FirstName { get; private set; }
        public bool IsBoss { get; private set; }

        private void Update() { UpdateStats(); }

        public void Initialize(int attack, int health, string firstName = "", bool isBoss = false)
        {
            if (_isInitialized) throw new Exception("Unit is already initialized");

            Attack = attack;
            Health = health;
            _initialHealth = health;
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

        public IEnumerator BecomeDefeated()
        {
            Renderer.color = Color.red;
            AttackText.color = Color.red;
            HealthText.color = Color.red;

            yield return AnimateDefeat();

            Destroy(gameObject);
        }

        public void Upgrade(int attack, int health)
        {
            Attack += attack;
            Health += health;
            _initialHealth += health;
        }

        public void FullHeal() { Health = _initialHealth; }

        private IEnumerator TakeDamage(int damage)
        {
            Health -= damage;
            yield return AnimateDamage();
            yield return ShowHealthChange();
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

        private IEnumerator ShowHealthChange()
        {
            Vector3 defaultScale = HealthText.transform.localScale;
            HealthText.transform.localScale = defaultScale * 1.5f;

            yield return new WaitForSeconds(0.5f);
            HealthText.transform.localScale = defaultScale;
        }

        private void UpdateStats()
        {
            AttackText.text = Attack.ToString();

            HealthText.text = Health.ToString();
            HealthText.color = Health < _initialHealth ? Color.red : Color.white;
        }
    }
}
