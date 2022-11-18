using System;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Teams
{
    public class UIEnemyUnitCount : MonoBehaviour
    {
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private TMP_Text CounterText;
        [SerializeField] private TMP_Text SuffixText;

        private void OnEnable()
        {
            EnemyTeam.UnitsChanged += UnitsChangedEventHandler;
            UnitsChangedEventHandler(sender: this, e: EventArgs.Empty);
        }

        private void OnDisable() { EnemyTeam.UnitsChanged -= UnitsChangedEventHandler; }

        private void UnitsChangedEventHandler(object sender, EventArgs e) { UpdateCounter(); }

        private void UpdateCounter()
        {
            int unitCount = EnemyTeam.Units.Count;
            CounterText.text = unitCount.ToString();
            SuffixText.text = unitCount == 1 ? "enemy unit remaining" : "enemy units remaining";
        }
    }
}
