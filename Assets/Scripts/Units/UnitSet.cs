using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "NewUnitSet", menuName = "Config/Unit Set")]
    public class UnitSet : ScriptableObject
    {
        [SerializeField] private List<UnitConfigQuantity> UnitConfigQuantities;
        [SerializeField] private UnitFactory UnitFactory;

        public List<Unit> GenerateUnits()
        {
            var units = new List<Unit>();

            foreach (UnitConfigQuantity configQuantity in UnitConfigQuantities)
                for (var i = 0; i < configQuantity.Quantity; i++)
                    units.Insert(
                        index: Random.Range(minInclusive: 0, maxExclusive: units.Count + 1),
                        item: UnitFactory.Create(configQuantity.UnitConfig)
                    );

            return units;
        }

        [Serializable]
        public struct UnitConfigQuantity
        {
            public UnitConfig UnitConfig;
            public int Quantity;
        }
    }
}
