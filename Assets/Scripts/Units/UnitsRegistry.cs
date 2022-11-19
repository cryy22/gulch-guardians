using System;
using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians.Units
{
    [DefaultExecutionOrder(-1)]
    public class UnitsRegistry : MonoBehaviour
    {
        private readonly List<Unit> _units = new();

        public event EventHandler Destroyed;

        public static UnitsRegistry Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            foreach (Unit unit in _units) SubscribeToEvents(unit);
        }

        private void OnDisable()
        {
            foreach (Unit unit in _units) UnsubscribeFromEvents(unit);
        }

        public void Register(Unit unit)
        {
            _units.Add(unit);
            SubscribeToEvents(unit);
        }

        private void SubscribeToEvents(Unit unit) { unit.Destroyed += DestroyedEventHandler; }

        private void UnsubscribeFromEvents(Unit unit) { unit.Destroyed -= DestroyedEventHandler; }

        private void DestroyedEventHandler(object sender, EventArgs e)
        {
            var unit = (Unit) sender;

            Destroyed?.Invoke(sender: unit, e: e);
            UnsubscribeFromEvents(unit);
            _units.Remove(item: unit);
        }
    }
}
