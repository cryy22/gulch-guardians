using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using GulchGuardians.Classes;
using GulchGuardians.Constants;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians.Units
{
    public class UnitSpriteView : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private SpriteLibrary _library;
        private Animator _animator;

        private UnitSpriteAssetMap _assetMap;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _library = GetComponent<SpriteLibrary>();
            _animator = GetComponent<Animator>();
        }

        public void Setup(UnitSpriteAssetMap assetMap, IEnumerable<AbilityType> abilities)
        {
            _assetMap = assetMap;

            _library.spriteLibraryAsset = _assetMap.Default;

            _animator.Play(
                stateNameHash: 0,
                layer: 0,
                normalizedTime: Random.Range(minInclusive: 0f, maxInclusive: 1f)
            );

            if (abilities.Contains(AbilityIndex.I.Boss)) _renderer.transform.localScale *= 2f;
        }

        public void SetIdleAnimation() { _animator.SetTrigger(AnimatorProperties.OnIdleTrigger); }
        public void SetActAnimation() { _animator.SetTrigger(AnimatorProperties.OnActTrigger); }
        public void SetHurtAnimation() { _animator.SetTrigger(AnimatorProperties.OnHurtTrigger); }
        public void UpdateSprite(ClassType classType) { _library.spriteLibraryAsset = _assetMap.GetAsset(classType); }

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
