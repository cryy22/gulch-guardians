using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.UI
{
    public class UIGamePhaseAnnouncer : MonoBehaviour
    {
        [SerializeField] private Sprite CombatSign;
        [SerializeField] private Sprite PreparationSign;
        [SerializeField] private Image GamePhaseLabel;

        [SerializeField] private AnimationCurve AnnouncementAnimationCurve;
        [SerializeField] private float AnnouncementDuration = 1.5f;

        private void Start() { GamePhaseLabel.gameObject.SetActive(false); }

        public IEnumerator AnnouncePhase(bool isPreparation)
        {
            GamePhaseLabel.gameObject.SetActive(true);
            GamePhaseLabel.sprite = isPreparation ? PreparationSign : CombatSign;

            Vector3 start = transform.position;
            Vector3 end = start + (Vector3.up * (Screen.height + GetLabelHeight()));

            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / AnnouncementDuration;
                transform.position = Vector3.Lerp(a: start, b: end, t: AnnouncementAnimationCurve.Evaluate(t));
                yield return null;
            }

            transform.position = start;
            GamePhaseLabel.gameObject.SetActive(false);
        }

        private float GetLabelHeight()
        {
            var corners = new Vector3[4];
            GamePhaseLabel.rectTransform.GetWorldCorners(corners);
            return corners[2].y - corners[0].y;
        }
    }
}
