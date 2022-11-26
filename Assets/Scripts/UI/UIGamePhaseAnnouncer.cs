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

        private Camera _camera;

        private void Awake() { _camera = Camera.main; }

        public IEnumerator AnnouncePhase(bool isPreparation)
        {
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
        }

        private float GetLabelHeight()
        {
            var corners = new Vector3[4];
            GamePhaseLabel.rectTransform.GetWorldCorners(corners);
            return corners[2].y - corners[0].y;
        }
    }
}
