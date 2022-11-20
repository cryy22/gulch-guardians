using System;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;

namespace GulchGuardians.UI
{
    public class UIFallenFriends : MonoBehaviour
    {
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private UnitRegistry UnitRegistry;

        [SerializeField] private GameObject NameTagPrefab;
        [SerializeField] private Transform Container;
        [SerializeField] private Transform NameTagContainer;

        private void Awake() { Container.gameObject.SetActive(false); }

        private void OnEnable() { UnitRegistry.Destroying += DestroyingEventHandler; }

        private void OnDisable() { UnitRegistry.Destroying -= DestroyingEventHandler; }

        private void DestroyingEventHandler(object sender, EventArgs _)
        {
            var unit = (Unit) sender;
            if (unit.Team != PlayerTeam) return;

            Container.gameObject.SetActive(true);

            GameObject nameTag = Instantiate(original: NameTagPrefab, parent: NameTagContainer);

            nameTag.GetComponentInChildren<TMP_Text>().text = unit.FirstName;
        }
    }
}
