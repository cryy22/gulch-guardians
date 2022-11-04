using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    public class UnitDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private SpriteRenderer Renderer;
        [SerializeField] private SpriteLibrary SpriteLibrary;
        [SerializeField] private Animator Animator;

        public void Setup(SpriteLibraryAsset spriteLibraryAsset, Unit.Attributes attributes)
        {
            SpriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
            Animator.Play(stateNameHash: 0, layer: 0, normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f));

            NameText.text = attributes.FirstName;
            UpdateAttributes(attributes);

            if (attributes.IsBoss) Renderer.transform.localScale *= 1.33f;
        }

        public void UpdateAttributes(Unit.Attributes attributes)
        {
            AttackText.text = attributes.Attack.ToString();
            HealthText.text = attributes.Health.ToString();
            HealthText.color = attributes.Health == attributes.MaxHealth ? Color.white : Color.red;
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

        public IEnumerator AnimateAttack(Unit target)
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

        public IEnumerator AnimateDamage()
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

        public IEnumerator AnimateDefeat()
        {
            AttackText.color = Color.red;
            HealthText.color = Color.red;

            SoundFXPlayer.Instance.PlayDefeatSound();
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

        public IEnumerator AnimateStatsChange(bool animateAttack = false, bool animateHealth = false)
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
    }
}
