using System.Collections;
using UnityEngine;

namespace GulchGuardians.UI
{
    public class UIBackgroundAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Background0;
        [SerializeField] private SpriteRenderer Background1;
        [SerializeField] private SpriteRenderer Background2;
        [SerializeField] private SpriteRenderer Background3;

        [SerializeField] private float B0Magnitude;
        [SerializeField] private float B1Magnitude;
        [SerializeField] private float B2Magnitude;
        [SerializeField] private float B3Magnitude;

        [SerializeField] private float AnimationSpeed = 1f;

        private void Start() { StartCoroutine(AnimateBackground()); }

        private IEnumerator AnimateBackground()
        {
            var t = 0f;

            Vector3 i0 = Background0.transform.position;
            Vector3 i1 = Background1.transform.position;
            Vector3 i2 = Background2.transform.position;
            Vector3 i3 = Background3.transform.position;

            while (true)
            {
                t += Time.deltaTime / AnimationSpeed;

                float d0 = Mathf.Sin(t) * B0Magnitude;
                float d1 = Mathf.Sin(t) * B1Magnitude;
                float d2 = Mathf.Sin(t) * B2Magnitude;
                float d3 = Mathf.Sin(t) * B3Magnitude;

                Background0.transform.position = new Vector3(x: i0.x + d0, y: i0.y, z: i0.z);
                Background1.transform.position = new Vector3(x: i1.x + d1, y: i1.y, z: i1.z);
                Background2.transform.position = new Vector3(x: i2.x + d2, y: i2.y, z: i2.z);
                Background3.transform.position = new Vector3(x: i3.x + d3, y: i3.y, z: i3.z);

                yield return null;
            }
        }
    }
}
