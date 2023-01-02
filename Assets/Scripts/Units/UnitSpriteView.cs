using System.Collections;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using GulchGuardians.Constants;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians.Units
{
    public class UnitSpriteView : MonoBehaviour
    {
        [SerializeField] private AbilityIndex AbilityIndex;

        private SpriteRenderer _renderer;
        private SpriteLibrary _library;
        private Animator _animator;

        private UnitSpriteAssetMap _assetMap;
        private bool _hasHealerAsset;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _library = GetComponent<SpriteLibrary>();
            _animator = GetComponent<Animator>();
        }

        public void Setup(UnitSpriteAssetMap assetMap, Unit unit)
        {
            _assetMap = assetMap;

            _library.spriteLibraryAsset = _assetMap.Default;
            _hasHealerAsset = _assetMap.Healer != null;

            _animator.Play(
                stateNameHash: 0,
                layer: 0,
                normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f)
            );

            if (unit.HasAbility(AbilityIndex.Boss)) _renderer.transform.localScale *= 2f;
        }

        public void SetIdleAnimation() { _animator.SetTrigger(AnimatorProperties.OnIdleTrigger); }
        public void SetActAnimation() { _animator.SetTrigger(AnimatorProperties.OnActTrigger); }
        public void SetHurtAnimation() { _animator.SetTrigger(AnimatorProperties.OnHurtTrigger); }

        public void UpdateSprite(Unit unit)
        {
            if (_hasHealerAsset && unit.HasAbility(AbilityIndex.Healer))
                _library.spriteLibraryAsset = _assetMap.Healer;
            else
                _library.spriteLibraryAsset = _assetMap.Default;
        }

        public IEnumerator AnimateDefeat()
        {
            _renderer.color = Color.red;

            SoundFXPlayer.Instance.PlayDefeatSound();
            _animator.SetTrigger(AnimatorProperties.OnHurtTrigger);

            const float spinDuration = 0.33f;
            const float spinPeriod = 0.33f;
            const float shrinkDuration = 0.2f;

            Vector3 initialScale = transform.localScale;

            var time = 0f;
            while (time < spinDuration)
            {
                time += Time.deltaTime;
                transform.localScale = new Vector3(
                    x: Mathf.Cos((time / spinPeriod) * Mathf.PI * 2) * initialScale.x,
                    y: initialScale.y,
                    z: initialScale.z
                );

                yield return null;
            }

            time = 0f;
            while (time < shrinkDuration)
            {
                time += Time.deltaTime;

                transform.localScale = Vector3.Slerp(a: initialScale, b: Vector3.zero, t: time / shrinkDuration);
                yield return null;
            }
        }

        public IEnumerator AnimateFlash(Color flashColor, float duration = 0.125f)
        {
            Color startColor = _renderer.color;

            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                _renderer.color = Color.Lerp(a: startColor, b: flashColor, t: t);
                yield return null;
            }

            _renderer.color = startColor;
        }
    }
}
