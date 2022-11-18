using System;
using System.Collections;
using GulchGuardians.Constants;
using GulchGuardians.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GulchGuardians.SceneControls
{
    public class UITitleScene : MonoBehaviour
    {
        [SerializeField] private CanvasRenderer OverlayPanel;
        [SerializeField] private float OverlayPanelTargetAlpha = 0.5f;
        [SerializeField] private CanvasRenderer TitleText;
        [SerializeField] private CanvasGroup StartButton;

        [SerializeField] private float OverlayPanelFadeInDuration = 2.0f;
        [SerializeField] private float TitleTextFadeInDelay = 0.5f;
        [SerializeField] private float TitleTextFadeInDuration = 3f;
        [SerializeField] private float StartButtonFadeInDelay = 2f;
        [SerializeField] private float StartButtonFadeInDuration = 2f;

        private float _overlayPanelTotalAlphaReduction;
        private bool _isGameStarting;

        private void Awake()
        {
            TitleText.SetAlpha(0f);
            StartButton.alpha = 0f;

            _overlayPanelTotalAlphaReduction = 1 - OverlayPanelTargetAlpha;
        }

        private void Start() { StartCoroutine(FadeInElements()); }

        public void OnStartGameClicked()
        {
            if (_isGameStarting) return;
            _isGameStarting = true;

            StartCoroutine(StartGame());
        }

        private static IEnumerator FadeInAlpha(Action<float> alphaSetter, float duration)
        {
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                alphaSetter(t);
                yield return null;
            }

            alphaSetter(1);
        }

        private static IEnumerator FadeOutAlpha(Action<float> alphaSetter, float duration)
        {
            var t = 1f;
            while (t > 0)
            {
                t -= Time.deltaTime / duration;
                alphaSetter(t);
                yield return null;
            }

            alphaSetter(0);
        }

        private IEnumerator StartGame()
        {
            yield return CoroutineWaiter.RunConcurrently(
                StartCoroutine(
                    FadeOutAlpha(
                        a => OverlayPanel.SetAlpha(1 - (_overlayPanelTotalAlphaReduction * a)),
                        duration: 0.5f
                    )
                ),
                StartCoroutine(FadeOutAlpha(alphaSetter: TitleText.SetAlpha, duration: 0.5f)),
                StartCoroutine(FadeOutAlpha(a => StartButton.alpha = a, duration: 0.5f))
            );
            SceneManager.LoadScene(Scenes.MainIndex);
        }

        private IEnumerator FadeInElements()
        {
            yield return FadeInAlpha(
                alpha => { OverlayPanel.SetAlpha(1 - (_overlayPanelTotalAlphaReduction * alpha)); },
                duration: OverlayPanelFadeInDuration
            );

            yield return new WaitForSeconds(TitleTextFadeInDelay);
            yield return FadeInAlpha(
                alphaSetter: TitleText.SetAlpha,
                duration: TitleTextFadeInDuration
            );

            yield return new WaitForSeconds(StartButtonFadeInDelay);
            yield return FadeInAlpha(
                alpha => StartButton.alpha = alpha,
                duration: StartButtonFadeInDuration
            );
        }
    }
}
